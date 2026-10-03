using FarmerToConsumer.Entities;
using FarmerToConsumer.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers
{
    [Authorize(Roles = "admin,superadmin")]
    public class AdminPaymentController(UserInfoRepo userRepo, AgentPaymentRepo agentPaymentRepo) : Controller
    {
        public IActionResult Payments(int? agentId, int? historyAgentId)
        {
            var agents = userRepo.GetAll().Data?.Where(u => NormalizeRole(u.Role) == "agent").ToList() ?? new List<UserInfo>();
            var history = agentPaymentRepo.GetAgentPaymentHistory().Data ?? new List<AgentPaymentHistoryItem>();
            if (historyAgentId.HasValue)
            {
                history = history.Where(h => h.AgentId == historyAgentId.Value).ToList();
            }

            ViewBag.Agents = agents;
            ViewBag.SelectedAgent = agentId.HasValue ? agents.FirstOrDefault(a => a.ID == agentId.Value) : null;
            ViewBag.HistoryAgent = historyAgentId.HasValue ? agents.FirstOrDefault(a => a.ID == historyAgentId.Value) : null;
            ViewBag.PaymentHistory = history;
            return View("~/Views/AdminDashboard/Payments.cshtml");
        }

        [HttpPost]
        public IActionResult PayAgent(int agentId, int amount)
        {
            var agent = userRepo.GetById(agentId).Data;
            if (agent == null || NormalizeRole(agent.Role) != "agent")
            {
                TempData["Error"] = "Agent not found.";
                return RedirectToAction("Payments");
            }

            var result = agentPaymentRepo.PayAgentSalary(agent.ID, amount);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Payments", new { agentId = agent.ID });
            }

            TempData["LastPayment"] = $"{agent.Name}: Sent {amount:0} BDT";
            TempData["Success"] = "Payment sent.";
            return RedirectToAction("Payments", new { agentId = agent.ID });
        }

        private static string NormalizeRole(string role)
        {
            return (role ?? "").Replace(" ", "").ToLower();
        }
    }
}
