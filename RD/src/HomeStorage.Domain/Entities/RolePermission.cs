namespace HomeStorage.Domain.Entities;

public class RolePermission
{
    // 外键：角色 ID
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    // 外键：权限 ID
    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}