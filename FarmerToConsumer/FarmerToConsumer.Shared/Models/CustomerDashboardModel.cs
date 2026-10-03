using FarmerToConsumer.Entities;

namespace FarmerToConsumer.Shared.Models;

public class CustomerDashboardModel
{
    public UserInfo? Customer { get; set; }
    public int CartItemCount { get; set; }
    public int PendingOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public List<CustomerOrder> RecentOrders { get; set; } = new();
    public List<ProductStock> FeaturedStocks { get; set; } = new();
}
