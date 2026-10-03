namespace FarmerToConsumer.Shared.Models;

public class CustomerCartModel
{
    public List<CustomerCartItemModel> Items { get; set; } = new();
    public int Subtotal => Items.Sum(e => e.LineTotal);
    public int DeliveryCharge { get; set; } = 99;
    public int Total => Subtotal + (Items.Count == 0 ? 0 : DeliveryCharge);
}
