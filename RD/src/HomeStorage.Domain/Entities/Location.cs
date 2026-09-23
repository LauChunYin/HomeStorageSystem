namespace HomeStorage.Domain.Entities;

public class Location
{
    public int Id {get; private set;}
    public string Name {get; private set;} = string.Empty;

    public int? ParentId{get;set;}
    public Location? Parent{get;set;} 

    public string? ImageUrl {get; private set;}

    public ICollection<Item> Items { get; set; } = new List<Item>();

    public ICollection<Location> Children { get; set; } = new List<Location>();
    
    public ICollection<RoleLocation> RoleLocations { get; set; } = new List<RoleLocation>();


    private Location() { }

    public static Location Create(string name, int? parentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("品类名不能为空", nameof(name));

        if (parentId < 0)
            throw new ArgumentOutOfRangeException(nameof(parentId), "父品类 ID 必须大于等于 0");

        return new Location
        {
            Name = name.Trim(),
            ParentId = parentId,
        };
    }

    public void Update(string name, int? parentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("品类名不能为空", nameof(name));

        if (parentId < 0)
            throw new ArgumentOutOfRangeException(nameof(parentId), "父品类 ID 必须大于等于 0");

        Name = name;
        ParentId = parentId;
    }
}