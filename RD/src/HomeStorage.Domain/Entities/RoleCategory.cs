namespace HomeStorage.Domain.Entities;

public class RoleCategory
{
    // 外键：角色 ID
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    // 外键：品类 ID
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}