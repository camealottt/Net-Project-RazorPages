using System;
using System.ComponentModel.DataAnnotations;

namespace Razor_Tutorial_Test.Model
{
    public class FollowRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AUser { get; set; }

        [Required]
        public int BUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
