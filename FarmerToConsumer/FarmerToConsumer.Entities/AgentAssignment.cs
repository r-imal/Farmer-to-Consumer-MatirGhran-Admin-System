using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmerToConsumer.Entities
{
    [Table("AgentDeliverymanAssign")]
    public class AgentAssignment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required, StringLength(120)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string Phone { get; set; } = "";

        public int AgentUserId { get; set; }

        [Required, StringLength(120)]
        public string AgentWorkVillage { get; set; } = null!;

        public decimal MonthlyDeliveredAmount { get; set; }

        [ForeignKey("AgentUserId")]
        public virtual Agent? Agent { get; set; }
    }
}
