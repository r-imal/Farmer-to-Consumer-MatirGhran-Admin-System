using System.Text.Json;
using System.Text;
using FarmerToConsumer.Data;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Shared;
using FarmerToConsumer.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FarmerToConsumer.Repos;

public class CustomerShopRepo(FtcDbContext context, IHttpContextAccessor accessor)
{
    private const string CartSessionKey = "CustomerCart";
    private const int DeliveryCharge = 99;

    public Result<CustomerProductListModel> GetProducts(string? category, string? search)
    {
        var result = new Result<CustomerProductListModel> { Data = new CustomerProductListModel() };
        try
        {
            var query = context.ProductStocks
                .Include(e => e.Product)
                .ThenInclude(e => e!.Farmer)
                .Where(e => e.Quantity > 0 && e.Product != null);

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(e => e.Product!.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.Product!.Name.Contains(search) ||
                    e.Product.Category.Contains(search) ||
                    (e.Product.Farmer != null && e.Product.Farmer.Name.Contains(search)));
            }

            result.Data.Stocks = query
                .OrderBy(e => e.Product!.Name)
                .ToList();
            result.Data.Categories = context.Products
                .Select(e => e.Category)
                .Distinct()
                .OrderBy(e => e)
                .ToList();
            result.Data.Category = category;
            result.Data.Search = search;
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }

    public Result<ProductStock> GetStock(int stockId)
    {
        var result = new Result<ProductStock>();
        try
        {
            result.Data = context.ProductStocks
                .Include(e => e.Product)
                .ThenInclude(e => e!.Farmer)
                .FirstOrDefault(e => e.ID == stockId && e.Quantity > 0);

            if (result.Data == null)
            {
                result.HasError = true;
                result.Message = "Product is not available.";
            }
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }

    public Result<CustomerCartModel> GetCart()
    {
        var result = new Result<CustomerCartModel> { Data = new CustomerCartModel { DeliveryCharge = DeliveryCharge } };
        try
        {
            var cart = ReadCart();
            var stockIds = cart.Keys.ToList();
            var stocks = context.ProductStocks
                .Include(e => e.Product)
                .ThenInclude(e => e!.Farmer)
                .Where(e => stockIds.Contains(e.ID))
                .ToList();

            result.Data.Items = stocks.Select(e => new CustomerCartItemModel
                {
                    StockId = e.ID,
                    Stock = e,
                    Quantity = Math.Min(cart[e.ID], Math.Max(e.Quantity, 1))
                })
                .Where(e => e.Quantity > 0)
                .ToList();
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }

    public Result<bool> AddToCart(int stockId)
    {
        var result = new Result<bool>();
        try
        {
            var stock = context.ProductStocks.Find(stockId);
            if (stock == null || stock.Quantity <= 0)
            {
                result.HasError = true;
                result.Message = "Product is not available.";
                return result;
            }

            var cart = ReadCart();
            cart[stockId] = cart.TryGetValue(stockId, out var quantity)
                ? Math.Min(quantity + 1, stock.Quantity)
                : 1;
            SaveCart(cart);
            result.Data = true;
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }

    public Result<bool> UpdateCart(int stockId, int quantity)
    {
        var result = new Result<bool>();
        try
        {
            var cart = ReadCart();
            if (quantity <= 0)
            {
                cart.Remove(stockId);
            }
            else
            {
                var stockQuantity = context.ProductStocks
                    .Where(e => e.ID == stockId)
                    .Select(e => e.Quantity)
                    .FirstOrDefault();
                cart[stockId] = Math.Min(quantity, Math.Max(stockQuantity, 1));
            }

            SaveCart(cart);
            result.Data = true;
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }

    public Result<CustomerOrder> Checkout(int customerId, CustomerCheckoutModel model)
    {
        var result = new Result<CustomerOrder>();
        try
        {
            if (customerId <= 0)
            {
                result.HasError = true;
                result.Message = "Please login before checkout.";
                return result;
            }

            var cartResult = GetCart();
            if (cartResult.HasError || cartResult.Data == null || cartResult.Data.Items.Count == 0)
            {
                result.HasError = true;
                result.Message = "Cart is empty.";
                return result;
            }

            foreach (var item in cartResult.Data.Items)
            {
                if (item.Stock == null || item.Quantity > item.Stock.Quantity)
                {
                    result.HasError = true;
                    result.Message = "One or more products are out of stock.";
                    return result;
                }
            }

            var order = new CustomerOrder
            {
                CustomerId = customerId,
                AgentId = cartResult.Data.Items.Select(e => e.Stock?.Product?.AddedByAgent).FirstOrDefault(e => e > 0),
                DeliveryAddress = model.DeliveryAddress,
                Status = "Pending",
                DeliveryStatus = "Pending",
                TotalAmount = cartResult.Data.Total
            };
            context.CustomerOrders.Add(order);
            context.SaveChanges();

            foreach (var item in cartResult.Data.Items)
            {
                context.CustomerOrderItems.Add(new CustomerOrderItem
                {
                    OrderId = order.ID,
                    ProductId = item.Stock!.ProductId,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                });
                item.Stock.Quantity -= item.Quantity;
            }

            var isOnline = model.PaymentMethod == CustomerPayment.OnlinePayment;
            context.CustomerPayments.Add(new CustomerPayment
            {
                OrderId = order.ID,
                Amount = order.TotalAmount,
                Method = model.PaymentMethod,
                Status = isOnline ? "Paid" : "Unpaid"
            });
            context.SaveChanges();

            SaveCart(new Dictionary<int, int>());
            result.Data = order;
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }

    public Result<List<CustomerOrder>> GetOrders(int customerId)
    {
        var result = new Result<List<CustomerOrder>> { Data = new List<CustomerOrder>() };
        try
        {
            result.Data = context.CustomerOrders
                .Include(e => e.Payment)
                .Where(e => e.CustomerId == customerId)
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

    public Result<CustomerOrderDetailModel> GetOrderDetail(int customerId, int orderId)
    {
        var result = new Result<CustomerOrderDetailModel>();
        try
        {
            var order = context.CustomerOrders
                .Include(e => e.Items)
                .ThenInclude(e => e.Product)
                .Include(e => e.Payment)
                .Include(e => e.Agent)
                .FirstOrDefault(e => e.ID == orderId && e.CustomerId == customerId);

            if (order == null)
            {
                result.HasError = true;
                result.Message = "Invalid order id.";
                return result;
            }

            result.Data = new CustomerOrderDetailModel { Order = order };
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }

    public Result<bool> PayOrder(int customerId, int orderId)
    {
        var result = new Result<bool>();
        try
        {
            var order = context.CustomerOrders
                .Include(e => e.Payment)
                .FirstOrDefault(e => e.ID == orderId && e.CustomerId == customerId);

            if (order?.Payment == null)
            {
                result.HasError = true;
                result.Message = "Invalid order id.";
                return result;
            }

            order.Payment.Status = "Paid";
            order.Payment.Method = CustomerPayment.OnlinePayment;
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

    public Result<CustomerDashboardModel> GetDashboard(int customerId)
    {
        var result = new Result<CustomerDashboardModel> { Data = new CustomerDashboardModel() };
        try
        {
            var orders = context.CustomerOrders
                .Include(e => e.Payment)
                .Where(e => e.CustomerId == customerId)
                .OrderByDescending(e => e.ID)
                .ToList();

            result.Data.Customer = context.UserInfos.Find(customerId);
            result.Data.CartItemCount = ReadCart().Values.Sum();
            result.Data.PendingOrders = orders.Count(e => e.DeliveryStatus != "Delivered");
            result.Data.DeliveredOrders = orders.Count(e => e.DeliveryStatus == "Delivered");
            result.Data.RecentOrders = orders.Take(5).ToList();
            result.Data.FeaturedStocks = context.ProductStocks
                .Include(e => e.Product)
                .Where(e => e.Quantity > 0 && e.Product != null)
                .OrderBy(e => e.Product!.Name)
                .Take(6)
                .ToList();
        }
        catch (Exception e)
        {
            result.HasError = true;
            result.Message = e.Message;
        }

        return result;
    }

    private Dictionary<int, int> ReadCart()
    {
        var session = accessor.HttpContext?.Session;
        var json = session != null && session.TryGetValue(CartSessionKey, out var bytes)
            ? Encoding.UTF8.GetString(bytes)
            : null;
        return string.IsNullOrWhiteSpace(json)
            ? new Dictionary<int, int>()
            : JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
    }

    private void SaveCart(Dictionary<int, int> cart)
    {
        accessor.HttpContext?.Session.Set(CartSessionKey, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cart)));
    }
}
