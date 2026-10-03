using FarmerToConsumer.Entities;
using FarmerToConsumer.Repos;
using FarmerToConsumer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FarmerToConsumer.Web.Controllers
{
    [Authorize(Roles = "admin,superadmin,agent")]
    public class ProductController(
        ProductRepo productRepo,
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

                var result = productRepo.GetAllByAgentId(agentResult.Data.ID);

                if (result.HasError)
                {
                    ViewBag.Error = result.Message;
                }

                return View(result.Data);
            }
            else
            {
                var result = productRepo.GetAll();

                if (result.HasError)
                {
                    ViewBag.Error = result.Message;
                }

                return View(result.Data);
            }
        }

        public IActionResult Detail(int dataId = -1)
        {
            Result<List<Farmer>> resultFarmer;

            if (User.IsInRole("agent"))
            {
                var agentResult = agentRepo.GetByUserInfoId(currentUser.UserId);

                if (agentResult.HasError || agentResult.Data == null)
                {
                    TempData["Error"] = agentResult.Message;
                    return RedirectToAction("Denied", "Account");
                }

                resultFarmer = farmerRepo.GetAllByAgentId(agentResult.Data.ID);
            }
            else
            {
                resultFarmer = farmerRepo.GetAll();
            }

            var farmers = resultFarmer.Data ?? new List<Farmer>();
            ViewBag.FarmerList = farmers.Select(e =>
                new SelectListItem()
                {
                    Text = e.Name,
                    Value = e.ID.ToString()
                }).ToList();

            if (dataId == -1)
            {
                return View(new Product());
            }

            var result = productRepo.GetById(dataId);

            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }

            if (result.Data == null)
            {
                TempData["Error"] = "Invalid Product Id.";
                return RedirectToAction("Index");
            }

            return View(result.Data);
        }

        [HttpPost]
        public IActionResult Detail(Product model)
        {
            Result<List<Farmer>> resultFarmer;

            if (User.IsInRole("agent"))
            {
                var agentResult = agentRepo.GetByUserInfoId(currentUser.UserId);

                if (agentResult.HasError || agentResult.Data == null)
                {
                    TempData["Error"] = agentResult.Message;
                    return RedirectToAction("Denied", "Account");
                }

                resultFarmer = farmerRepo.GetAllByAgentId(agentResult.Data.ID);

                var farmers = resultFarmer.Data ?? new List<Farmer>();
                if (!farmers.Any(e => e.ID == model.FarmerId))
                {
                    TempData["Error"] = "Invalid Farmer.";
                    return RedirectToAction("Denied", "Account");
                }
            }
            else
            {
                resultFarmer = farmerRepo.GetAll();
            }

            var farmerList = resultFarmer.Data ?? new List<Farmer>();
            ViewBag.FarmerList = farmerList.Select(e =>
                new SelectListItem()
                {
                    Text = e.Name,
                    Value = e.ID.ToString()
                }).ToList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var result = productRepo.Save(model);

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
            var result = productRepo.Delete(dataId);

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
