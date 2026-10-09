using HomeStorage.Application.Dtos;

namespace HomeStorage.Application.Interfaces;

public interface ILocationService
{
    // 获取所有位置信息
    Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync();

    // 获取所有父位置信息
    Task<IEnumerable<LocationResponseDto>> GetAllParentLocationsAsync();

    // 根据名字获取位置信息
    Task<LocationResponseDto?> GetLocationByNameAsync(string name);

    // 获取指定父位置的子位置信息
    Task<IEnumerable<LocationResponseDto>> GetAllChildLocationsByParentIdAsync(int parentId);

    // 创建信位置
    Task<LocationResponseDto> CreateLocationAsync(CreateLocationDto dto);

    // 删除位置
    Task<bool> DeleteLocationAsync(int id);

    // 修改位置信息
    Task<LocationResponseDto?> UpdateLocationAsync(int id, UpdateLocationDto dto);
}