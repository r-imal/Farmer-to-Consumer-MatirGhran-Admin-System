using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmerToConsumer.Entities
{
    [Table("Farmer")]
    public class Farmer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int AgentId { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = "";

        [Required, StringLength(150)]
        public string Village { get; set; } = "";

        [Required, StringLength(150)]
        public string District { get; set; } = "";

        [Required, StringLength(11)]
        public string Contact { get; set; } = "";

        [ForeignKey("AgentId")]
        public virtual Agent? Agent { get; set; }
    }
}