namespace ETicaretAPI.Services.Search.Domain.DTOs.SearchDtos;

public class SearchSuggestionDto
{
    public string Text { get; set; } = string.Empty;
    public double Score { get; set; }
}
