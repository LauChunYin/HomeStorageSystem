using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;

namespace HomeStorage.Application.Services;

public class CategoryService : ICategoryService
{
    // 注入我们第三步写好的物品仓储管家
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    // 获取所有品类信息
    public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToResponseDto);
    }

    // 获取所有父品类信息
    public async Task<IEnumerable<CategoryResponseDto>> GetAllParentCategoriesAsync()
    {
        var categories = await _categoryRepository.GetCategoriesByParentIdAsync(0);
        return categories.Select(MapToResponseDto);
    }

    // 获取指定父品类的子品类信息
    public async Task<IEnumerable<CategoryResponseDto>> GetAllChildCategoriesByParentIdAsync(int parentId)
    {
        var categories = await _categoryRepository.GetCategoriesByParentIdAsync(parentId);
        return categories.Select(MapToResponseDto);
    }

    // 获取指定父品类的子品类信息
    public async Task<CategoryResponseDto?> GetCategoryByNameAsync(string name)
    {
        var category = await _categoryRepository.GetCategoryByNameAsync(name);
        return category == null ? null :MapToResponseDto(category);
    }

    // 创建信品类
    public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var existingCategory = await _categoryRepository.GetCategoryByNameAsync(dto.Name);
        if(existingCategory is not null)
            throw new BusinessException("品类名已使用，请更换其它品类名");

        // 使用 BCrypt 加密
        var category = Category.Create(dto.Name, dto.ParentId);
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return MapToResponseDto(category);
    }

    // 删除品类
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
                    ?? throw new KeyNotFoundException("用户不存在, 请检查");

        _categoryRepository.Delete(category);
        await _categoryRepository.SaveChangesAsync();
        return true;
    }

    // 修改品类信息
    public async Task<CategoryResponseDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
                        ?? throw new KeyNotFoundException("用户不存在, 请检查");

        category.Update(dto.Name, dto.ParentId);
        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync();
        return MapToResponseDto(category);
    }

    private static CategoryResponseDto MapToResponseDto(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.Parent?.Id ?? 0,
            ParentName = category.Parent?.Name ?? string.Empty
        };
    }
}