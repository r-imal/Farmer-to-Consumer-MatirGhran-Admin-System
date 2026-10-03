using FarmerToConsumer.Repos;
using FarmerToConsumer.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers;

public class CustomerOrderController(CustomerShopRepo repo, CurrentUserHelper currentUser) : Controller
{
    public IActionResult Index()
    {
        if (currentUser.UserId <= 0)
        {
            return RedirectToAction("Login", "Account");
        }

        var result = repo.GetOrders(currentUser.UserId);
        if (result.HasError)
        {
            ViewBag.Error = result.Message;
        }

        return View(result.Data);
    }

    public IActionResult Detail(int dataId)
    {
        if (currentUser.UserId <= 0)
        {
            return RedirectToAction("Login", "Account");
        }

        var result = repo.GetOrderDetail(currentUser.UserId, dataId);
        if (result.HasError || result.Data == null)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction("Index");
        }

        return View(result.Data);
    }

    public IActionResult Pay(int dataId)
    {
        var result = repo.PayOrder(currentUser.UserId, dataId);
        TempData[result.HasError ? "Error" : "Success"] = result.HasError ? result.Message : "Payment completed online.";
        return RedirectToAction("Detail", new { dataId });
    }
}
