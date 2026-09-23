using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HomeStorage.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // 访问路径：/api/users
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    // 依赖注入：系统会自动把配置好的 CategoriesController 实例传进来
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    //管理员权限可用
    // 1. GET: /api/categories (获取所有品类信息)
    // 返回 200 + 品类信息
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    // 2. POST: /api/categories (创建品类)
    //返回：200+品类信息
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetCategorybyName), new { userName = category.Name }, category);
    }

    // 3. DELETE: /api/categories/id (删除品类)
    // 返回：204
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }

    // 4. GET: /api/categories/name (获取该品类名的品类信息)
    // 返回：200+当前品类信息
    [HttpGet("{name}")]
    [Authorize]
    public async Task<ActionResult<CategoryResponseDto>> GetCategorybyName(string name)
    {

        var category = await _categoryService.GetCategoryByNameAsync(name);
        return Ok(category);
    }

    // 5. PUT: /api/categories/id (修改位置信息)
    //返回：204
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategoryInfo(int id, [FromBody] UpdateCategoryDto dto)
    {
        if (!IsOwnerOrAdmin(id)) return Forbid(); // 防水平越权

        await _categoryService.UpdateCategoryAsync(id, dto);
        return NoContent();
    }
    
    // 辅助校验：判断是否为本人或管理员
    private bool IsOwnerOrAdmin(int targetUserId)
    {
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return (int.TryParse(currentUserIdClaim, out var currentUserId) && currentUserId == targetUserId) 
               || User.IsInRole("Admin");
    }
}
