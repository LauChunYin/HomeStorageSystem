using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface IItemRepository : IRepository<Item>
{
    Task<IEnumerable<Item>> SearchAsync(
    IReadOnlyList<int>? allowedCategoryIds,
    IReadOnlyList<int>? allowedLocationIds,
    int? categoryId = null,
    int? locationId = null);
}