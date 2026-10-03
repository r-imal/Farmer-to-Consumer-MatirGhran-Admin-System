using FarmerToConsumer.Entities;
using FarmerToConsumer.Repos;
using FarmerToConsumer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FarmerToConsumer.Web.Controllers
{
    [Authorize(Roles = "admin,superadmin,agent")]
    public class ProductStockController(
        ProductStockRepo productStockRepo,
        ProductRepo productRepo,
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

                var result = productStockRepo.GetAllByAgentId(agentResult.Data.ID);

                if (result.HasError)
                {
                    ViewBag.Error = result.Message;
                }

                return View(result.Data);
            }
            else
            {
                var result = productStockRepo.GetAll();

                if (result.HasError)
                {
                    ViewBag.Error = result.Message;
                }

                return View(result.Data);
            }
        }

        public IActionResult Detail(int dataId = -1)
        {
            Result<List<Product>> resultProduct;

            if (User.IsInRole("agent"))
            {
                var agentResult = agentRepo.GetByUserInfoId(currentUser.UserId);

                if (agentResult.HasError || agentResult.Data == null)
                {
                    TempData["Error"] = agentResult.Message;
                    return RedirectToAction("Denied", "Account");
                }

                resultProduct = productRepo.GetAllByAgentId(agentResult.Data.ID);
            }
            else
            {
                resultProduct = productRepo.GetAll();
            }

            var products = resultProduct.Data ?? new List<Product>();
            ViewBag.ProductList = products.Select(e =>
                new SelectListItem()
                {
                    Text = e.Name,
                    Value = e.ID.ToString()
                }).ToList();

            if (dataId == -1)
            {
                return View(new ProductStock());
            }

            var result = productStockRepo.GetById(dataId);

            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }

            if (result.Data == null)
            {
                TempData["Error"] = "Invalid Product Stock Id.";
                return RedirectToAction("Index");
            }

            return View(result.Data);
        }

        [HttpPost]
        public IActionResult Detail(ProductStock model)
        {
            Result<List<Product>> resultProduct;

            if (User.IsInRole("agent"))
            {
                var agentResult = agentRepo.GetByUserInfoId(currentUser.UserId);

                if (agentResult.HasError || agentResult.Data == null)
                {
                    TempData["Error"] = agentResult.Message;
                    return RedirectToAction("Denied", "Account");
                }

                resultProduct = productRepo.GetAllByAgentId(agentResult.Data.ID);

                var products = resultProduct.Data ?? new List<Product>();
                if (!products.Any(e => e.ID == model.ProductID))
                {
                    TempData["Error"] = "Invalid Product.";
                    return RedirectToAction("Denied", "Account");
                }
            }
            else
            {
                resultProduct = productRepo.GetAll();
            }

            var productList = resultProduct.Data ?? new List<Product>();
            ViewBag.ProductList = productList.Select(e =>
                new SelectListItem()
                {
                    Text = e.Name,
                    Value = e.ID.ToString()
                }).ToList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var result = productStockRepo.Save(model);

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
            var result = productStockRepo.Delete(dataId);

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
