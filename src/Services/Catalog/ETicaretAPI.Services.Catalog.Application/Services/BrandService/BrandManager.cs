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
using ETicaretAPI.Services.Catalog.Domain.DTOs.BrandDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Services.BrandService;

public class BrandManager : IBrandService
{
    private readonly IBrandRepository _brandRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    private static string ProductCacheKey(int productId) => $"catalog:product:{productId}";

    public BrandManager(
        IBrandRepository brandRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService cacheService)
    {
        _brandRepository = brandRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<PagedResult<GetBrandDto>>> GetBrandsFilterAsync(GetBrandForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var predicate = BrandPredicate.GetExpression(filterDto);
        var orders = BrandOrder.GetOrder(filterDto.OrderBy, filterDto.OrderType);

        var brands = await _brandRepository.GetAllAsNoTrackingWithPaginationAsync(
            filterDto.Page, filterDto.PageSize, predicate, orders, cancellationToken: cancellationToken);

        var result = _mapper.Map<PagedResult<GetBrandDto>>(brands);
        return ApiResponse<PagedResult<GetBrandDto>>.Success(result);
    }

    public async Task<ApiResponse<GetBrandDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetWithAsNoTrackingAsync(
            x => x.Id == id, cancellationToken: cancellationToken);

        if (brand is null)
            throw new NotFoundException(nameof(Brand), id);

        var result = _mapper.Map<GetBrandDto>(brand);
        return ApiResponse<GetBrandDto>.Success(result);
    }

    public async Task<ApiResponse<GetBrandDto>> CreateAsync(CreateBrandDto dto, CancellationToken cancellationToken = default)
    {
        var brand = _mapper.Map<Brand>(dto);
        brand.Slug = dto.Name?.ToSlug() ?? string.Empty;
        brand.IsActive = true;

        // Slug uniqueness kontrolü
        if (await _brandRepository.IsSlugExistsAsync(brand.Slug, cancellationToken: cancellationToken))
            throw new ValidationException([$"'{brand.Slug}' slug'ı zaten kullanılıyor. Farklı bir marka adı deneyin."]);

        await _brandRepository.AddAsync(brand, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetBrandDto>(brand);
        return ApiResponse<GetBrandDto>.Success(result);
    }

    public async Task<ApiResponse<GetBrandDto>> UpdateAsync(UpdateBrandDto dto, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: cancellationToken);

        if (brand is null)
            throw new NotFoundException(nameof(Brand), dto.Id);

        _mapper.Map(dto, brand);
        brand.Slug = dto.Name.ToSlug();

        // Slug uniqueness kontrolü (kendi slug'ını hariç tut)
        if (await _brandRepository.IsSlugExistsAsync(brand.Slug, dto.Id, cancellationToken))
            throw new ValidationException([$"'{brand.Slug}' slug'ı zaten kullanılıyor. Farklı bir marka adı deneyin."]);

        await _brandRepository.UpdateAsync(brand, cancellationToken);

        await InvalidateProductCachesByBrandAsync(dto.Id, cancellationToken);

        var result = _mapper.Map<GetBrandDto>(brand);
        return ApiResponse<GetBrandDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (brand is null)
            throw new NotFoundException(nameof(Brand), id);

        await _brandRepository.SoftDeleteAsync(brand, cancellationToken);

        await InvalidateProductCachesByBrandAsync(id, cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    private async Task InvalidateProductCachesByBrandAsync(int brandId, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(x => x.BrandId == brandId, cancellationToken: cancellationToken);
        foreach (var product in products)
            await _cacheService.RemoveAsync(ProductCacheKey(product.Id), cancellationToken);
    }
}
