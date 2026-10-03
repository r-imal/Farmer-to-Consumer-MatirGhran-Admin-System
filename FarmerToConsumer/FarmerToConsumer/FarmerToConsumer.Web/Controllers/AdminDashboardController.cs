using FarmerToConsumer.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers
{
    [Authorize(Roles = "admin,superadmin")]
    public class AdminDashboardController(AdminDashboardRepo adminDashboardRepo) : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Users = adminDashboardRepo.GetUsers().Data;
            ViewBag.Orders = adminDashboardRepo.GetOrders().Data;
            return View();
        }
    }
}
