using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.SharedLibrary.Events;
using ETicaretAPI.Services.Search.Application.Repositories;
using ETicaretAPI.Services.Search.Domain.Documents;
using Microsoft.Extensions.Logging;

namespace ETicaretAPI.Services.Search.Application.EventHandlers;

public sealed class ProductUpdatedEventHandler : IEventHandler<ProductUpdatedEvent>
{
    private readonly IProductSearchRepository _searchRepository;
    private readonly ILogger<ProductUpdatedEventHandler> _logger;

    public ProductUpdatedEventHandler(
        IProductSearchRepository searchRepository,
        ILogger<ProductUpdatedEventHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task HandleAsync(ProductUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating product {ProductId} in search index", @event.ProductId);

        var document = new ProductDocument
        {
            Id = @event.ProductId,
            Code = @event.Code,
            Name = @event.Name,
            Description = @event.Description,
            Slug = @event.Slug,
            Price = @event.Price,
            StockQuantity = @event.StockQuantity,
            IsActive = @event.IsActive,
            IsFeatured = @event.IsFeatured,
            CategoryId = @event.CategoryId,
            CategoryName = @event.CategoryName,
            BrandId = @event.BrandId,
            BrandName = @event.BrandName,
            ImageUrls = @event.ImageUrls,
            ModifiedAt = @event.UpdatedDate
        };

        await _searchRepository.UpdateProductAsync(document, cancellationToken);

        _logger.LogInformation("Product {ProductId} updated in search index", @event.ProductId);
    }
}
