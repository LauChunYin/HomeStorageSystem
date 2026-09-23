using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location?> GetLocationByNameAsync(string name);
    Task<IEnumerable<Location>> GetLocationsByParentIdAsync(int parentId);
    Task<bool> SaveChangesAsync();
}