using System.ComponentModel.DataAnnotations;

namespace Razor_Tutorial_Test.Model
{
    public class LikeRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Item { get; set; } 

        [Required]
        public int User { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
