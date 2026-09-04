namespace HomeStorage.Domain.Entities;

/// <summary>
/// 物品领域实体（采用充血模型设计）
/// </summary>
public class Item
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string? Location { get; private set; }
    public int Quantity { get; private set; }
    public string? ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // EF Core 反射所必须的私有无参构造函数
    private Item() { }

    /// <summary>
    /// 工厂方法：用于安全创建新物品（保证创建时刻数据合法）
    /// </summary>
    public static Item Create(string name, string category, string? location, int quantity, string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("物品名称不能为空", nameof(name));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("物品分类不能为空", nameof(category));

        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "库存数量不能为负数");

        return new Item
        {
            Name = name.Trim(),
            Category = category.Trim(),
            Location = location?.Trim(),
            Quantity = quantity,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// 领域业务方法：修改物品基本信息
    /// </summary>
    public void UpdateInfo(string name, string category, string? location, int quantity, string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("物品名称不能为空", nameof(name));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("物品分类不能为空", nameof(category));

        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "库存数量不能为负数");

        Name = name.Trim();
        Category = category.Trim();
        Location = location?.Trim();
        Quantity = quantity;

        if (!string.IsNullOrEmpty(imageUrl))
        {
            ImageUrl = imageUrl;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}