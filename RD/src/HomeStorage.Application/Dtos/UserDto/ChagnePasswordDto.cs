using System.ComponentModel.DataAnnotations;

namespace HomeStorage.Application.Dtos;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "原密码不能为空")]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "新密码不能为空")]
    [MinLength(10, ErrorMessage = "新密码不能少于10个字符")]
    [MaxLength(100, ErrorMessage = "新密码不能超过100个字符")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{10,100}$", 
        ErrorMessage = "新密码必须包含大写字母、小写字母、数字和特殊字符"
    )]
    public string NewPassword { get; set; } = string.Empty;
}