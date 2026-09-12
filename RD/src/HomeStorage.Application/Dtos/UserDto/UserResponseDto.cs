namespace HomeStorage.Application.Dtos;
using HomeStorage.Domain.Entities;

public class UserResponseDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;

    public List<string> Permissions{ get;set; }= new();
}