using System.ComponentModel.DataAnnotations;
using HomeStorage.Domain.Entities;

namespace HomeStorage.Application.Dtos;

public class UpdateItemDto
{
    [Required(ErrorMessage = "物品名称不能为空")]
    [MaxLength(100, ErrorMessage = "物品名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "分类不能为空")]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "位置不能为空")]
    public int? LocationId { get; set; }

    public string? ImageUrl { get; set; }
}