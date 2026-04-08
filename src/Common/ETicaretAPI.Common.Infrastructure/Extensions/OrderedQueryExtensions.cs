using System.Linq.Expressions;

namespace ETicaretAPI.Common.Infrastructure.Extensions;

public static class OrderedQueryExtensions
{
    public static IQueryable<T> AddOrder<T>(this IQueryable<T> query, IEnumerable<(Expression<Func<T, object>> orderSelector, bool orderAsc)> orders) where T : class
    {
        if (orders == null || !orders.Any())
            return query;

        IOrderedQueryable<T> orderedQuery = null;

        foreach (var (orderSel, orderAsc) in orders)
        {
            if (orderSel == null)
                continue;

            if (orderAsc)
            {
                orderedQuery = orderedQuery == null
                    ? query.OrderBy(orderSel)
                    : orderedQuery.ThenBy(orderSel);
                continue;
            }

            //nulls last, for descending order (order by x is null, x desc)
            var nullCheckExpr = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    orderSel.Body,
                    Expression.Constant(null, orderSel.Body.Type)
                ),
                orderSel.Parameters
            );

            orderedQuery = orderedQuery == null
                ? query.OrderBy(nullCheckExpr).ThenByDescending(orderSel)
                : orderedQuery.ThenBy(nullCheckExpr).ThenByDescending(orderSel);
        }

        query = orderedQuery ?? query;

        return query;
    }
}