using System.ComponentModel.DataAnnotations;

namespace HomeStorage.Application.Dtos;

public class UpdateUserInfoDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(100, ErrorMessage = "用户名不能超过100个字符")]
    public string UserName { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    //外键
    [Range(1, int.MaxValue, ErrorMessage = "必须选择有效的角色")]
    public int RoleId { get; set; }
}