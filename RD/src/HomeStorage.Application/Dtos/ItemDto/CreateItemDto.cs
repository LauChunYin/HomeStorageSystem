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

    // 允许无存放位置（null）：此类物料仅系统管理员可见，故类型必须可空
    [Range(1, int.MaxValue, ErrorMessage = "必须选择有效的存放位置")]
    public int? LocationId { get; set; }

    [MaxLength(500, ErrorMessage = "图片地址长度不能超过500个字符")]
    public string? ImageUrl { get; set; }
}