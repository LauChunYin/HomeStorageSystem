using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using HomeStorage.Domain.Entities;
using HomeStorage.Domain.Interfaces;
using HomeStorage.Domain.Exceptions;

namespace HomeStorage.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserScopeProvider _scopeProvider;

    public CategoryService(IUnitOfWork unitOfWork, IUserScopeProvider scopeProvider)
    {
        _unitOfWork = unitOfWork;
        _scopeProvider = scopeProvider;
    }

    // 获取所有品类信息
    public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var scope = await _scopeProvider.GetScopeAsync();
        var categories = await _unitOfWork.Categories.GetAllFilteredAsync(
            scope.IsAdmin ? null : scope.CategoryIds);
        return categories.Select(MapToResponseDto);
    }

    // 获取所有父品类信息
    public async Task<IEnumerable<CategoryResponseDto>> GetAllParentCategoriesAsync()
    {
        var scope = await _scopeProvider.GetScopeAsync();

        // 根节点的 ParentId 在数据库中是 NULL，必须传 null 而不是 0
        var categories = await _unitOfWork.Categories.GetCategoriesByParentIdAsync(null);
        return FilterByScope(categories, scope).Select(MapToResponseDto);
    }

    // 获取指定父品类的子品类信息
    public async Task<IEnumerable<CategoryResponseDto>> GetAllChildCategoriesByParentIdAsync(int parentId)
    {
        var scope = await _scopeProvider.GetScopeAsync();

        // 连父品类都无权查看时，其子品类同样不可见（权限不级联，按 ID 精确比对）
        if (!scope.IsAdmin && !scope.CategoryIds.Contains(parentId))
            return Array.Empty<CategoryResponseDto>();

        var categories = await _unitOfWork.Categories.GetCategoriesByParentIdAsync(parentId);
        return FilterByScope(categories, scope).Select(MapToResponseDto);
    }

    // 获取指定父品类的子品类信息
    public async Task<CategoryResponseDto?> GetCategoryByNameAsync(string name)
    {
        var category = await _unitOfWork.Categories.GetCategoryByNameAsync(name);
        if (category == null) return null;

        // 按名称查询同样受数据范围约束，避免绕过列表接口越权读取
        var scope = await _scopeProvider.GetScopeAsync();
        if (!scope.IsAdmin && !scope.CategoryIds.Contains(category.Id)) return null;

        return MapToResponseDto(category);
    }

    // 创建信品类
    public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var existingCategory = await _unitOfWork.Categories.GetCategoryByNameAsync(dto.Name);
        if (existingCategory is not null)
            throw new BusinessException("品类名已使用，请更换其它品类名");

        var category = Category.Create(dto.Name, dto.ParentId);
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponseDto(category);
    }

    // 删除品类
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id)
                    ?? throw new KeyNotFoundException("品类不存在, 请检查");

        await EnsureCategoryInScopeAsync(category);

        // 用 exists 查询判断，避免把子品类和物品整张图加载进内存
        if (await _unitOfWork.Categories.HasChildrenOrItemsAsync(id))
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

        // 写操作校验：目标品类本身、以及要挂靠的新父品类都必须落在授权范围内
        await EnsureCategoryInScopeAsync(category);
        await EnsureParentInScopeAsync(dto.ParentId);

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

    // 数据范围过滤：IsAdmin 为 true 时代表不做限制
    private static IEnumerable<Category> FilterByScope(IEnumerable<Category> source, UserScope scope)
        => scope.IsAdmin ? source : source.Where(c => scope.CategoryIds.Contains(c.Id));

    /// <summary>写操作校验：目标品类必须落在调用者授权范围内，越权则抛 403。</summary>
    private async Task EnsureCategoryInScopeAsync(Category category)
    {
        var scope = await _scopeProvider.GetScopeAsync();
        if (scope.IsAdmin) return;

        if (!scope.CategoryIds.Contains(category.Id))
            throw new UnauthorizedAccessException("无权操作该品类");
    }

    /// <summary>写操作校验：不能把品类挂到调用者无权访问的父品类下。</summary>
    private async Task EnsureParentInScopeAsync(int? parentId)
    {
        if (!parentId.HasValue) return;

        var scope = await _scopeProvider.GetScopeAsync();
        if (scope.IsAdmin) return;

        if (!scope.CategoryIds.Contains(parentId.Value))
            throw new UnauthorizedAccessException("无权将品类挂靠到该父品类下");
    }

    private static CategoryResponseDto MapToResponseDto(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.ParentId,
            ParentName = category.Parent?.Name
        };
    }
}