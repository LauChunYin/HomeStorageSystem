using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
        var items = await _userService.GetAllUsersAsync();
        return Ok(items);
    }

    // 2. POST: /api/users (创建用户)
    //返回：200+用户信息
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto dto)
    {
        var item = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUserbyName), new { userName = item.UserName }, item);
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
        var items = await _userService.GetUserByUserNameAsync(userName);
        return Ok(items);
    }

    //仅用户本人可以用
    // 5. PUT: /api/users/id (修改用户信息)
    //返回：204
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUserInfo(int id, [FromBody] UpdateUserInfoDto dto)
    {
        await _userService.UpdateUserInfoAsync(id, dto);
        return NoContent();
    }

    // 6. PUT: /api/users/id (修改用户密码)
    //返回：204
    [HttpPut("{id:int}/Password")]
    public async Task<IActionResult> ChangeUserPassword(int id, [FromBody] ChangePasswordDto dto)
    {
        await _userService.ChangePasswordAsync(id, dto);
        return NoContent();
    }
}
