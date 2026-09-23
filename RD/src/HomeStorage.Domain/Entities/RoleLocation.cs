namespace HomeStorage.Domain.Entities;

public class RoleLocation
{
    // 外键：角色 ID
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    // 外键：位置 ID
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
}