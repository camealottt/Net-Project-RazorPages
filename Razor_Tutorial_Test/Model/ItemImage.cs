using System.ComponentModel.DataAnnotations;

namespace Razor_Tutorial_Test.Model
{
    public class ItemImages
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public int ItemId { get; set; } 
    }
}
