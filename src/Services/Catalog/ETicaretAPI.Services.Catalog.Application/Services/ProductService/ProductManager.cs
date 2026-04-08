using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
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

  public ProductManager(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper)
  {
    _productRepository = productRepository;
    _unitOfWork = unitOfWork;
    _mapper = mapper;
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

    return ApiResponse<bool>.Success(true);
  }

  public async Task<ApiResponse<bool>> UpdateStockAsync(int productId, int quantity, CancellationToken cancellationToken = default)
  {
    var product = await _productRepository.GetAsync(x => x.Id == productId, cancellationToken: cancellationToken);

    if (product is null)
      throw new NotFoundException(nameof(Product), productId);

    product.StockQuantity = quantity;

    await _productRepository.UpdateFieldAsync(product, [x => x.StockQuantity], cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return ApiResponse<bool>.Success(true);
  }
}
