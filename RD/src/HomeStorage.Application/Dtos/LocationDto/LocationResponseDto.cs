namespace HomeStorage.Application.Dtos;
using HomeStorage.Domain.Entities;

public class LocationResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    public int? ParentId { get; set; }
    public string? ParentName{ get; set; }
}