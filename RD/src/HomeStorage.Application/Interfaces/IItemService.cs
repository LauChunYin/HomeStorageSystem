using HomeStorage.Application.Dtos;

namespace HomeStorage.Application.Interfaces;

public interface IItemService
{
    // 获取所有物品列表
    Task<IEnumerable<ItemResponseDto>> GetAllItemsAsync();

    // 根据 Id 查询单个物品
    Task<ItemResponseDto?> GetItemByIdAsync(int id);

    // 获取所有指定位置的物品列表
    Task<IEnumerable<ItemResponseDto>> GetItemsByCategoryAndLocationAsync(int categoryId, int locationId);

    // 创建新物品
    Task<ItemResponseDto> CreateItemAsync(CreateItemDto dto);

    // 更新已有物品（如果物品不存在返回 null）
    Task<ItemResponseDto?> UpdateItemAsync(int id, UpdateItemDto dto);

    // 删除物品（成功返回 true，不存在返回 false）
    Task<bool> DeleteItemAsync(int id);
}