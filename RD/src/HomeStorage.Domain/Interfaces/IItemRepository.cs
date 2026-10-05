using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface IItemRepository : IRepository<Item>
{
    Task<IEnumerable<Item>> GetItemsByCategoryAndLocationAsync(int categoryId, int locationId);
    Task<bool> SaveChangesAsync();
}