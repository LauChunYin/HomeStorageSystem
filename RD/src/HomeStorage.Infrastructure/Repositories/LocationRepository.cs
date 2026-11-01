using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Repositories;

public class LocationRepository : Repository<Location>, ILocationRepository
{
    public LocationRepository(AppDbContext context) : base(context) { }

    // 返回 DTO 需要 ParentName，因此按需 Include(Parent)；
    // 原先的 RoleLocations -> Role 预加载没有任何调用方读取，属冗余加载，已移除。
    public async Task<Location?> GetLocationByNameAsync(string name)
    {
        return await _context.Locations
            .Include(l => l.Parent)
            .FirstOrDefaultAsync(l => l.Name == name);
    }

    public async Task<IEnumerable<Location>> GetLocationsByParentIdAsync(int? parentId)
    {
        return await _context.Locations
            .Include(l => l.Parent)
            .Where(l => l.ParentId == parentId)
            .AsNoTracking()
            .ToListAsync();
    }

    // 删除前校验：用两个 exists 查询代替原先加载 Children + Items 整张图
    public async Task<bool> HasChildrenOrItemsAsync(int locationId)
    {
        return await _context.Locations.AnyAsync(l => l.ParentId == locationId)
            || await _context.Items.AnyAsync(i => i.LocationId == locationId);
    }

    public async Task<IEnumerable<Location>> GetAllFilteredAsync(IReadOnlyList<int>? allowedIds)
    {
        var query = _context.Locations
            .Include(l => l.Parent)
            .AsNoTracking()
            .AsQueryable();

        if (allowedIds != null)
            query = query.Where(l => allowedIds.Contains(l.Id));

        return await query.ToListAsync();
    }
}
