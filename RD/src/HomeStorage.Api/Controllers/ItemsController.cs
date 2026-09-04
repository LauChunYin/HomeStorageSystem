using HomeStorage.Application.Dtos;
using HomeStorage.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeStorage.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // 访问路径：/api/items
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    // 依赖注入：系统会自动把配置好的 ItemService 实例传进来
    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    // 1. GET: /api/items (获取所有物品)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetAll()
    {
        var items = await _itemService.GetAllItemsAsync();
        return Ok(items);
    }

    // 2. GET: /api/items/5 (根据 Id 获取单个物品)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ItemResponseDto>> GetById(int id)
    {
        var item = await _itemService.GetItemByIdAsync(id);
        if (item == null) return NotFound(new { message = $"未找到 ID 为 {id} 的物品" });
        return Ok(item);
    }

    // 3. GET: /api/items/locations (获取所有存放位置)
    [HttpGet("locations")]
    public async Task<ActionResult<IEnumerable<string>>> GetLocations()
    {
        var locations = await _itemService.GetLocationsAsync();
        return Ok(locations);
    }

    // 4. POST: /api/items (创建新物品)
    [HttpPost]
    public async Task<ActionResult<ItemResponseDto>> Create([FromBody] CreateItemDto dto)
    {
        var createdItem = await _itemService.CreateItemAsync(dto);
        // 返回 201 Created 状态码，并在 Header 中带上查询该新物品的 URL 地址
        return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
    }

    // 5. PUT: /api/items/5 (修改物品)
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ItemResponseDto>> Update(int id, [FromBody] UpdateItemDto dto)
    {
        var updatedItem = await _itemService.UpdateItemAsync(id, dto);
        if (updatedItem == null) return NotFound(new { message = $"未找到 ID 为 {id} 的物品" });
        return Ok(updatedItem);
    }

    // 6. DELETE: /api/items/5 (删除物品)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _itemService.DeleteItemAsync(id);
        if (!success) return NotFound(new { message = $"未找到 ID 为 {id} 的物品" });
        return NoContent(); // 返回 204 无内容
    }
}