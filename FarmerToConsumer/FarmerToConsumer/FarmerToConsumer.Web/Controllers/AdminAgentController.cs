using FarmerToConsumer.Entities;
using FarmerToConsumer.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers
{
    [Authorize(Roles = "admin,superadmin")]
    public class AdminAgentController(
        UserInfoRepo userRepo,
        AgentRepo agentRepo,
        AgentAssignmentRepo agentAssignmentRepo
    ) : Controller
    {
        public IActionResult AgentRequests()
        {
            var users = userRepo.GetAll().Data ?? new List<UserInfo>();
            return View("~/Views/AdminDashboard/AgentRequests.cshtml", users.Where(u => NormalizeRole(u.Role) == "agent_pending").ToList());
        }

        [HttpPost]
        public IActionResult HireAgent(int id, string village)
        {
            var user = userRepo.GetById(id).Data;
            if (user == null)
            {
                TempData["Error"] = "Agent request not found.";
                return RedirectToAction("AgentRequests");
            }

            user.Role = "agent";
            user.Village = string.IsNullOrWhiteSpace(village) ? user.Village : village;

            var saveUserResult = userRepo.Save(user);
            if (saveUserResult.HasError)
            {
                TempData["Error"] = saveUserResult.Message;
                return RedirectToAction("AgentRequests");
            }

            var agentResult = agentRepo.GetByUserInfoId(user.ID);
            Agent agent;

            if (agentResult.HasError || agentResult.Data == null)
            {
                var saveAgentResult = agentRepo.Save(new Agent
                {
                    UserInfoID = user.ID,
                    Name = user.Name,
                    Phone = user.Phone ?? "",
                    Area = user.Village ?? "",
                    CommissionRate = 5
                });

                if (saveAgentResult.HasError || saveAgentResult.Data == null)
                {
                    TempData["Error"] = saveAgentResult.Message;
                    return RedirectToAction("AgentRequests");
                }

                agent = saveAgentResult.Data;
            }
            else
            {
                agent = agentResult.Data;
                agent.Name = user.Name;
                agent.Phone = user.Phone ?? "";
                agent.Area = user.Village ?? "";
                agentRepo.Save(agent);
            }

            var assignmentResult = agentAssignmentRepo.AssignAgentWork(agent.ID, agent.Area);

            TempData[assignmentResult.HasError ? "Error" : "Success"] = assignmentResult.HasError ? assignmentResult.Message : "Agent approved and assigned.";
            return RedirectToAction("AgentRequests");
        }

        public IActionResult Assignments()
        {
            var agentResult = agentRepo.GetAll();
            if (agentResult.HasError)
            {
                ViewBag.Error = agentResult.Message;
            }

            var assignmentResult = agentAssignmentRepo.GetAll();
            if (assignmentResult.HasError)
            {
                ViewBag.Error = assignmentResult.Message;
            }

            ViewBag.Agents = agentResult.Data ?? new List<Agent>();
            ViewBag.Assignments = assignmentResult.Data ?? new List<AgentAssignment>();
            return View("~/Views/AdminDashboard/AgentAssignments.cshtml");
        }

        public IActionResult AssignmentDetails(int id)
        {
            var result = agentAssignmentRepo.GetAssignedAgent(id);
            if (result.HasError || result.Data == null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Assignments");
            }

            ViewBag.Assignment = agentAssignmentRepo.GetByAgentId(id).Data?.OrderByDescending(e => e.ID).FirstOrDefault();
            return View("~/Views/AdminDashboard/AgentAssignmentDetails.cshtml", result.Data);
        }

        [HttpPost]
        public IActionResult AssignAgentWork(int agentId, string village)
        {
            var result = agentAssignmentRepo.AssignAgentWork(agentId, village);
            TempData[result.HasError ? "Error" : "Success"] = result.HasError ? result.Message : "Agent work village assigned.";
            return RedirectToAction("Assignments");
        }

        [HttpPost]
        public IActionResult UpdateAssignment(int agentId, string village)
        {
            var result = agentAssignmentRepo.AssignAgentWork(agentId, village);
            TempData[result.HasError ? "Error" : "Success"] = result.HasError ? result.Message : "Assignment updated.";
            return RedirectToAction("AssignmentDetails", new { id = agentId });
        }

        [HttpPost]
        public IActionResult DeleteAssignment(int id)
        {
            var result = agentAssignmentRepo.DeleteAgentAssignment(id);
            TempData[result.HasError ? "Error" : "Success"] = result.HasError ? result.Message : "Assignment deleted.";
            return RedirectToAction("Assignments");
        }

        private static string NormalizeRole(string role)
        {
            return (role ?? "").Replace(" ", "").ToLower();
        }
    }
}
