using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetRoleWithPermissionsAsyns(int roleId);
    Task<Role?> GetRoleWithCategoriesAsyns(int roleId);
    Task<Role?> GetRoleWithLocationsAsyns(int roleId);

    void UpdateRolePermissions(Role role, List<int> permissionIds);
    void UpdateRoleCategories(Role role, List<int> categoryIds);
    void UpdateRoleLocations(Role role, List<int> locationIds);
}