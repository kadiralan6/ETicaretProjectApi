using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.SharedLibrary.Extensions;
using ETicaretAPI.Services.Catalog.Application.Orders;
using ETicaretAPI.Services.Catalog.Application.Predicates;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Application.Selectors;
using ETicaretAPI.Services.Catalog.Domain.DTOs.ProductDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Services.ProductService;

public class ProductManager : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProductImageRepository _productImageRepository;
    private readonly ICacheService _cacheService;

    private static string ProductCacheKey(int id) => $"catalog:product:{id}";
    private static readonly TimeSpan ProductCacheTtl = TimeSpan.FromMinutes(60);

    public ProductManager(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IProductImageRepository productImageRepository,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productImageRepository = productImageRepository;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<PagedResult<GetProductDto>>> GetProductsFilterAsync(GetProductForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var predicate = ProductPredicate.GetExpression(filterDto);
        var orders = ProductOrder.GetOrder(filterDto.OrderBy, filterDto.OrderType);
        var selector = ProductSelector.GetProductWithDetailsSelector();

        var products = await _productRepository.GetAllAsNoTrackingWithPaginationAsync(
            filterDto.Page, filterDto.PageSize, predicate, orders, selector, cancellationToken);

        var result = _mapper.Map<PagedResult<GetProductDto>>(products);
        return ApiResponse<PagedResult<GetProductDto>>.Success(result);
    }

    public async Task<ApiResponse<GetProductDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cached = await _cacheService.GetAsync<GetProductDto>(ProductCacheKey(id), cancellationToken);
        if (cached is not null)
            return ApiResponse<GetProductDto>.Success(cached);

        var product = await _productRepository.GetWithAsNoTrackingAsync(
            x => x.Id == id,
            new System.Linq.Expressions.Expression<Func<Product, object>>[]
            {
                x => x.Category,
                x => x.Brand
            },
            cancellationToken);

        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        var result = _mapper.Map<GetProductDto>(product);

        var imageUrls = await _productImageRepository.GetAllAsync(a => a.ProductId == id, cancellationToken: cancellationToken);
        result.ImageUrls = imageUrls.Select(i => i.Url).ToList();

        await _cacheService.SetAsync(ProductCacheKey(id), result, ProductCacheTtl, cancellationToken);

        return ApiResponse<GetProductDto>.Success(result);
    }

    public async Task<ApiResponse<GetProductDto>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = _mapper.Map<Product>(dto);
        product.Slug = dto.Name?.ToSlug() ?? string.Empty;
        product.IsActive = true;

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetProductDto>(product);
        await _cacheService.SetAsync(ProductCacheKey(product.Id), result, ProductCacheTtl, cancellationToken);

        return ApiResponse<GetProductDto>.Success(result);
    }

    public async Task<ApiResponse<GetProductDto>> UpdateAsync(UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: cancellationToken);

        if (product is null)
            throw new NotFoundException(nameof(Product), dto.Id);

        _mapper.Map(dto, product);
        product.Slug = dto.Name.ToSlug();

        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(ProductCacheKey(dto.Id), cancellationToken);

        var result = _mapper.Map<GetProductDto>(product);
        return ApiResponse<GetProductDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        await _productRepository.SoftDeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(ProductCacheKey(id), cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> UpdateStockAsync(UpdateProductQuantityDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetAsync(x => x.Id == dto.ProductId, cancellationToken: cancellationToken);

        if (product is null)
            throw new NotFoundException(nameof(Product), dto.ProductId);

        product.StockQuantity = dto.Quantity ?? product.StockQuantity;

        await _productRepository.UpdateFieldAsync(product, [x => x.StockQuantity], cancellationToken);
        await _cacheService.RemoveAsync(ProductCacheKey(dto.ProductId!.Value), cancellationToken);

        return ApiResponse<bool>.Success(true);
    }
}
