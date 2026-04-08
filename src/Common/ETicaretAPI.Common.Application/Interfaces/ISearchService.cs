namespace ETicaretAPI.Common.Application.Interfaces;

/// <summary>
/// Elasticsearch arama servisi interface'i. Ürün arama vb. için kullanılır.
/// </summary>
public interface ISearchService<T> where T : class
{
  Task<IReadOnlyList<T>> SearchAsync(string query, CancellationToken cancellationToken = default);
  Task IndexDocumentAsync(T document, CancellationToken cancellationToken = default);
  Task IndexManyAsync(IEnumerable<T> documents, CancellationToken cancellationToken = default);
  Task DeleteDocumentAsync(string id, CancellationToken cancellationToken = default);
  Task<bool> IndexExistsAsync(CancellationToken cancellationToken = default);
  Task CreateIndexAsync(CancellationToken cancellationToken = default);
}
