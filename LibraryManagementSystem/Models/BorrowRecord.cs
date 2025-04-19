using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(14); // 默认借阅14天

        public DateTime? ReturnDate { get; set; } // 可为空，表示未归还

        // 导航属性
        public Book Book { get; set; }
        public User User { get; set; }
    }
}