using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "书名是必填项")]
        [StringLength(100, ErrorMessage = "书名不能超过100个字符")]
        public string Title { get; set; }

        [Required(ErrorMessage = "作者是必填项")]
        [StringLength(50, ErrorMessage = "作者名不能超过50个字符")] // 添加长度限制
        public string Author { get; set; }

        [Required(ErrorMessage = "ISBN是必填项")]
        [RegularExpression(@"^\d{3}-\d{10}$", ErrorMessage = "ISBN格式应为XXX-XXXXXXXXXX")]
        public string ISBN { get; set; }

        [StringLength(100, ErrorMessage = "出版社名称不能超过100个字符")] // 添加长度限制
        public string? Publisher { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; } = new DateTime(2023, 1, 1); // 改为静态默认值

        [Required]
        public string Status { get; set; } = "可借";

        // 导航属性（初始化集合避免null引用）
        public List<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    }
}