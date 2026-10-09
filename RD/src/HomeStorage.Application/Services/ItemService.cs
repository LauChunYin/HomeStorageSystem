using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;

namespace HomeStorage.Application.Services;

public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public ItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // 1. 获取所有物品
    public async Task<IEnumerable<ItemResponseDto>> GetAllItemsAsync()
    {
        var items = await _unitOfWork.Items.GetAllAsync();
        // 将数据库里的 Item 实体列表逐个转换为给前端看的 ItemResponseDto
        return items.Select(MapToResponseDto);
    }

    // 2. 根据 Id 获取物品
    public async Task<ItemResponseDto?> GetItemByIdAsync(int id)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);
        return item == null ? null : MapToResponseDto(item);
    }

    // 3. 获取所有存放位置
    public async Task<IEnumerable<ItemResponseDto>> GetItemsByCategoryAndLocationAsync(int categoryId, int locationId)
    {
        var items = await _unitOfWork.Items.GetItemsByCategoryAndLocationAsync(categoryId, locationId);
        return items.Select(MapToResponseDto);
    }

    // 4. 创建新物品
    public async Task<ItemResponseDto> CreateItemAsync(CreateItemDto dto)
    {
        // ① 调用领域模型 Item 的静态工厂方法校验并生成实体
        var item = Item.Create(dto.Name, dto.CategoryId, dto.LocationId, dto.ImageUrl);

        // ② 扔给仓储管家放入内存追踪
        await _unitOfWork.Items.AddAsync(item);

        // ③ 真正提交落盘到 PostgreSQL 数据库（此时 item 会获得自动生成的 Id）
        await _unitOfWork.SaveChangesAsync();

        // ④ 转换为 ResponseDto 返回给外界
        return MapToResponseDto(item);
    }

    // 5. 更新物品
    public async Task<ItemResponseDto?> UpdateItemAsync(int id, UpdateItemDto dto)
    {
        // ① 先从数据库查出已有的物品实体
        var item = await _unitOfWork.Items.GetByIdAsync(id);
        if (item == null) return null; // 查不到直接返回 null

        // ② 调用实体对象的实例方法 UpdateInfo 修改内部数据
        item.UpdateInfo(dto.Name, dto.CategoryId, dto.LocationId, dto.ImageUrl);

        // ③ 标记为 Update 状态
        _unitOfWork.Items.Update(item);

        // ④ 提交保存到 PostgreSQL
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(item);
    }

    public async Task<ItemResponseDto?> StockInAsync(int id, MoveItemDto dto)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);
        if (item == null) return null; // 查不到直接返回 null

        item.StockIn(dto.Quantity);

        _unitOfWork.Items.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(item);
    }

    public async Task<ItemResponseDto?> StockOutAsync(int id, MoveItemDto dto)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);
        if (item == null) return null; // 查不到直接返回 null

        item.StockOut(dto.Quantity);

        _unitOfWork.Items.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(item);
    }

    // 6. 删除物品
    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);
        if (item == null) return false;

        _unitOfWork.Items.Delete(item);
        return await _unitOfWork.SaveChangesAsync();
    }

    // 私有辅助方法：负责将 Item 领域对象映射翻译成 ItemResponseDto
    private static ItemResponseDto MapToResponseDto(Item item)
    {
        return new ItemResponseDto
        {
            Id = item.Id,
            Name = item.Name,
            CategoryId = item.CategoryId,
            CategoryName = item.Category?.Name ?? string.Empty,
            LocationId = item.LocationId,
            LocationName = item.Location?.Name,
            Quantity = item.Quantity,
            ImageUrl = item.ImageUrl,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}