namespace ETicaretAPI.Common.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern. Transaction yönetimi için kullanılır.
/// </summary>
public interface IUnitOfWork : IDisposable
{
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  Task BeginTransactionAsync(CancellationToken cancellationToken = default);
  Task CommitTransactionAsync(CancellationToken cancellationToken = default);
  Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
