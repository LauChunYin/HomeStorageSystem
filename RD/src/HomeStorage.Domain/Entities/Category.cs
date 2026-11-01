namespace HomeStorage.Domain.Entities;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public int? ParentId { get; private set; }
    public Category? Parent { get; private set; }

    // 容器：一个分类包含多个物品
    public ICollection<Category> Children { get; private set; } = new List<Category>();

    // 容器：一个分类包含多个物品
    public ICollection<Item> Items { get; private set; } = new List<Item>();

    public ICollection<RoleCategory> RoleCategories { get; private set; } = new List<RoleCategory>();

    private Category() { }

    public static Category Create(string name, int? parentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("品类名不能为空", nameof(name));

        if (parentId.HasValue && parentId <= 0)
            throw new ArgumentOutOfRangeException(nameof(parentId), "父品类 ID 必须大于 0");

        return new Category
        {
            Name = name.Trim(),
            ParentId = parentId,
        };
    }

    public void Update(string name, int? parentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("品类名不能为空", nameof(name));

        if (parentId.HasValue && parentId <= 0)
            throw new ArgumentOutOfRangeException(nameof(parentId), "父品类 ID 必须大于 0");

        Name = name.Trim();
        ParentId = parentId;
    }
}