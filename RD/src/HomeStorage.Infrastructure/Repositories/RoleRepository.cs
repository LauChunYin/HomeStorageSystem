using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<Role?> GetRoleWithPermissionsAsync(int roleId)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public async Task<Role?> GetRoleWithCategoriesAsync(int roleId)
    {
        return await _context.Roles
            .Include(r => r.RoleCategories)
                .ThenInclude(rp => rp.Category)
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public async Task<Role?> GetRoleWithLocationsAsync(int roleId)
    {
        return await _context.Roles
            .Include(r => r.RoleLocations)
                .ThenInclude(rp => rp.Location)
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public void UpdateRolePermissions(Role role, List<int> permissionIds)
    {
        _context.RolePermissions.RemoveRange(role.RolePermissions);

        foreach (var permissionId in permissionIds.Distinct())
        {
            role.RolePermissions.Add(RolePermission.Create(role.Id, permissionId));
        }
    }

    public void UpdateRoleCategories(Role role, List<int> categoryIds)
    {
        _context.RoleCategories.RemoveRange(role.RoleCategories);

        foreach (var categoryId in categoryIds.Distinct())
        {
            role.RoleCategories.Add(RoleCategory.Create(role.Id, categoryId));
        }
    }

    public void UpdateRoleLocations(Role role, List<int> locationIds)
    {
        _context.RoleLocations.RemoveRange(role.RoleLocations);

        foreach (var locationId in locationIds.Distinct())
        {
            role.RoleLocations.Add(RoleLocation.Create(role.Id, locationId));
        }
    }

    public async Task<List<int>> GetAllowedCategoryIdsAsync(int roleId)
    {
        return await _context.RoleCategories
            .Where(rc => rc.RoleId == roleId)
            .Select(rc => rc.CategoryId)
            .ToListAsync();
    }

    public async Task<List<int>> GetAllowedLocationIdsAsync(int roleId)
    {
        return await _context.RoleLocations
            .Where(rl => rl.RoleId == roleId)
            .Select(rl => rl.LocationId)
            .ToListAsync();
    }

}