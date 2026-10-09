using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;

namespace HomeStorage.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    // 获取所有品类信息
    public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return categories.Select(MapToResponseDto);
    }

    // 获取所有父品类信息
    public async Task<IEnumerable<CategoryResponseDto>> GetAllParentCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetCategoriesByParentIdAsync(0);
        return categories.Select(MapToResponseDto);
    }

    // 获取指定父品类的子品类信息
    public async Task<IEnumerable<CategoryResponseDto>> GetAllChildCategoriesByParentIdAsync(int parentId)
    {
        var categories = await _unitOfWork.Categories.GetCategoriesByParentIdAsync(parentId);
        return categories.Select(MapToResponseDto);
    }

    // 获取指定父品类的子品类信息
    public async Task<CategoryResponseDto?> GetCategoryByNameAsync(string name)
    {
        var category = await _unitOfWork.Categories.GetCategoryByNameAsync(name);
        return category == null ? null :MapToResponseDto(category);
    }

    // 创建信品类
    public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var existingCategory = await _unitOfWork.Categories.GetCategoryByNameAsync(dto.Name);
        if(existingCategory is not null)
            throw new BusinessException("品类名已使用，请更换其它品类名");

        // 使用 BCrypt 加密
        var category = Category.Create(dto.Name, dto.ParentId);
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponseDto(category);
    }

    // 删除品类
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetCategoryWithDetailsAsync(id)
                    ?? throw new KeyNotFoundException("用户不存在, 请检查");

        if (category.Children.Any() || category.Items.Any())
            throw new BusinessException("该分类下存在子分类或物品，无法直接删除");

        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // 修改品类信息
    public async Task<CategoryResponseDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id)
                        ?? throw new KeyNotFoundException("品类不存在, 请检查");

        var existing = await _unitOfWork.Categories.GetCategoryByNameAsync(dto.Name);
        if (existing != null && existing.Id != id)
        {
            throw new BusinessException($"已存在名称为 '{dto.Name}' 的品类");
        }

        category.Update(dto.Name, dto.ParentId);
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();
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