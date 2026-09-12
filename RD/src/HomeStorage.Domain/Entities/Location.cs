namespace HomeStorage.Domain.Entities;

public class Location
{
    public int Id {get; private set;}
    public string Name {get; private set;}
    public string? ImageUrl {get; private set;}

    public ICollection<Item> Items { get; set; } = new List<Item>();
}