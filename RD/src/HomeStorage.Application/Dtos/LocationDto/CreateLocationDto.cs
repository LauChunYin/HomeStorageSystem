using System.ComponentModel.DataAnnotations;

namespace HomeStorage.Application.Dtos;

public class CreateLocationDto
{
    [Required(ErrorMessage = "位置名不能为空")]
    [MaxLength(20, ErrorMessage = "位置名不能超过20个字符")]
    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int? ParentId { get; set; }
}