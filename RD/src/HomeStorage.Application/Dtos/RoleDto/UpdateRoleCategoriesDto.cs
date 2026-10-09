using System.ComponentModel.DataAnnotations;

namespace HomeStorage.Application.Dtos;

public record UpdateRoleCategoriesDto
{
    public List<int> CategoryIds;
}