using System.ComponentModel.DataAnnotations;
using HomeStorage.Domain.Enums;

namespace HomeStorage.Application.Dtos;

public class MoveItemDto
{
    [Range(1, int.MaxValue, ErrorMessage = "库存数量不能为负数")]
    public int MoveQuantity { get; set; }

    public ItemStatus Status { get; set; } = ItemStatus.InStock;
}


