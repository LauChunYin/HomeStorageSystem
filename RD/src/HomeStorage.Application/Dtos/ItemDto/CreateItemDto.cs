using System.ComponentModel.DataAnnotations;
using HomeStorage.Domain.Entities;

namespace HomeStorage.Application.Dtos;

public class CreateItemDto
{
    [Required(ErrorMessage = "物品名称不能为空")]
    [MaxLength(100, ErrorMessage = "物品名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "必须选择有效的分类")]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "必须选择有效的存放位置")]
    public int? LocationId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "库存数量不能为负数")]
    public int Quantity { get; set; }

    public string? ImageUrl { get; set; }
}