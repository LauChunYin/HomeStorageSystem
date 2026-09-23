using HomeStorage.Application.Dtos;

namespace HomeStorage.Application.Interfaces;

public interface ILocationService
{
    // 获取所有品类信息
    Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync();

    // 获取所有父品类信息
    Task<IEnumerable<LocationResponseDto>> GetAllParentLocationsAsync();

    // 根据名字获取品类信息
    Task<LocationResponseDto?> GetLocationByNameAsync(string name);

    // 获取指定父品类的子品类信息
    Task<IEnumerable<LocationResponseDto>> GetAllChildLocationsByParentIdAsync(int parentId);

    // 创建信品类
    Task<LocationResponseDto> CreateLocationAsync(CreateLocationDto dto);

    // 删除品类
    Task<bool> DeleteLocationAsync(int id);

    // 修改品类信息
    Task<LocationResponseDto?> UpdateLocationAsync(int id, UpdateLocationDto dto);
}