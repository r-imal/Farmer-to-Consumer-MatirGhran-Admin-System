using System.Security.Claims;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Models;
using FarmerToConsumer.Repos;
using FarmerToConsumer.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers
{
    public class AccountController(UserInfoRepo userRepo, CurrentUserHelper currentUserHelper) : Controller
    {
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var result = userRepo.Authenticate(model.Login, model.Password);
            if (result.HasError || result.Data == null)
            {
                ViewBag.Error = "Invalid login information.";
                return View(model);
            }

            var user = result.Data;
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, NormalizeRole(user.Role)),
                new Claim("Email", user.Email),
                new Claim("UserId", user.ID.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "FtcAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("FtcAuth", principal);

            return RedirectToDashboard(user.Role);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("FtcAuth");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RegisterConsumer()
        {
            return View("Register", new UserInfo { Role = "Consumer" });
        }

        public IActionResult RegisterAgent()
        {
            return View("Register", new UserInfo { Role = "agent_pending" });
        }

        [HttpPost]
        public IActionResult Register(UserInfo model)
        {
            model.Role = NormalizeRole(model.Role) == "agent_pending" ? "agent_pending" : "Consumer";

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var result = userRepo.Save(model);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            TempData["Success"] = model.Role == "agent_pending"
                ? "Agent registration submitted for approval."
                : "Consumer registration completed.";

            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            var user = CurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        public IActionResult Denied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserInfo model)
        {
            var user = CurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            model.ID = user.ID;
            model.Email = string.IsNullOrWhiteSpace(model.Email) ? user.Email : model.Email;
            model.Password = user.Password;
            model.Role = user.Role;
            model.CreatedAt = user.CreatedAt;

            var result = userRepo.Save(model);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(user);
            }

            if (result.Data != null)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, result.Data.Name),
                    new Claim(ClaimTypes.Role, NormalizeRole(result.Data.Role)),
                    new Claim("Email", result.Data.Email),
                    new Claim("UserId", result.Data.ID.ToString()),
                };

                var identity = new ClaimsIdentity(claims, "FtcAuth");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("FtcAuth", principal);
            }

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var user = CurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (user.Password != currentPassword)
            {
                TempData["Error"] = "Current password is incorrect.";
                return RedirectToAction("Profile");
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword != confirmPassword)
            {
                TempData["Error"] = "New password and confirm password must match.";
                return RedirectToAction("Profile");
            }

            var result = userRepo.ChangePassword(user.ID, currentPassword, newPassword);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Profile");
            }

            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Profile");
        }

        private UserInfo? CurrentUser()
        {
            var userId = currentUserHelper.UserId;
            return userId > 0 ? userRepo.GetById(userId).Data : null;
        }

        private IActionResult RedirectToDashboard(string role)
        {
            var normalized = NormalizeRole(role);
            if (normalized is "admin" or "superadmin")
            {
                return RedirectToAction("Index", "AdminDashboard");
            }

            if (normalized == "agent")
            {
                return RedirectToAction("Index", "AgentDashboard");
            }

            if (normalized == "consumer")
            {
                return RedirectToAction("Index", "CustomerDashboard");
            }

            return RedirectToAction("Index", "Home");
        }

        private static string NormalizeRole(string role)
        {
            return (role ?? "").Replace(" ", "").ToLower();
        }
    }
}
