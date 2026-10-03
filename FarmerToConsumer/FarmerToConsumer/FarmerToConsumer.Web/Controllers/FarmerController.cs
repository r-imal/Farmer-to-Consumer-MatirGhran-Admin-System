using FarmerToConsumer.Entities;
using FarmerToConsumer.Repos;
using FarmerToConsumer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers
{
    [Authorize(Roles = "admin,superadmin,agent")]
    public class FarmerController(
        FarmerRepo farmerRepo,
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

                var result = farmerRepo.GetAllByAgentId(agentResult.Data.ID);

                if (result.HasError)
                {
                    ViewBag.Error = result.Message;
                }

                return View(result.Data);
            }
            else
            {
                var result = farmerRepo.GetAll();

                if (result.HasError)
                {
                    ViewBag.Error = result.Message;
                }

                return View(result.Data);
            }
        }

        public IActionResult Detail(int dataId = -1)
        {
            if (dataId == -1)
            {
                return View(new Farmer());
            }

            var result = farmerRepo.GetById(dataId);

            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }

            if (result.Data == null)
            {
                TempData["Error"] = "Invalid Farmer Id.";
                return RedirectToAction("Index");
            }

            return View(result.Data);
        }

        [HttpPost]
        public IActionResult Detail(Farmer model)
        {
            if (User.IsInRole("agent"))
            {
                var agentResult = agentRepo.GetByUserInfoId(currentUser.UserId);

                if (agentResult.HasError || agentResult.Data == null)
                {
                    TempData["Error"] = agentResult.Message;
                    return RedirectToAction("Denied", "Account");
                }

                model.AgentId = agentResult.Data.ID;
            }

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var result = farmerRepo.Save(model);

            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            if (result.Data != null)
            {
                TempData["Success"] = $"Data#{result.Data.ID} saved Successfully";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int dataId)
        {
            var result = farmerRepo.Delete(dataId);

            if (result.HasError)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = $"Data#{dataId} deleted Successfully";
            }

            return RedirectToAction("Index");
        }
    }
}
