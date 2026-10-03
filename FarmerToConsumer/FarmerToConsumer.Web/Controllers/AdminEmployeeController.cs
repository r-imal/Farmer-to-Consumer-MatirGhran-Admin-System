using FarmerToConsumer.Entities;
using FarmerToConsumer.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers
{
    [Authorize(Roles = "admin,superadmin")]
    public class AdminEmployeeController(UserInfoRepo userRepo) : Controller
    {
        public IActionResult Employees()
        {
            var result = userRepo.GetAll();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View("~/Views/AdminDashboard/Employees.cshtml", new List<UserInfo>());
            }

            var employees = (result.Data ?? new List<UserInfo>())
                .Where(u => IsEmployeeRole(u.Role))
                .OrderBy(u => RoleRank(u.Role))
                .ThenBy(u => u.Name)
                .ToList();
            return View("~/Views/AdminDashboard/Employees.cshtml", employees);
        }

        public IActionResult EditEmployee(int id)
        {
            var result = userRepo.GetById(id);
            if (result.HasError || result.Data == null || !IsEmployeeRole(result.Data.Role))
            {
                TempData["Error"] = "Employee not found.";
                return RedirectToAction("Employees");
            }

            if (!CanEditEmployee(result.Data))
            {
                TempData["Error"] = "You do not have permission to edit this employee.";
                return RedirectToAction("Employees");
            }

            return View("~/Views/AdminDashboard/EditEmployee.cshtml", result.Data);
        }

        [HttpPost]
        public IActionResult EditEmployee(UserInfo model)
        {
            var existing = userRepo.GetById(model.ID).Data;
            if (existing == null || !CanEditEmployee(existing))
            {
                TempData["Error"] = "You do not have permission to update this employee.";
                return RedirectToAction("Employees");
            }

            existing.Name = model.Name;
            existing.Email = model.Email;
            existing.Phone = model.Phone;
            existing.Village = model.Village;
            existing.District = model.District;
            existing.Address = model.Address;
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                existing.Password = model.Password;
            }

            if (NormalizeRole(existing.Role) == "superadmin")
            {
                existing.Role = "superadmin";
            }
            else
            {
                var role = NormalizeRole(model.Role);
                existing.Role = role switch
                {
                    "admin" => "admin",
                    "agent" => "agent",
                    _ => "agent"
                };
            }

            var result = userRepo.Save(existing);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View("~/Views/AdminDashboard/EditEmployee.cshtml", existing);
            }

            TempData["Success"] = "Employee information updated.";
            return RedirectToAction("Employees");
        }

        [HttpPost]
        public IActionResult DeleteEmployee(int id)
        {
            var target = userRepo.GetById(id).Data;
            if (target == null)
            {
                TempData["Error"] = "Employee not found.";
                return RedirectToAction("Employees");
            }

            if (!CanDeleteEmployee(target))
            {
                TempData["Error"] = "This employee cannot be deleted by your role.";
                return RedirectToAction("Employees");
            }

            var result = userRepo.Delete(id);
            TempData[result.HasError ? "Error" : "Success"] = result.HasError ? result.Message : "Employee deleted.";
            return RedirectToAction("Employees");
        }

        public IActionResult Users()
        {
            return View("~/Views/AdminDashboard/Users.cshtml", userRepo.GetAll().Data);
        }

        public IActionResult EditUser(int id)
        {
            var result = userRepo.GetById(id);
            if (result.HasError || result.Data == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Users");
            }

            if (!CanEditUser(result.Data))
            {
                TempData["Error"] = "You do not have permission to edit this user.";
                return RedirectToAction("Users");
            }

            return View("~/Views/AdminDashboard/EditUser.cshtml", result.Data);
        }

        [HttpPost]
        public IActionResult EditUser(UserInfo model)
        {
            var existing = userRepo.GetById(model.ID).Data;
            if (existing == null || !CanEditUser(existing))
            {
                TempData["Error"] = "You do not have permission to edit this user.";
                return RedirectToAction("Users");
            }

            existing.Name = model.Name;
            existing.Email = model.Email;
            existing.Phone = model.Phone;
            existing.Village = model.Village;
            existing.District = model.District;
            existing.Address = model.Address;
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                existing.Password = model.Password;
            }

            if (NormalizeRole(existing.Role) != "superadmin")
            {
                var role = NormalizeRole(model.Role);
                existing.Role = role switch
                {
                    "admin" => "admin",
                    "agent" => "agent",
                    _ => "consumer"
                };
            }

            var result = userRepo.Save(existing);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View("~/Views/AdminDashboard/EditUser.cshtml", existing);
            }

            TempData["Success"] = "User information updated.";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var target = userRepo.GetById(id).Data;
            if (target == null)
            {
                return RedirectToAction("Users");
            }

            var targetRole = NormalizeRole(target.Role);
            if (targetRole == "superadmin")
            {
                TempData["Error"] = "This user cannot be deleted by your role.";
                return RedirectToAction("Users");
            }

            userRepo.Delete(id);
            return RedirectToAction("Users");
        }

        private static bool IsEmployeeRole(string role)
        {
            var normalized = NormalizeRole(role);
            return normalized == "admin" || normalized == "agent";
        }

        private static string NormalizeRole(string role)
        {
            return (role ?? "").Replace(" ", "").ToLower();
        }

        private static int RoleRank(string role)
        {
            return NormalizeRole(role) switch
            {
                "admin" => 1,
                "agent" => 2,
                _ => 9
            };
        }

        private bool CanEditEmployee(UserInfo target)
        {
            var targetRole = NormalizeRole(target.Role);
            return targetRole == "superadmin" || targetRole == "admin" || targetRole == "agent";
        }

        private bool CanEditUser(UserInfo target)
        {
            var targetRole = NormalizeRole(target.Role);
            return targetRole == "superadmin" || targetRole == "admin" || targetRole == "agent" || targetRole == "consumer";
        }

        private bool CanDeleteEmployee(UserInfo target)
        {
            var targetRole = NormalizeRole(target.Role);
            return targetRole == "admin" || targetRole == "agent";
        }
    }
}
