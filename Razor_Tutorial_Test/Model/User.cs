using System.ComponentModel.DataAnnotations;

namespace Razor_Tutorial_Test.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Password field is required")]
        public string PasswordHash { get; set; }

        public string? ProfilePicture { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? Bio { get; set; }
    }
}
