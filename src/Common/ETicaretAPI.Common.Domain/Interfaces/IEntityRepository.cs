using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Common.Domain.Interfaces;

/// <summary>
/// Generic repository pattern interface. Tüm servisler bu interface'i kullanır.
/// </summary>
public interface IEntityRepository<TEntity> where TEntity : class, IEntity<int>, new()
{
  #region Synchronous Methods
  IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties);
  IEnumerable<TEntity> GetAllWithAsNoTracking(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties);
  IEnumerable<TEntity> GetAllWithSelector(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TEntity>>? selector = null);
  IEnumerable<TEntity> GetAllAsNoTrackingWithSelector(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TEntity>>? selector = null);
  IPagedResult<TEntity> GetAllWithPagination(int page = 1, int pageSize = 10, Expression<Func<TEntity, bool>> predicate = null,
  IEnumerable<(Expression<Func<TEntity, object>> orderSelector, bool orderAsc)> orders = null, Expression<Func<TEntity, TEntity>> selector = null);
  IPagedResult<TEntity> GetAllAsNoTrackingWithPagination(int page = 1, int pageSize = 10, Expression<Func<TEntity, bool>> predicate = null,
  IEnumerable<(Expression<Func<TEntity, object>> orderSelector, bool orderAsc)> orders = null, Expression<Func<TEntity, TEntity>> selector = null);
  TEntity? Get(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
  TEntity? GetWithAsNoTracking(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
  TEntity? GetWithSelector(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>>? selector = null);
  TEntity? GetAsNoTrackingWithSelector(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>>? selector = null);
  TEntity Add(TEntity entity);
  List<TEntity> AddRange(IEnumerable<TEntity> entities);
  TEntity Update(TEntity entity);
  List<TEntity> UpdateRange(IEnumerable<TEntity> entities);
  bool HardDelete(TEntity entity);
  List<bool> HardDeleteRange(IEnumerable<TEntity> entities);
  bool SoftDelete(TEntity entity);
  List<bool> SoftDeleteRange(IEnumerable<TEntity> entities);
  List<TEntity> GetListWithStoredProsedure(string prosedureName, object[] parameters = null);
  bool ExecuteStoredProcedure(string prosedureName, object[] parameters = null);
  int Count(Expression<Func<TEntity, bool>>? predicate = null);
  decimal Average(string columnName, Expression<Func<TEntity, bool>>? predicate = null);
  #endregion

  #region Async Methods
  Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, object>>[]? includeProperties = null, CancellationToken cancellationToken = default);
  Task<IEnumerable<TEntity>> GetAllWithAsNoTrackingAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, object>>[]? includeProperties = null, CancellationToken cancellationToken = default);
  Task<IEnumerable<TEntity>> GetAllWithSelectorAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TEntity>>? selector = null, CancellationToken cancellationToken = default);
  Task<IEnumerable<TEntity>> GetAllAsNoTrackingWithSelectorAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TEntity>>? selector = null, CancellationToken cancellationToken = default);
  Task<IPagedResult<TEntity>> GetAllWithPaginationAsync(int page = 1, int pageSize = 10, Expression<Func<TEntity, bool>> predicate = null,
    IEnumerable<(Expression<Func<TEntity, object>> orderSelector, bool orderAsc)> orders = null, Expression<Func<TEntity, TEntity>> selector = null, CancellationToken cancellationToken = default);
  Task<IPagedResult<TEntity>> GetAllAsNoTrackingWithPaginationAsync(int page = 1, int pageSize = 10, Expression<Func<TEntity, bool>> predicate = null,
    IEnumerable<(Expression<Func<TEntity, object>> orderSelector, bool orderAsc)> orders = null, Expression<Func<TEntity, TEntity>> selector = null, CancellationToken cancellationToken = default);
  Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? includeProperties = null, CancellationToken cancellationToken = default);
  Task<TEntity?> GetWithAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? includeProperties = null, CancellationToken cancellationToken = default);
  Task<TEntity?> GetWithSelectorAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>>? selector = null, CancellationToken cancellationToken = default);
  Task<TEntity?> GetAsNoTrackingWithSelectorAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>>? selector = null, CancellationToken cancellationToken = default);
  Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
  Task<List<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
  Task<TEntity> UpdateAsync(TEntity? entity, CancellationToken cancellationToken = default);
  Task<List<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
  Task<bool> HardDeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
  Task<List<bool>> HardDeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
  Task<bool> SoftDeleteAsync(TEntity? entity, CancellationToken cancellationToken = default);
  Task<List<bool>> SoftDeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
  Task<List<TEntity>> GetListWithStoredProsedureAsync(string prosedureName, object[] parameters = null, CancellationToken cancellationToken = default);
  Task<bool> ExecuteStoredProcedureAsync(string prosedureName, object[] parameters = null, CancellationToken cancellationToken = default);
  Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
  Task<decimal> AverageAsync(string columnName, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
  Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

  /// <summary>
  /// Entity'nin sadece belirtilen property'lerini günceller. ModifiedAt otomatik ayarlanır.
  /// </summary>
  Task<TEntity> UpdateFieldAsync(TEntity entity, Expression<Func<TEntity, object>>[] propertiesToUpdate, CancellationToken cancellationToken = default);

  /// <summary>
  /// Birden fazla entity'nin sadece belirtilen property'lerini toplu günceller. ModifiedAt otomatik ayarlanır.
  /// </summary>
  Task<List<TEntity>> UpdateFieldRangeAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>>[] propertiesToUpdate, CancellationToken cancellationToken = default);
  #endregion
}
