using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmerToConsumer.Entities
{
    [Table("Agent")]
    public class Agent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? UserInfoID { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = "";

        [Required, StringLength(50)]
        public string Phone { get; set; } = "";

        [Required, StringLength(150)]
        public string Area { get; set; } = "";

        public decimal CommissionRate { get; set; } = 5;

        [ForeignKey("UserInfoID")]
        public virtual UserInfo? UserInfo { get; set; }
    }
}