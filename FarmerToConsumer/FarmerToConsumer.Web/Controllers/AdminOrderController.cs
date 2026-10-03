using FarmerToConsumer.Entities;
using FarmerToConsumer.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers
{
    [Authorize(Roles = "admin,superadmin")]
    public class AdminOrderController(OrderRepo orderRepo) : Controller
    {
        public IActionResult Orders()
        {
            return View("~/Views/AdminDashboard/Orders.cshtml", orderRepo.GetAll().Data ?? new List<CustomerOrder>());
        }

        public IActionResult OrderLogDetails()
        {
            ViewBag.Orders = orderRepo.GetAll().Data ?? new List<CustomerOrder>();
            return View("~/Views/AdminDashboard/OrderLogDetails.cshtml");
        }
    }
}
