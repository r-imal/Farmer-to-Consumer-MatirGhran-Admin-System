using FarmerToConsumer.Data;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Shared;

namespace FarmerToConsumer.Repos
{
    public class AgentAssignmentRepo(FtcDbContext context)
    {
        public Result<List<AgentAssignment>> GetAll()
        {
            var result = new Result<List<AgentAssignment>>()
            {
                Data = new List<AgentAssignment>()
            };

            try
            {
                result.Data = context.AgentAssignments.ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<List<AgentAssignment>> GetByAgentId(int agentId)
        {
            var result = new Result<List<AgentAssignment>>()
            {
                Data = new List<AgentAssignment>()
            };

            try
            {
                result.Data = context.AgentAssignments
                    .Where(e => e.AgentUserId == agentId)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Agent> GetAssignedAgent(int agentId)
        {
            var result = new Result<Agent>();

            try
            {
                result.Data = context.Agents.Find(agentId);

                if (result.Data == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Agent Id.";
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<bool> AssignAgentWork(int agentId, string village)
        {
            var result = new Result<bool>();

            try
            {
                var agent = context.Agents.Find(agentId);

                if (agent == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Agent Id.";
                    return result;
                }

                agent.Area = village;

                var assignment = context.AgentAssignments
                    .FirstOrDefault(e => e.AgentUserId == agent.ID);

                if (assignment == null)
                {
                    assignment = new AgentAssignment();
                    context.AgentAssignments.Add(assignment);
                }

                assignment.AgentUserId = agent.ID;
                assignment.Name = agent.Name;
                assignment.Phone = agent.Phone ?? "";
                assignment.AgentWorkVillage = village;
                assignment.MonthlyDeliveredAmount = 0;

                context.SaveChanges();
                result.Data = true;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<bool> DeleteAgentAssignment(int agentId)
        {
            var result = new Result<bool>();

            try
            {
                var agent = context.Agents.Find(agentId);

                if (agent == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Agent Id.";
                    return result;
                }

                var assignments = context.AgentAssignments
                    .Where(e => e.AgentUserId == agentId)
                    .ToList();

                if (assignments.Any())
                {
                    context.AgentAssignments.RemoveRange(assignments);
                }

                agent.Area = "";

                context.SaveChanges();
                result.Data = true;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }
    }
}
