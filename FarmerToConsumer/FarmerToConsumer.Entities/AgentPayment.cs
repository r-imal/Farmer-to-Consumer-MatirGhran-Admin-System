using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmerToConsumer.Entities
{
    [Table("AgentPayment")]
    public class AgentPayment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int AgentId { get; set; }
        public int Amount { get; set; }
        public DateTime PaidAt { get; set; } = DateTime.Now;

        [ForeignKey("AgentId")]
        public virtual UserInfo? Agent { get; set; }
    }
}
