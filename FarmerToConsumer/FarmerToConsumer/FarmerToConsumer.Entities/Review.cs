using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmerToConsumer.Entities
{
    [Table("Reviews")]
    public class Review
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Rating { get; set; }

        [Required, StringLength(500)]
        public string Comment { get; set; } = null!;
    }
}
