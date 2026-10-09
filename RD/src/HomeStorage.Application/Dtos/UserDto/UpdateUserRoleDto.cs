using System.ComponentModel.DataAnnotations;

namespace HomeStorage.Application.Dtos;

public class UpdateUserRoleDto
{
    //外键
    [Range(1, int.MaxValue, ErrorMessage = "必须选择有效的角色")]
    public int RoleId { get; set; }
}