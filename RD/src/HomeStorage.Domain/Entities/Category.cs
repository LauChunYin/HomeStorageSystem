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
}