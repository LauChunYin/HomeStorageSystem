using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Exceptions;
using HomeStorage.Domain.Interfaces;

namespace HomeStorage.Application.Services;

public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserScopeProvider _scopeProvider;

    public ItemService(IUnitOfWork unitOfWork, IUserScopeProvider scopeProvider)
    {
        _unitOfWork = unitOfWork;
        _scopeProvider = scopeProvider;
    }

    // 1. 获取所有物品
    public async Task<IEnumerable<ItemResponseDto>> GetAllItemsAsync()
    {
        var scope = await _scopeProvider.GetScopeAsync();

        var items = await _unitOfWork.Items.SearchAsync(
            scope.IsAdmin ? null : scope.CategoryIds,
            scope.IsAdmin ? null : scope.LocationIds);

        // 将数据库里的 Item 实体列表逐个转换为给前端看的 ItemResponseDto
        return items.Select(MapToResponseDto);
    }

    // 2. 根据 Id 获取物品
    public async Task<ItemResponseDto?> GetItemByIdAsync(int id)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);
        if (item == null) return null;

        var scope = await _scopeProvider.GetScopeAsync();
        if (!scope.IsAdmin)
        {
            if (!scope.CategoryIds.Contains(item.CategoryId)) return null;
            if (item.LocationId.HasValue && !scope.LocationIds.Contains(item.LocationId.Value)) return null;
        }

        return item == null ? null : MapToResponseDto(item);
    }

    // 3. 获取所有存放位置
    public async Task<IEnumerable<ItemResponseDto>> GetItemsByCategoryAndLocationAsync(int categoryId, int locationId)
    {
        var scope = await _scopeProvider.GetScopeAsync();

        if (!scope.IsAdmin)
        {
            if (!scope.CategoryIds.Contains(categoryId)) return Array.Empty<ItemResponseDto>();
            if (!scope.LocationIds.Contains(locationId)) return Array.Empty<ItemResponseDto>();
        }

        var items = await _unitOfWork.Items.SearchAsync(
            scope.IsAdmin ? null : scope.CategoryIds,
            scope.IsAdmin ? null : scope.LocationIds,
            categoryId,
            locationId);

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
        var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId)
            ?? throw new BusinessException("分类不存在");

        //先从数据库查出已有的物品实体
        var item = await _unitOfWork.Items.GetByIdAsync(id);
        if (item == null) return null; // 查不到直接返回 null

        // 写操作双重校验：既要能访问「物品当前所在范围」，也要能访问「要写入的目标范围」
        await EnsureItemInScopeAsync(item);
        await EnsureTargetInScopeAsync(dto.CategoryId, dto.LocationId);

        //调用实体对象的实例方法 UpdateInfo 修改内部数据
        item.UpdateInfo(dto.Name, dto.CategoryId, dto.LocationId, dto.ImageUrl);

        //标记为 Update 状态
        _unitOfWork.Items.Update(item);

        //提交保存到 PostgreSQL
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(item);
    }

    public async Task<ItemResponseDto?> StockInAsync(int id, MoveItemDto dto)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);
        if (item == null) return null; // 查不到直接返回 null

        await EnsureItemInScopeAsync(item);
        item.StockIn(dto.MoveQuantity);

        _unitOfWork.Items.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(item);
    }

    public async Task<ItemResponseDto?> StockOutAsync(int id, MoveItemDto dto)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);
        if (item == null) return null; // 查不到直接返回 null

        await EnsureItemInScopeAsync(item);
        item.StockOut(dto.MoveQuantity);

        _unitOfWork.Items.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(item);
    }

    // 6. 删除物品
    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);
        if (item == null) return false;

        await EnsureItemInScopeAsync(item);

        _unitOfWork.Items.Delete(item);
        return await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// 校验「该物料当前所在的数据范围」是否可被调用者操作，越权则抛 403。
    /// 规则与查询侧保持一致：品类必须在授权内，位置必须在授权内，无位置物料仅管理员可操作。
    /// </summary>
    private async Task EnsureItemInScopeAsync(Item item)
    {
        var scope = await _scopeProvider.GetScopeAsync();
        if (scope.IsAdmin) return;

        if (!scope.CategoryIds.Contains(item.CategoryId) || !IsLocationInScope(item.LocationId, scope))
            throw new UnauthorizedAccessException("无权操作该物料");
    }

    /// <summary>
    /// 校验「要写入的目标品类/位置」是否在调用者授权范围内，
    /// 防止把物料改动到自己看不到的品类或位置（等价于越权写入）。
    /// </summary>
    private async Task EnsureTargetInScopeAsync(int categoryId, int? locationId)
    {
        var scope = await _scopeProvider.GetScopeAsync();
        if (scope.IsAdmin) return;

        if (!scope.CategoryIds.Contains(categoryId) || !IsLocationInScope(locationId, scope))
            throw new UnauthorizedAccessException("无权将物料写入该品类或位置");
    }

    /// <summary>无位置（null）对非管理员一律视为不可访问。</summary>
    private static bool IsLocationInScope(int? locationId, UserScope scope)
        => locationId.HasValue && scope.LocationIds.Contains(locationId.Value);

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
            Status = item.Status,
            ImageUrl = item.ImageUrl,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}