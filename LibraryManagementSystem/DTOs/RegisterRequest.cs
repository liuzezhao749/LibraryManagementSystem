using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "姓拼音不能为空")]
        [RegularExpression(@"^[a-z]+$", ErrorMessage = "姓拼音必须是小写字母")]
        public string LastNamePinyin { get; set; } // 姓拼音（如 "liu"）

        [Required(ErrorMessage = "名拼音首字母不能为空")]
        [RegularExpression(@"^[a-z]+$", ErrorMessage = "名拼音首字母必须是小写字母")]
        public string FirstNameInitials { get; set; } // 名拼音首字母（如 "sx"）

        [Required(ErrorMessage = "8位数字后缀不能为空")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "必须为8位数字")]
        public string NumberSuffix { get; set; } // 新增：8位数字后缀（如 "12334444"）

        [Required(ErrorMessage = "真实姓名不能为空")]
        public string Name { get; set; } // 真实姓名（如 "刘少轩"）
    }
}