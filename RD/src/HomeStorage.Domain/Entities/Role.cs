namespace HomeStorage.Domain.Entities;

public class Role
{
    public int Id { get; private set; }
    public string RoleCode { get; private set; } = string.Empty;
    public string RoleName { get; private set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}