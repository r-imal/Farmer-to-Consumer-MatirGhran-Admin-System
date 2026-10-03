using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmerToConsumer.Entities
{
    [Table("Product")]
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int FarmerId { get; set; }

        public int AddedByAgent { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = "";

        [Required, StringLength(150)]
        public string Category { get; set; } = "";

        [ForeignKey("FarmerId")]
        public virtual Farmer? Farmer { get; set; }

        [ForeignKey("AddedByAgent")]
        public virtual Agent? Agent { get; set; }
    }
}