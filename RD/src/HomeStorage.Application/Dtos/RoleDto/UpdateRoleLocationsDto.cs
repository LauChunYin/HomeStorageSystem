using System.ComponentModel.DataAnnotations;

namespace HomeStorage.Application.Dtos;

public record UpdateRoleLocationsDto
{
    public List<int> LocationIds { get; set; } = new();
}