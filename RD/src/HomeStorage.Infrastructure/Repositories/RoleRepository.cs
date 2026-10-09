using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<Role?> GetRolePermissionsAsyns(int roleId)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public async Task<Role?> GetRoleCategoriesAsyns(int roleId)
    {
        return await _context.Roles
            .Include(r => r.RoleCategories)
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public async Task<Role?> GetRoleLocationsAsyns(int roleId)
    {
        return await _context.Roles
            .Include(r => r.RoleLocations)
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public void UpdateRolePermissionsAsyns(Role role, List<int> permissionIds)
    {
        _context.RolePermissions.RemoveRange(role.RolePermissions);

        foreach (var permissionId in permissionIds.Distinct())
        {
            role.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId
            });
        }
    }

    public void UpdateRoleCategoriessAsyns(Role role, List<int> categoryIds)
    {
        _context.RoleCategories.RemoveRange(role.RoleCategories);

        foreach (var categoryId in categoryIds.Distinct())
        {
            role.RoleCategories.Add(new RoleCategory
            {
                RoleId = role.Id,
                CategoryId = categoryId
            });
        }
    }

    public void UpdateRoleLocationsAsyns(Role role, List<int> locationIds)
    {
        _context.RoleLocations.RemoveRange(role.RoleLocations);

        foreach (var locationId in locationIds.Distinct())
        {
            role.RoleLocations.Add(new RoleLocation
            {
                RoleId = role.Id,
                LocationId = locationId
            });
        }
    }

}