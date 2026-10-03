using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmerToConsumer.Entities
{
    [Table("CustomerPayment")]
    public class CustomerPayment
    {
        public const string CashOnDelivery = "CashOnDelivery";
        public const string OnlinePayment = "OnlinePayment";

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int OrderId { get; set; }
        public int Amount { get; set; }

        [Required, StringLength(50)]
        public string Method { get; set; } = CashOnDelivery;

        [Required, StringLength(50)]
        public string Status { get; set; } = "Unpaid";

        [ForeignKey("OrderId")]
        public virtual CustomerOrder? Order { get; set; }
    }
}
