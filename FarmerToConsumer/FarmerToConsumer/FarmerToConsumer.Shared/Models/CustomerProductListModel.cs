using FarmerToConsumer.Entities;

namespace FarmerToConsumer.Shared.Models;

public class CustomerProductListModel
{
    public List<ProductStock> Stocks { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public string? Category { get; set; }
    public string? Search { get; set; }
}
