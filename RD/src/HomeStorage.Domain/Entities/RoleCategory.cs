namespace HomeStorage.Domain.Entities;

/// <summary>角色与品类的数据范围关联</summary>
public class RoleCategory
{
    // 外键：角色 ID
    public int RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    // 外键：品类 ID
    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    // EF Core 反射所需的私有无参构造函数
    private RoleCategory() { }

    public static RoleCategory Create(int roleId, int categoryId)
    {
        if (roleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(roleId), "角色 ID 必须大于 0");

        if (categoryId <= 0)
            throw new ArgumentOutOfRangeException(nameof(categoryId), "品类 ID 必须大于 0");

        return new RoleCategory { RoleId = roleId, CategoryId = categoryId };
    }
}
