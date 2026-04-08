using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Application.Results.Concrete;
using Microsoft.EntityFrameworkCore;


namespace ETicaretAPI.Common.Infrastructure.Extensions;

public static class PaginationExtensions
{
    public static IPagedResult<T> GetPaged<T>(this IQueryable<T> query, int page, int pageSize,
        int? rowCount = null) where T : class
    {
        var result = new PagedResult<T>();
        result.CurrentPage = page;
        result.PageSize = pageSize;
        result.RowCount = rowCount ?? query.Count();

        var pageCount = (double)result.RowCount / pageSize;
        result.PageCount = (int)Math.Ceiling(pageCount);

        var skip = (page - 1) * pageSize;
        result.Results = query.Skip(skip)
            .Take(pageSize)
            .ToList();

        return result;
    }

    public static async Task<IPagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query, int page, int pageSize,
        int? rowCount = null) where T : class
    {
        var result = new PagedResult<T>();
        result.CurrentPage = page;
        result.PageSize = pageSize;
        result.RowCount = rowCount ?? await query.CountAsync();

        var pageCount = (double)result.RowCount / pageSize;
        result.PageCount = (int)Math.Ceiling(pageCount);

        var skip = (page - 1) * pageSize;
        result.Results = await query.Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return result;
    }
}