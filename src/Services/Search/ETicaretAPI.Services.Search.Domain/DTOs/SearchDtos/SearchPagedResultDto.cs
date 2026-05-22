namespace ETicaretAPI.Services.Search.Domain.DTOs.SearchDtos;

public class SearchPagedResultDto<T>
{
    public List<T> Results { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int PageCount => (int)Math.Ceiling((double)TotalCount / PageSize);
}
