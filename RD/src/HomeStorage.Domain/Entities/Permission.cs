namespace HomeStorage.Domain.Entities;

public class Permission
{
    public int Id {get; private set;}
    public string PermissionCode {get; private set;} = string.Empty;
    public string Description {get; private set;} = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}