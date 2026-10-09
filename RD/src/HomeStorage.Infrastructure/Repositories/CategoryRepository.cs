using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }
    
    
    public async Task<Category?> GetCategoryByNameAsync(string name)
    {
        return await _context.Categories
        .Include(c=>c.RoleCategories)
            .ThenInclude(cg=>cg.Role)
        .FirstOrDefaultAsync(c=>c.Name == name);
    }

    public async Task<IEnumerable<Category>> GetCategoriesByParentIdAsync(int? parentId)
    {
        return await _context.Categories
        .Include(c=>c.RoleCategories)
            .ThenInclude(cg=>cg.Role)
        .Where(c=>c.ParentId == parentId)
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<Category?> GetCategoryWithDetailsAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Children)
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}