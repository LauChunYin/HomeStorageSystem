using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetCategoryByNameAsync(string name);
    Task<IEnumerable<Category>> GetCategoriesByParentIdAsync(int? parentId);

    /// <summary>判断品类下是否还有子品类或物品（删除前校验，只需 exists 查询，无需加载整张图）</summary>
    Task<bool> HasChildrenOrItemsAsync(int categoryId);

    Task<IEnumerable<Category>> GetAllFilteredAsync(IReadOnlyList<int>? allowedIds);
}