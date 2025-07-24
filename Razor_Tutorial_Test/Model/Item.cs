using System.ComponentModel.DataAnnotations;

namespace Razor_Tutorial_Test.Model
{
    public class Items
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, 99999.99)]
        public decimal Price { get; set; }

        public int? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? Status { get; set; }

        [Required]
        public int Owner { get; set; }
    }
}
