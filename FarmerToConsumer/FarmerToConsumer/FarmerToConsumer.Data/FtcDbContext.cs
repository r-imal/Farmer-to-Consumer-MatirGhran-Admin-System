using Microsoft.EntityFrameworkCore;
using FarmerToConsumer.Entities;

namespace FarmerToConsumer.Data
{
    public class FtcDbContext(DbContextOptions<FtcDbContext> options) : DbContext(options)
    {
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }
        public DbSet<AgentAssignment> AgentAssignments { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<CustomerOrderItem> CustomerOrderItems { get; set; }
        public DbSet<CustomerPayment> CustomerPayments { get; set; }
        public DbSet<AgentPayment> AgentPayments { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}