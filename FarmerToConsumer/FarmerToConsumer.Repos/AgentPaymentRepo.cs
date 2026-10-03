using FarmerToConsumer.Data;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Shared;
using Microsoft.EntityFrameworkCore;

namespace FarmerToConsumer.Repos;

public class AgentPaymentRepo(FtcDbContext context)
{
    public Result<bool> PayAgentSalary(int agentUserId, int amount)
    {
        var result = new Result<bool>();
        try
        {
            if (amount <= 0)
            {
                result.HasError = true;
                result.Message = "No payable amount found for this agent.";
                return result;
            }

            context.AgentPayments.Add(new AgentPayment
            {
                AgentId = agentUserId,
                Amount = amount
            });
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

    public Result<List<AgentPaymentHistoryItem>> GetAgentPaymentHistory()
    {
        var result = new Result<List<AgentPaymentHistoryItem>> { Data = new List<AgentPaymentHistoryItem>() };
        try
        {
            result.Data = context.AgentPayments
                .Include(e => e.Agent)
                .OrderByDescending(e => e.PaidAt)
                .Select(e => new AgentPaymentHistoryItem
                {
                    AgentId = e.AgentId,
                    AgentName = e.Agent != null ? e.Agent.Name : "Unknown Agent",
                    Amount = e.Amount,
                    PaidAt = e.PaidAt,
                    From = "MatirGhran Company",
                    To = e.Agent != null ? e.Agent.Name : "Unknown Agent"
                })
                .ToList();
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }
}

public class AgentPaymentHistoryItem
{
    public int AgentId { get; set; }
    public string AgentName { get; set; } = "";
    public int Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public string From { get; set; } = "";
    public string To { get; set; } = "";
}
