using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.SharedLibrary.Events;
using ETicaretAPI.Services.Search.Application.Repositories;
using ETicaretAPI.Services.Search.Domain.Documents;
using Microsoft.Extensions.Logging;

namespace ETicaretAPI.Services.Search.Application.EventHandlers;

public sealed class ProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
{
    private readonly IProductSearchRepository _searchRepository;
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(
        IProductSearchRepository searchRepository,
        ILogger<ProductCreatedEventHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Indexing new product {ProductId}: {ProductName}", @event.ProductId, @event.Name);

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
            CreatedAt = @event.CreatedDate
        };

        await _searchRepository.IndexProductAsync(document, cancellationToken);

        _logger.LogInformation("Product {ProductId} indexed successfully", @event.ProductId);
    }
}
