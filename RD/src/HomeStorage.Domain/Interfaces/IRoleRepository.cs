using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetRolePermissionsAsyns(int roleId);
    Task<Role?> GetRoleCategoriesAsyns(int roleId);
    Task<Role?> GetRoleLocationsAsyns(int roleId);

    void UpdateRolePermissionsAsyns(Role role, List<int> permissionIds);
    void UpdateRoleCategoriesAsyns(Role role, List<int> categoryIds);
    void UpdateRoleLocationsAsyns(Role role, List<int> locationIds);
}