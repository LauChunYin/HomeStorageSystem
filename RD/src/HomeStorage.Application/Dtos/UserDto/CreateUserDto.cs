using System.ComponentModel.DataAnnotations;

namespace HomeStorage.Application.Dtos;

public class CreateUserDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(100, ErrorMessage = "用户名不能超过100个字符")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(10, ErrorMessage = "密码不能少于10个字符")]
    [MaxLength(100, ErrorMessage = "密码不能超过100个字符")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{10,100}$", 
        ErrorMessage = "密码必须包含大写字母、小写字母、数字和特殊字符"
    )]
    public string Password { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    //外键
    [Range(1, int.MaxValue, ErrorMessage = "必须选择有效的角色")]
    public int RoleId { get; set; }
}