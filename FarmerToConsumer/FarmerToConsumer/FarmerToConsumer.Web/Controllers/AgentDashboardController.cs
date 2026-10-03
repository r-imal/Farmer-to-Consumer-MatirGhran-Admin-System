using System.Collections.Generic;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Repos;
using FarmerToConsumer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers
{
    [Authorize(Roles = "agent")]
    public class AgentDashboardController(
        AgentRepo agentRepo,
        ProductRepo productRepo,
        OrderRepo orderRepo,
        CurrentUserHelper currentUserHelper
    ) : Controller
    {
        public IActionResult Index()
        {
            var agentResult = agentRepo.GetByUserInfoId(currentUserHelper.UserId);
            if (agentResult.HasError)
            {
                ViewBag.Error = agentResult.Message;
            }
            var agent = agentResult.Data;

            ViewBag.AgentName = "";
            ViewBag.AgentArea = "";
            ViewBag.ProductCount = 0;
            ViewBag.OrderCount = 0;
            ViewBag.CommissionRate = 0;

            if (agent != null)
            {
                var productResult = productRepo.GetAllByAgentId(agent.ID);
                var orderResult = orderRepo.GetAllByAgentId(agent.ID);

                if (productResult.HasError)
                {
                    ViewBag.Error = productResult.Message;
                }

                if (orderResult.HasError)
                {
                    ViewBag.Error = orderResult.Message;
                }

                ViewBag.AgentName = agent.Name;
                ViewBag.AgentArea = agent.Area;
                ViewBag.CommissionRate = agent.CommissionRate;
                ViewBag.ProductCount = (productResult.Data ?? new List<Product>()).Count;
                ViewBag.OrderCount = (orderResult.Data ?? new List<CustomerOrder>()).Count;
            }

            return View(agent);
        }
    }
}
