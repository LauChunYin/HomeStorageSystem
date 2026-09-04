using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface IItemRepository : IRepository<Item>
{
    Task<IEnumerable<string>> GetDistinctLocationsAsync();
    Task<bool> SaveChangesAsync();
}