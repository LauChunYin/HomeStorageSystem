namespace HomeStorage.Domain.Entities;

/// <summary>角色与功能权限的关联</summary>
public class RolePermission
{
    // 外键：角色 ID
    public int RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    // 外键：权限 ID
    public int PermissionId { get; private set; }
    public Permission Permission { get; private set; } = null!;

    // EF Core 反射所需的私有无参构造函数
    private RolePermission() { }

    public static RolePermission Create(int roleId, int permissionId)
    {
        if (roleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(roleId), "角色 ID 必须大于 0");

        if (permissionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(permissionId), "权限 ID 必须大于 0");

        return new RolePermission { RoleId = roleId, PermissionId = permissionId };
    }
}
