namespace ETicaretAPI.Common.Domain.Interfaces;

public interface IPagedResult<T> where T : class
{
  List<T> Results { get; set; }
  int CurrentPage { get; set; }
  int FirstRowOnPage { get; }
  int LastRowOnPage { get; }
  int PageCount { get; set; }
  int PageSize { get; set; }
  int RowCount { get; set; }
}
