using System.ComponentModel.DataAnnotations;

namespace HomeStorage.Application.Dtos;

public class LoginDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}