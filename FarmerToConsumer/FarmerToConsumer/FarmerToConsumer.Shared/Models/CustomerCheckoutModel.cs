using System.ComponentModel.DataAnnotations;

namespace FarmerToConsumer.Shared.Models;

public class CustomerCheckoutModel
{
    [Required, StringLength(300)]
    public string DeliveryAddress { get; set; } = "";

    [Required]
    public string PaymentMethod { get; set; } = "OnlinePayment";
}
