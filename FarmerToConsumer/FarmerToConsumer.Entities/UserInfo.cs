using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerToConsumer.Entities
{
    [Table("Users")]
    public class UserInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [StringLength(250)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(250)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Column("PasswordHash")]
        public string Password { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Phone { get; set; } = "";

        [StringLength(120)]
        public string? Village { get; set; }

        [StringLength(120)]
        public string? District { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
