using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmerToConsumer.Entities
{
    [Table("CustomerOrder")]
    public class CustomerOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int CustomerId { get; set; }
        public int? AgentId { get; set; }

        public int TotalAmount { get; set; }

        [Required, StringLength(50)]
        public string Status { get; set; } = "Pending";

        [Required, StringLength(300)]
        public string DeliveryAddress { get; set; } = null!;

        [Required, StringLength(50)]
        public string DeliveryStatus { get; set; } = "Pending";

        [ForeignKey("CustomerId")]
        public virtual UserInfo? Customer { get; set; }

        [ForeignKey("AgentId")]
        public virtual Agent? Agent { get; set; }

        public virtual List<CustomerOrderItem> Items { get; set; } = new();
        public virtual CustomerPayment? Payment { get; set; }
    }
}
