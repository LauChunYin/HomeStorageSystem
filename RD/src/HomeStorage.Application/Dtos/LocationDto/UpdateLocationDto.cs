using System.ComponentModel.DataAnnotations;

namespace HomeStorage.Application.Dtos;

public class UpdateLocationDto
{
    [Required(ErrorMessage = "位置名不能为空")]
    [MaxLength(100, ErrorMessage = "位置名不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "图片地址长度不能超过500个字符")]
    public string? ImageUrl { get; set; }

    public int? ParentId { get; set; }

}