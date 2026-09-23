namespace HomeStorage.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int? ParentId{get;set;}
    public Category? Parent{get;set;}

    // 容器：一个分类包含多个物品
    public ICollection<Category> Children { get; set; } = new List<Category>();

    // 容器：一个分类包含多个物品
    public ICollection<Item> Items { get; set; } = new List<Item>();

    public ICollection<RoleCategory> RoleCategories { get; set; } = new List<RoleCategory>();

    private Category() { }

    public static Category Create(string name, int? parentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("品类名不能为空", nameof(name));

        if (parentId < 0)
            throw new ArgumentOutOfRangeException(nameof(parentId), "父品类 ID 必须大于等于 0");

        return new Category
        {
            Name = name.Trim(),
            ParentId = parentId,
        };
    }

    public void Update(string name, int parentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("品类名不能为空", nameof(name));

        if (parentId < 0)
            throw new ArgumentOutOfRangeException(nameof(parentId), "父品类 ID 必须大于等于 0");

        Name = name;
        ParentId = parentId;
    }
}