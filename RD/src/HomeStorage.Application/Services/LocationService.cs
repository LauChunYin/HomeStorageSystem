using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;

namespace HomeStorage.Application.Services;

public class LocationService : ILocationService
{
    // 注入我们第三步写好的物品仓储管家
    private readonly IUnitOfWork _unitOfWork;

    public LocationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    // 获取所有位置信息
    public async Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync()
    {
        //返回所有位置信息
        var locations = await _unitOfWork.Locations.GetAllAsync();
        return locations.Select(MapToResponseDto);
    }

    // 获取所有父位置信息
    public async Task<IEnumerable<LocationResponseDto>> GetAllParentLocationsAsync()
    {
        var locations = await _unitOfWork.Locations.GetLocationsByParentIdAsync(0);
        return locations.Select(MapToResponseDto);
    }

    // 获取指定父位置的子位置信息
    public async Task<IEnumerable<LocationResponseDto>> GetAllChildLocationsByParentIdAsync(int parentId)
    {
        var locations = await _unitOfWork.Locations.GetLocationsByParentIdAsync(parentId);
        return locations.Select(MapToResponseDto);
    }

    // 获取指定父位置的子位置信息
    public async Task<LocationResponseDto?> GetLocationByNameAsync(string name)
    {
        var location = await _unitOfWork.Locations.GetLocationByNameAsync(name);
        return location == null ? null :MapToResponseDto(location);
    }

    // 创建位置
    public async Task<LocationResponseDto> CreateLocationAsync(CreateLocationDto dto)
    {
        var existingLocation = await _unitOfWork.Locations.GetLocationByNameAsync(dto.Name);
        if(existingLocation is not null)
            throw new BusinessException("位置名已使用，请更换其它位置名");

        // 使用 BCrypt 加密
        var location = Location.Create(dto.Name, dto.ParentId);
        await _unitOfWork.Locations.AddAsync(location);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponseDto(location);
    }

    // 删除位置
    public async Task<bool> DeleteLocationAsync(int id)
    {
        var location = await _unitOfWork.Locations.GetLocationWithDetailsAsync(id)
                    ?? throw new KeyNotFoundException("位置不存在, 请检查");

        if (location.Children.Any() || location.Items.Any())
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

    private static LocationResponseDto MapToResponseDto(Location location)
    {
        return new LocationResponseDto
        {
            Id = location.Id,
            Name = location.Name,
            ParentId = location.Parent?.Id ?? 0,
            ParentName = location.Parent?.Name ?? string.Empty,
            ImageUrl = location.ImageUrl
        };
    }
}