namespace HomeStorage.Application.Dtos;
using HomeStorage.Domain.Entities;

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? ParentName{ get; set; }
}