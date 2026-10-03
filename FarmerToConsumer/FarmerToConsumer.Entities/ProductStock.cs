using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmerToConsumer.Entities
{
    [Table("ProductStock")]
    public class ProductStock
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int ProductId { get; set; }

        [NotMapped]
        [Display(Name = "Product")]
        public int ProductID
        {
            get => ProductId;
            set => ProductId = value;
        }

        public int Price { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
