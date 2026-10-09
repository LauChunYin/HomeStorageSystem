using System.ComponentModel.DataAnnotations;
using HomeStorage.Domain.Entities;

namespace HomeStorage.Application.Dtos;

public class MoveItemDto
{
    [Range(0, int.MaxValue, ErrorMessage = "库存数量不能为负数")]
    public int Quantity { get; set; }
    
    public ItemStatus Status{get;set;} = ItemStatus.InStock;
}


    