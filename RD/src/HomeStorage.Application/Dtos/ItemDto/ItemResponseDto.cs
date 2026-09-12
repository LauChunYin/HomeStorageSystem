using HomeStorage.Domain.Entities;

namespace HomeStorage.Application.Dtos;

public class ItemResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int CategoryId {get;set;}
    public Category Category { get; set; } = null;
    public int LocationId {get;set;}
    public Location? Location { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}