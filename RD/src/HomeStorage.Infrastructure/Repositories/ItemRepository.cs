using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeStorage.Infrastructure.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(AppDbContext context) : base(context) { }

    // 重写查询单个，加上 Include（DTO 需要 CategoryName / LocationName）
    public override async Task<Item?> GetByIdAsync(int id)
    {
        return await _context.Items
            .Include(i => i.Category)
            .Include(i => i.Location)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<Item>> SearchAsync(
    IReadOnlyList<int>? allowedCategoryIds,
    IReadOnlyList<int>? allowedLocationIds,
    int? categoryId = null,
    int? locationId = null)
    {
        var query = _context.Items
            .Include(i => i.Category)
            .Include(i => i.Location)
            .AsNoTracking()
            .AsQueryable();

        // 数据范围过滤（null 表示"不限制"，通常用于 Admin）
        if (allowedCategoryIds != null)
            query = query.Where(i => allowedCategoryIds.Contains(i.CategoryId));

        if (allowedLocationIds != null)
            query = query.Where(i => i.LocationId != null
                                  && allowedLocationIds.Contains(i.LocationId.Value));

        // 用户主动筛选
        if (categoryId.HasValue)
            query = query.Where(i => i.CategoryId == categoryId.Value);

        if (locationId.HasValue)
            query = query.Where(i => i.LocationId == locationId.Value);

        return await query.ToListAsync();
    }
}