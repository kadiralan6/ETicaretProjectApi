using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.SharedLibrary.Events;
using ETicaretAPI.Services.Search.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace ETicaretAPI.Services.Search.Application.EventHandlers;

public sealed class ProductDeletedEventHandler : IEventHandler<ProductDeletedEvent>
{
    private readonly IProductSearchRepository _searchRepository;
    private readonly ILogger<ProductDeletedEventHandler> _logger;

    public ProductDeletedEventHandler(
        IProductSearchRepository searchRepository,
        ILogger<ProductDeletedEventHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task HandleAsync(ProductDeletedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing product {ProductId} from search index", @event.ProductId);

        await _searchRepository.DeleteProductAsync(@event.ProductId, cancellationToken);

        _logger.LogInformation("Product {ProductId} removed from search index", @event.ProductId);
    }
}
