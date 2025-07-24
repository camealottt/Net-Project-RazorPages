using System.ComponentModel.DataAnnotations;

namespace Razor_Tutorial_Test.Model
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

    }
}