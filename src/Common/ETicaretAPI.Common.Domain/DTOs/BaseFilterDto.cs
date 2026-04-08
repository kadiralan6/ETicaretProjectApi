using ETicaretAPI.Common.Domain.Entities;
using ETicaretAPI.Common.Domain.Enums;

public class BaseFilterDto<TOrderBy> : IDto where TOrderBy : Enum
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public TOrderBy OrderBy { get; set; }
    public OrderTypeEnum OrderType { get; set; }
    public string? Search { get; set; }
}