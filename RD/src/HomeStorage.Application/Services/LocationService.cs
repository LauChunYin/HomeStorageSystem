using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;

namespace HomeStorage.Application.Services;

public class LocationService : ILocationService
{
    // 注入我们第三步写好的物品仓储管家
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }
    
    // 获取所有位置信息
    public async Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync()
    {
        //返回所有用户信息
        var locations = await _locationRepository.GetAllAsync();
        return locations.Select(MapToResponseDto);
    }

    // 获取所有父品类信息
    public async Task<IEnumerable<LocationResponseDto>> GetAllParentLocationsAsync()
    {
        var locations = await _locationRepository.GetLocationsByParentIdAsync(0);
        return locations.Select(MapToResponseDto);
    }

    // 获取指定父品类的子品类信息
    public async Task<IEnumerable<LocationResponseDto>> GetAllChildLocationsByParentIdAsync(int parentId)
    {
        var locations = await _locationRepository.GetLocationsByParentIdAsync(parentId);
        return locations.Select(MapToResponseDto);
    }

    // 获取指定父品类的子品类信息
    public async Task<LocationResponseDto?> GetLocationByNameAsync(string name)
    {
        var location = await _locationRepository.GetLocationByNameAsync(name);
        return location == null ? null :MapToResponseDto(location);
    }

    // 创建信品类
    public async Task<LocationResponseDto> CreateLocationAsync(CreateLocationDto dto)
    {
        var existingLocation = await _locationRepository.GetLocationByNameAsync(dto.Name);
        if(existingLocation is not null)
            throw new BusinessException("品类名已使用，请更换其它品类名");

        // 使用 BCrypt 加密
        var location = Location.Create(dto.Name, dto.ParentId);
        await _locationRepository.AddAsync(location);
        await _locationRepository.SaveChangesAsync();
        return MapToResponseDto(location);
    }

    // 删除品类
    public async Task<bool> DeleteLocationAsync(int id)
    {
        var category = await _locationRepository.GetByIdAsync(id)
                    ?? throw new KeyNotFoundException("用户不存在, 请检查");

        _locationRepository.Delete(category);
        await _locationRepository.SaveChangesAsync();
        return true;
    }

    // 修改品类信息
    public async Task<LocationResponseDto?> UpdateLocationAsync(int id, UpdateLocationDto dto)
    {
        var location = await _locationRepository.GetByIdAsync(id)
                        ?? throw new KeyNotFoundException("用户不存在, 请检查");

        location.Update(dto.Name, dto.ParentId);
        _locationRepository.Update(location);
        await _locationRepository.SaveChangesAsync();
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