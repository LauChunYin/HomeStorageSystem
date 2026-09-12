using System.ComponentModel.DataAnnotations;
using HomeStorage.Domain.Entities;

namespace HomeStorage.Application.Dtos;

public class CreateItemDto
{
    [Required(ErrorMessage = "物品名称不能为空")]
    [MaxLength(100, ErrorMessage = "物品名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "分类不能为空")]
    [MaxLength(50, ErrorMessage = "分类名称长度不能超过50个字符")]
    public int CategoryId { get; set; }

    [MaxLength(100, ErrorMessage = "存放位置长度不能超过100个字符")]
    public int? LocationId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "库存数量不能为负数")]
    public int Quantity { get; set; }

    public string? ImageUrl { get; set; }
}