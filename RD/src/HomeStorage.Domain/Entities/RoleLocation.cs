namespace HomeStorage.Domain.Entities;

/// <summary>角色与位置的数据范围关联</summary>
public class RoleLocation
{
    // 外键：角色 ID
    public int RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    // 外键：位置 ID
    public int LocationId { get; private set; }
    public Location Location { get; private set; } = null!;

    // EF Core 反射所需的私有无参构造函数
    private RoleLocation() { }

    public static RoleLocation Create(int roleId, int locationId)
    {
        if (roleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(roleId), "角色 ID 必须大于 0");

        if (locationId <= 0)
            throw new ArgumentOutOfRangeException(nameof(locationId), "位置 ID 必须大于 0");

        return new RoleLocation { RoleId = roleId, LocationId = locationId };
    }
}
