using HomeStorage.Domain.Entities;

namespace HomeStorage.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetCategoryByNameAsync(string name);
    Task<IEnumerable<Category>> GetCategoriesByParentIdAsync(int parentId);
    Task<bool> SaveChangesAsync();
}