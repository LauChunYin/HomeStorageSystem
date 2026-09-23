using HomeStorage.Application.Dtos;

namespace HomeStorage.Application.Interfaces;

public interface ICategoryService
{
    // 获取所有品类信息
    Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();

    // 获取所有父品类信息
    Task<IEnumerable<CategoryResponseDto>> GetAllParentCategoriesAsync();

    // 根据名字获取品类信息
    Task<CategoryResponseDto?> GetCategoryByNameAsync(string name);

    // 获取指定父品类的子品类信息
    Task<IEnumerable<CategoryResponseDto>> GetAllChildCategoriesByParentIdAsync(int parentId);

    // 创建信品类
    Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto);

    // 删除品类
    Task<bool> DeleteCategoryAsync(int id);

    // 修改品类信息
    Task<CategoryResponseDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
}