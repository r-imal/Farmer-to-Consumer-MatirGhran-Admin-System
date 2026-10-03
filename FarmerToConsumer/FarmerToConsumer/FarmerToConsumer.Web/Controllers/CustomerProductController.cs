using FarmerToConsumer.Repos;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers;

public class CustomerProductController(CustomerShopRepo repo) : Controller
{
    public IActionResult Index(string? category, string? search)
    {
        return RedirectToAction("Index", "Home", new { category, search });
    }

    public IActionResult Details(int dataId)
    {
        var result = repo.GetStock(dataId);
        if (result.HasError || result.Data == null)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction("Index");
        }

        return View(result.Data);
    }
}
