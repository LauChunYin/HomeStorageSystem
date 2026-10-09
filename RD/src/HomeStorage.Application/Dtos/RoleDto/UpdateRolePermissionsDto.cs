using System.ComponentModel.DataAnnotations;
using HomeStorage.Domain.Entities;

namespace HomeStorage.Application.Dtos;

public record UpdateRolePermissionsDto
{
    public List<int> PermissionIds;
}