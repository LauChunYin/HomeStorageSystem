using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Item>> GetItemsByCategoryAndLocationAsync(int categoryId, int locationId)
    {
        return await _context.Items
            .Include(i => i.Category)
            .Include(i => i.Location)
            .Where(i => i.CategoryId == categoryId && i.LocationId == locationId)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<IEnumerable<Item>> GetAllAsync()
    {
        return await _context.Items
            .Include(i => i.Category)
            .Include(i => i.Location)
            .AsNoTracking()
            .ToListAsync();
    }

    // 重写查询单个，加上 Include
    public override async Task<Item?> GetByIdAsync(int id)
    {
        return await _context.Items
            .Include(i => i.Category)
            .Include(i => i.Location)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}