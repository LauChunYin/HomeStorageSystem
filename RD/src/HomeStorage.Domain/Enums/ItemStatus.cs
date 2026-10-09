using System.ComponentModel;

public enum ItemStatus
{
    [Description("在库")]
    InStock=1,
    [Description("出库")]
    OutStock,
    [Description("售罄")]
    SoldOut
}