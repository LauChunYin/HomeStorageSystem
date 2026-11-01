using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetRoleWithPermissionsAsync(int roleId);
    Task<Role?> GetRoleWithCategoriesAsync(int roleId);
    Task<Role?> GetRoleWithLocationsAsync(int roleId);

    void UpdateRolePermissions(Role role, List<int> permissionIds);
    void UpdateRoleCategories(Role role, List<int> categoryIds);
    void UpdateRoleLocations(Role role, List<int> locationIds);

    Task<List<int>> GetAllowedCategoryIdsAsync(int roleId);
    Task<List<int>> GetAllowedLocationIdsAsync(int roleId);
}