using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HomeStorage.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // 访问路径：/api/users
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    // 依赖注入：系统会自动把配置好的 locationsController 实例传进来
    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    //管理员权限可用
    // 1. GET: /api/locations (获取所有位置信息)
    // 返回 200 + 品类信息
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<LocationResponseDto>>> GetAllLocations()
    {
        var locations = await _locationService.GetAllLocationsAsync();
        return Ok(locations);
    }

    // 2. POST: /api/locations (创建位置)
    //返回：200+位置信息
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LocationResponseDto>> CreateLocation([FromBody] CreateLocationDto dto)
    {
        var location = await _locationService.CreateLocationAsync(dto);
        return CreatedAtAction(nameof(GetLocationbyName), new { Name = location.Name }, location);
    }

    // 3. DELETE: /api/locations/id (删除位置)
    // 返回：204
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        await _locationService.DeleteLocationAsync(id);
        return NoContent();
    }

    // 4. GET: /api/locations/name (获取该位置名的位置信息)
    // 返回：200+当前位置信息
    [HttpGet("{name}")]
    [Authorize]
    public async Task<ActionResult<LocationResponseDto>> GetLocationbyName(string name)
    {

        var location = await _locationService.GetLocationByNameAsync(name);
        return Ok(location);
    }

    // 5. PUT: /api/locations/id (修改位置信息)
    //返回：204
    [HttpPut("{id:int}/profile")]
    public async Task<IActionResult> UpdateLocationInfo(int id, [FromBody] UpdateLocationDto dto)
    {
        if (!IsOwnerOrAdmin(id)) return Forbid(); // 防水平越权

        await _locationService.UpdateLocationAsync(id, dto);
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
