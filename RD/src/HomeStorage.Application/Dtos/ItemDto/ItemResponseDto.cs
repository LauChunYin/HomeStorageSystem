using HomeStorage.Domain.Entities;

namespace HomeStorage.Application.Dtos;

public class ItemResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int CategoryId {get;set;}
    public string CategoryName { get; set; } = string.Empty;

    public int? LocationId {get;set;}
    public string? LocationName { get; set; }

    public string? ImageUrl { get; set; }
    
    public int Quantity { get; set; }
    public ItemStatus Status{get;set;} = ItemStatus.InStock;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}