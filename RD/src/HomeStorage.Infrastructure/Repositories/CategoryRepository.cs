using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    // 返回 DTO 需要 ParentName，因此按需 Include(Parent)；
    // 原先的 RoleCategories -> Role 预加载没有任何调用方读取，属冗余加载，已移除。
    public async Task<Category?> GetCategoryByNameAsync(string name)
    {
        return await _context.Categories
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<IEnumerable<Category>> GetCategoriesByParentIdAsync(int? parentId)
    {
        return await _context.Categories
            .Include(c => c.Parent)
            .Where(c => c.ParentId == parentId)
            .AsNoTracking()
            .ToListAsync();
    }

    // 删除前校验：用两个 exists 查询代替原先加载 Children + Items 整张图
    public async Task<bool> HasChildrenOrItemsAsync(int categoryId)
    {
        return await _context.Categories.AnyAsync(c => c.ParentId == categoryId)
            || await _context.Items.AnyAsync(i => i.CategoryId == categoryId);
    }

    public async Task<IEnumerable<Category>> GetAllFilteredAsync(IReadOnlyList<int>? allowedIds)
    {
        var query = _context.Categories
            .Include(c => c.Parent)
            .AsNoTracking()
            .AsQueryable();

        if (allowedIds != null)
            query = query.Where(c => allowedIds.Contains(c.Id));

        return await query.ToListAsync();
    }
}
