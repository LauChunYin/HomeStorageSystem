using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Domain.Exceptions;

namespace HomeStorage.Application.Services;

public class LocationService : ILocationService
{
    // 注入我们第三步写好的物品仓储管家
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserScopeProvider _scopeProvider;

    public LocationService(IUnitOfWork unitOfWork, IUserScopeProvider scopeProvider)
    {
        _unitOfWork = unitOfWork;
        _scopeProvider = scopeProvider;
    }

    // 获取所有位置信息
    public async Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync()
    {
        //返回所有位置信息
        var scope = await _scopeProvider.GetScopeAsync();
        var locations = await _unitOfWork.Locations.GetAllFilteredAsync(
            scope.IsAdmin ? null : scope.LocationIds);
        return locations.Select(MapToResponseDto);
    }

    // 获取所有父位置信息
    public async Task<IEnumerable<LocationResponseDto>> GetAllParentLocationsAsync()
    {
        var scope = await _scopeProvider.GetScopeAsync();
        var locations = await _unitOfWork.Locations.GetLocationsByParentIdAsync(null);
        return FilterByScope(locations, scope).Select(MapToResponseDto);
    }

    // 获取指定父位置的子位置信息
    public async Task<IEnumerable<LocationResponseDto>> GetAllChildLocationsByParentIdAsync(int parentId)
    {
        var scope = await _scopeProvider.GetScopeAsync();

        // 连父位置都无权查看时，其子位置同样不可见（权限不级联，按 ID 精确比对）
        if (!scope.IsAdmin && !scope.LocationIds.Contains(parentId))
            return Array.Empty<LocationResponseDto>();

        var locations = await _unitOfWork.Locations.GetLocationsByParentIdAsync(parentId);
        return FilterByScope(locations, scope).Select(MapToResponseDto);
    }

    // 获取指定父位置的子位置信息
    public async Task<LocationResponseDto?> GetLocationByNameAsync(string name)
    {
        var location = await _unitOfWork.Locations.GetLocationByNameAsync(name);
        if (location == null) return null;

        // 按名称查询同样受数据范围约束，避免绕过列表接口越权读取
        var scope = await _scopeProvider.GetScopeAsync();
        if (!scope.IsAdmin && !scope.LocationIds.Contains(location.Id)) return null;

        return MapToResponseDto(location);
    }

    // 创建位置
    public async Task<LocationResponseDto> CreateLocationAsync(CreateLocationDto dto)
    {
        var existingLocation = await _unitOfWork.Locations.GetLocationByNameAsync(dto.Name);
        if (existingLocation is not null)
            throw new BusinessException("位置名已使用，请更换其它位置名");

        var location = Location.Create(dto.Name, dto.ParentId);
        await _unitOfWork.Locations.AddAsync(location);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponseDto(location);
    }

    // 删除位置
    public async Task<bool> DeleteLocationAsync(int id)
    {
        var location = await _unitOfWork.Locations.GetByIdAsync(id)
                    ?? throw new KeyNotFoundException("位置不存在, 请检查");

        await EnsureLocationInScopeAsync(location);

        // 用 exists 查询判断，避免把子位置和物品整张图加载进内存
        if (await _unitOfWork.Locations.HasChildrenOrItemsAsync(id))
            throw new BusinessException("该分类下存在子分类或物品，无法直接删除");

        _unitOfWork.Locations.Delete(location);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // 修改位置信息
    public async Task<LocationResponseDto?> UpdateLocationAsync(int id, UpdateLocationDto dto)
    {
        var location = await _unitOfWork.Locations.GetByIdAsync(id)
                        ?? throw new KeyNotFoundException("位置不存在, 请检查");

        // 写操作校验：目标位置本身、以及要挂靠的新父位置都必须落在授权范围内
        await EnsureLocationInScopeAsync(location);
        await EnsureParentInScopeAsync(dto.ParentId);

        var existing = await _unitOfWork.Locations.GetLocationByNameAsync(dto.Name);
        if (existing != null && existing.Id != id)
        {
            throw new BusinessException($"已存在名称为 '{dto.Name}' 的位置");
        }

        location.Update(dto.Name, dto.ParentId);
        _unitOfWork.Locations.Update(location);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponseDto(location);
    }

    // 数据范围过滤：IsAdmin 为 true 时代表不做限制
    private static IEnumerable<Location> FilterByScope(IEnumerable<Location> source, UserScope scope)
        => scope.IsAdmin ? source : source.Where(l => scope.LocationIds.Contains(l.Id));

    /// <summary>写操作校验：目标位置必须落在调用者授权范围内，越权则抛 403。</summary>
    private async Task EnsureLocationInScopeAsync(Location location)
    {
        var scope = await _scopeProvider.GetScopeAsync();
        if (scope.IsAdmin) return;

        if (!scope.LocationIds.Contains(location.Id))
            throw new UnauthorizedAccessException("无权操作该位置");
    }

    /// <summary>写操作校验：不能把位置挂到调用者无权访问的父位置下。</summary>
    private async Task EnsureParentInScopeAsync(int? parentId)
    {
        if (!parentId.HasValue) return;

        var scope = await _scopeProvider.GetScopeAsync();
        if (scope.IsAdmin) return;

        if (!scope.LocationIds.Contains(parentId.Value))
            throw new UnauthorizedAccessException("无权将位置挂靠到该父位置下");
    }

    private static LocationResponseDto MapToResponseDto(Location location)
    {
        return new LocationResponseDto
        {
            Id = location.Id,
            Name = location.Name,
            ParentId = location.ParentId,
            ParentName = location.Parent?.Name,
            ImageUrl = location.ImageUrl
        };
    }
}