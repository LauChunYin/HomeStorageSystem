using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Repositories;

public class LocationRepository : Repository<Location>, ILocationRepository
{
    public LocationRepository(AppDbContext context) : base(context) { }
    
    
    public async Task<Location?> GetLocationByNameAsync(string name)
    {
        return await _context.Locations
        .Include(l=>l.RoleLocations)
            .ThenInclude(lt=>lt.RoleId)
            .FirstOrDefaultAsync(u=>u.Name == name);
    }

    public async Task<IEnumerable<Location>> GetLocationsByParentIdAsync(int parentId)
    {
        return await _context.Locations
        .Include(l=>l.RoleLocations)
            .ThenInclude(lt=>lt.RoleId)
        .Where(u=>u.ParentId == parentId)
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}