using FarmerToConsumer.Repos;
using FarmerToConsumer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers
{
    [Authorize(Roles = "admin,superadmin,agent")]
    public class OrderController(
        OrderRepo orderRepo,
        AgentRepo agentRepo,
        CurrentUserHelper currentUser
    ) : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("agent"))
            {
                var agentResult = agentRepo.GetByUserInfoId(currentUser.UserId);

                if (agentResult.HasError || agentResult.Data == null)
                {
                    TempData["Error"] = agentResult.Message;
                    return RedirectToAction("Denied", "Account");
                }

                var result = orderRepo.GetAllByAgentId(agentResult.Data.ID);

                if (result.HasError)
                {
                    ViewBag.Error = result.Message;
                }

                return View(result.Data);
            }
            else
            {
                var result = orderRepo.GetAll();

                if (result.HasError)
                {
                    ViewBag.Error = result.Message;
                }

                return View(result.Data);
            }
        }

        public IActionResult UpdateStatus(int orderId, string status)
        {
            Result<bool> result;

            if (User.IsInRole("agent"))
            {
                var agentResult = agentRepo.GetByUserInfoId(currentUser.UserId);

                if (agentResult.HasError || agentResult.Data == null)
                {
                    TempData["Error"] = agentResult.Message;
                    return RedirectToAction("Denied", "Account");
                }

                result = orderRepo.UpdateStatus(orderId, status, agentResult.Data.ID);
            }
            else
            {
                result = orderRepo.UpdateStatus(orderId, status);
            }

            if (result.HasError)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = $"Order#{orderId} status updated Successfully";
            }

            return RedirectToAction("Index");
        }
    }
}