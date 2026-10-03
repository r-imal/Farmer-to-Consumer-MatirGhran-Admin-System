using FarmerToConsumer.Repos;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers
{
    public class HomeController(CustomerShopRepo repo) : Controller
    {
        public IActionResult Index(string? category, string? search)
        {
            var result = repo.GetProducts(category, search);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
            }

            return View(result.Data);
        }
    }
}
