using FarmerToConsumer.Data;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Shared;
using Microsoft.EntityFrameworkCore;

namespace FarmerToConsumer.Repos
{
    public class OrderRepo(FtcDbContext context)
    {
        public Result<List<CustomerOrder>> GetAll()
        {
            var result = new Result<List<CustomerOrder>>()
            {
                Data = new List<CustomerOrder>()
            };

            try
            {
                result.Data = context.CustomerOrders
                    .Include(o => o.Items)
                    .Include(o => o.Payment)
                    .Include(o => o.Agent)
                    .OrderByDescending(o => o.ID)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<List<CustomerOrder>> GetAllByAgentId(int agentId)
        {
            var result = new Result<List<CustomerOrder>>()
            {
                Data = new List<CustomerOrder>()
            };

            try
            {
                result.Data = context.CustomerOrders
                    .Include(o => o.Items)
                    .Include(o => o.Payment)
                    .Include(o => o.Agent)
                    .Where(o => o.AgentId == agentId)
                    .OrderByDescending(o => o.ID)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<bool> UpdateStatus(int orderId, string status, int? agentId = null)
        {
            var result = new Result<bool>();

            try
            {
                CustomerOrder? objToUpdate;

                if (agentId.HasValue)
                {
                    objToUpdate = context.CustomerOrders
                        .FirstOrDefault(o => o.ID == orderId && o.AgentId == agentId.Value);
                }
                else
                {
                    objToUpdate = context.CustomerOrders.Find(orderId);
                }

                if (objToUpdate == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Order Id.";
                    return result;
                }

                objToUpdate.Status = status;
                objToUpdate.DeliveryStatus = status;

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