using System.ComponentModel.DataAnnotations;

namespace DotNet.Models
{
    public class Login
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign key to User table

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string PasswordHash { get; set; }

        public Student studentNav { get; set; } // Navigation property
    }
}
