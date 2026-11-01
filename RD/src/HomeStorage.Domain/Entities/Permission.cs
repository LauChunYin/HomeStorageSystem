namespace HomeStorage.Domain.Entities;

public class Permission
{
    public int Id {get; private set;}
    public string PermissionCode {get; private set;} = string.Empty;
    public string Description {get; private set;} = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    /// <summary>
    /// 工厂方法：创建一个新权限点（PermissionCode 需全局唯一，由数据库唯一索引兜底）
    /// </summary>
    public static Permission Create(string permissionCode, string description)
    {
        if (string.IsNullOrWhiteSpace(permissionCode))
            throw new ArgumentException("权限点编码不能为空", nameof(permissionCode));

        return new Permission
        {
            PermissionCode = permissionCode.Trim(),
            Description = description.Trim()
        };
    }
}