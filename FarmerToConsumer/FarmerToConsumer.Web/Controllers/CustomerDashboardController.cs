using FarmerToConsumer.Repos;
using FarmerToConsumer.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers;

public class CustomerDashboardController(CustomerShopRepo repo, CurrentUserHelper currentUser) : Controller
{
    public IActionResult Index()
    {
        if (currentUser.UserId <= 0)
        {
            return RedirectToAction("Login", "Account");
        }

        var result = repo.GetDashboard(currentUser.UserId);
        if (result.HasError)
        {
            ViewBag.Error = result.Message;
        }

        return View(result.Data);
    }
}
