using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<string>> GetDistinctLocationsAsync()
    {
        return await _dbSet
            .Where(i => i.Location != null)
            .Select(i => i.Location!.Name)
            .Distinct()
            .ToListAsync();
    }

    public override async Task<IEnumerable<Item>> GetAllAsync()
    {
        return await _dbSet
            .Include(u => u.Category)
            .Include(u => u.Location)
            .AsNoTracking()
            .ToListAsync();
    }

    // 重写查询单个，加上 Include
    public override async Task<Item?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(u => u.Category)
            .Include(u => u.Location)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}