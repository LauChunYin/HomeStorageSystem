namespace HomeStorage.Domain.Entities;

/// <summary>
/// 物品领域实体（采用充血模型设计）
/// </summary>
public class Item
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public int? LocationId { get; private set; }
    public Location? Location { get; private set; }

    public int Quantity { get; private set; }

    public ItemStatus Status {get; private set;}

    public string? ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // EF Core 反射所必须的私有无参构造函数
    private Item() { }

    /// <summary>
    /// 工厂方法：用于安全创建新物品（保证创建时刻数据合法）
    /// </summary>
    public static Item Create(string name, int categoryId, int? locationId, string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("物品名称不能为空", nameof(name));

        if (categoryId <= 0)
            throw new ArgumentException("物品分类不能为空", nameof(categoryId));

        return new Item
        {
            Name = name.Trim(),
            CategoryId = categoryId,
            LocationId = locationId,
            Quantity = 1,
            Status = ItemStatus.InStock,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// 领域业务方法：修改物品基本信息
    /// </summary>
    public void UpdateInfo(string name, int categoryId, int? locationId, string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("物品名称不能为空", nameof(name));

        if (categoryId <= 0)
            throw new ArgumentException("物品分类不能为空", nameof(categoryId));

        Name = name.Trim();
        CategoryId = categoryId;
        LocationId = locationId;
        ImageUrl = imageUrl;

        UpdatedAt = DateTime.UtcNow;
    }

    public void StockIn(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("入库数量不能少于等于0", nameof(quantity));
            
        Quantity += quantity;
        Status = Quantity>0?ItemStatus.InStock:ItemStatus.OutStock;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StockOut(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("出库数量不能少于等于0", nameof(quantity));

        Quantity -= quantity;
        Status = Quantity>0?ItemStatus.InStock:ItemStatus.OutStock;
        UpdatedAt = DateTime.UtcNow;
    }
}