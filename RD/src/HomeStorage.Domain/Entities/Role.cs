namespace HomeStorage.Domain.Entities;

public class Role
{
    public int Id { get; private set; }
    public string RoleCode { get; private set; } = string.Empty;
    public string RoleName { get; private set; } = string.Empty;

    public ICollection<User> Users { get; private set; } = new List<User>();
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public ICollection<RoleCategory> RoleCategories { get; private set; } = new List<RoleCategory>();
    public ICollection<RoleLocation> RoleLocations { get; private set; } = new List<RoleLocation>();

    /// <summary>
    /// 工厂方法：创建一个新角色（RoleCode 需全局唯一，由数据库唯一索引兜底）
    /// </summary>
    public static Role Create(string roleCode, string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
            throw new ArgumentException("角色编码不能为空", nameof(roleCode));

        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("角色名称不能为空", nameof(roleName));

        return new Role
        {
            RoleCode = roleCode.Trim(),
            RoleName = roleName.Trim()
        };
    }
}