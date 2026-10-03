using FarmerToConsumer.Entities;

namespace FarmerToConsumer.Shared.Models;

public class CustomerCartItemModel
{
    public int StockId { get; set; }
    public int Quantity { get; set; }
    public ProductStock? Stock { get; set; }
    public int UnitPrice => Stock?.Price ?? 0;
    public int LineTotal => UnitPrice * Quantity;
}
