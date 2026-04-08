
using ETicaretAPI.Common.Domain.Interfaces;

namespace ETicaretAPI.Common.Application.Results.Concrete;

public class PagedResult<T> : IPagedResult<T> where T : class
{
    public List<T> Results { get; set; } = new();

    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public int PageSize { get; set; }
    public int RowCount { get; set; }
    public int FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;
    public int LastRowOnPage => Math.Min(CurrentPage * PageSize, RowCount);

}