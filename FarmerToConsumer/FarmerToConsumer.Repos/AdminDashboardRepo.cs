using FarmerToConsumer.Data;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Shared;
using Microsoft.EntityFrameworkCore;

namespace FarmerToConsumer.Repos;

public class AdminDashboardRepo(FtcDbContext context)
{
    public Result<List<UserInfo>> GetUsers()
    {
        var result = new Result<List<UserInfo>> { Data = new List<UserInfo>() };
        try
        {
            result.Data = context.UserInfos.OrderBy(e => e.Name).ToList();
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }

    public Result<List<CustomerOrder>> GetOrders()
    {
        var result = new Result<List<CustomerOrder>> { Data = new List<CustomerOrder>() };
        try
        {
            result.Data = context.CustomerOrders
                .Include(e => e.Items)
                .Include(e => e.Payment)
                .OrderByDescending(e => e.ID)
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
