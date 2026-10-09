using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HomeStorage.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // 访问路径：/api/users
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    // 依赖注入：系统会自动把配置好的 UserService 实例传进来
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    //管理员权限可用
    // 1. GET: /api/users (获取所有用户信息)
    // 返回 200 + 用户信息
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // 2. POST: /api/users (创建用户)
    //返回：200+用户信息
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUserbyName), new { userName = user.UserName }, user);
    }

    // 3. DELETE: /api/users/id (删除用户)
    // 返回：204
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }

    //用户本人和管理员都可用
    // 4. GET: /api/users/userName (获取该用户名的用户信息)
    // 返回：200+当前用户信息
    [HttpGet("{userName}")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetUserbyName(string userName)
    {
        var currentUserName = User.FindFirst(ClaimTypes.Name)?.Value;
        if(currentUserName != userName && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var user = await _userService.GetUserByUserNameAsync(userName);
        if(user == null) return NotFound("未找到该用户");
        return Ok(user);
    }

    //仅用户本人可以用
    // 5. PUT: /api/users/id/Profile (修改用户信息)
    //返回：204
    [HttpPut("{id:int}/profile")]
    public async Task<IActionResult> UpdateUserInfo(int id, [FromBody] UpdateUserInfoDto dto)
    {
        if (!IsOwnerOrAdmin(id)) return Forbid(); // 防水平越权

        await _userService.UpdateUserInfoAsync(id, dto);
        return NoContent();
    }

    //仅用户本人可以用
    // 5. PUT: /api/users/id/Role (修改用户信息)
    //返回：204
    [HttpPut("{id:int}/Role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto dto)
    {
        if (!IsOwnerOrAdmin(id)) return Forbid(); // 防水平越权

        await _userService.UpdateUserRoleAsync(id, dto);
        return NoContent();
    }

    // 6. PUT: /api/users/id/Password (修改用户密码)
    //返回：204
    [HttpPut("{id:int}/Password")]
    public async Task<IActionResult> ChangeUserPassword(int id, [FromBody] ChangePasswordDto dto)
    {
        if (!IsOwnerOrAdmin(id)) return Forbid(); // 防水平越权
        
        await _userService.ChangePasswordAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// 为指定角色分配/更新功能权限点（仅限管理员）
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="dto">选中的权限 ID 列表</param>
    [HttpPut("roles/{roleId:int}/permissions")]
    [Authorize(Roles = "Admin")] // 核心防线：只有系统管理员有权调整权限
    public async Task<IActionResult> UpdateRolePermissions(
        [FromRoute] int roleId, 
        [FromBody] UpdateRolePermissionsDto dto)
    {
        await _userService.UpdateRolePermissionsAsync(roleId, dto.PermissionIds);
        return NoContent(); // 204 修改成功，无需返回 Body
    }

    /// <summary>
    /// 为指定角色分配/更新数据品类授权范围（仅限管理员）
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="dto">选中的品类 ID 列表</param>
    [HttpPut("roles/{roleId:int}/categories")]
    [Authorize(Roles = "Admin")] // 核心防线：只有系统管理员有权调整数据权限
    public async Task<IActionResult> UpdateRoleCategories(
        [FromRoute] int roleId, 
        [FromBody] UpdateRoleCategoriesDto dto)
    {
        await _userService.UpdateRoleCategoriesAsync(roleId, dto.CategoryIds);
        return NoContent(); // 204 修改成功，无需返回 Body
    }

    /// <summary>
    /// 为指定角色分配/更新数据品类授权范围（仅限管理员）
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="dto">选中的品类 ID 列表</param>
    [HttpPut("roles/{roleId:int}/locations")]
    [Authorize(Roles = "Admin")] // 核心防线：只有系统管理员有权调整数据权限
    public async Task<IActionResult> UpdateRoleLocations(
        [FromRoute] int roleId, 
        [FromBody] UpdateRoleLocationsDto dto)
    {
        await _userService.UpdateRoleLocationsAsync(roleId, dto.LocationIds);
        return NoContent(); // 204 修改成功，无需返回 Body
    }

    // 辅助校验：判断是否为本人或管理员
    private bool IsOwnerOrAdmin(int targetUserId)
    {
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return (int.TryParse(currentUserIdClaim, out var currentUserId) && currentUserId == targetUserId) 
               || User.IsInRole("Admin");
    }
}
