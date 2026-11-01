using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location?> GetLocationByNameAsync(string name);
    Task<IEnumerable<Location>> GetLocationsByParentIdAsync(int? parentId);

    /// <summary>判断位置下是否还有子位置或物品（删除前校验，只需 exists 查询，无需加载整张图）</summary>
    Task<bool> HasChildrenOrItemsAsync(int locationId);

    Task<IEnumerable<Location>> GetAllFilteredAsync(IReadOnlyList<int>? allowedIds);
}