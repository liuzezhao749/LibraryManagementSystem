using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        public string Role { get; set; } = "Reader";

        public string? PasswordHash { get; set; }
        public string? MagicLinkToken { get; set; }
        public DateTime? TokenExpiry { get; set; }

        public List<BorrowRecord> BorrowedBooks { get; set; } = new();
    }
}