using AutoMapper;
using System.Linq.Expressions;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Infrastructure.Extensions;
using ETicaretAPI.Common.SharedLibrary.Extensions;
using ETicaretAPI.Services.Catalog.Application.Orders;
using ETicaretAPI.Services.Catalog.Application.Predicates;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Domain.DTOs.CategoryDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using ETicaretAPI.Services.Catalog.Application.Selectors;

namespace ETicaretAPI.Services.Catalog.Application.Services.CategoryService;

public class CategoryManager : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    private static string ProductCacheKey(int productId) => $"catalog:product:{productId}";

    public CategoryManager(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService cacheService)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<PagedResult<GetCategoryDto>>> GetCategoriesFilterAsync(GetCategoryForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var predicate = CategoryPredicate.GetExpression(filterDto);
        predicate = predicate.And(a => a.ParentCategoryId == null);

        var orders = CategoryOrder.GetOrder(filterDto.OrderBy, filterDto.OrderType);
        var selector = CategorySelector.GetCategoryWithSubCategoriesSelector();
        var categories = await _categoryRepository.GetAllAsNoTrackingWithPaginationAsync(filterDto.Page,
             filterDto.PageSize, predicate, orders, selector, cancellationToken);

        var result = _mapper.Map<PagedResult<GetCategoryDto>>(categories);
        return ApiResponse<PagedResult<GetCategoryDto>>.Success(result);
    }

    public async Task<ApiResponse<GetCategoryDto>> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        // Eğer ParentCategoryId verilmişse, üst kategorinin var olup olmadığını doğrula
        if (dto.ParentCategoryId.HasValue)
        {
            var parentExists = await _categoryRepository.AnyAsync(
                x => x.Id == dto.ParentCategoryId.Value, cancellationToken);

            if (!parentExists)
                throw new NotFoundException($"ParentCategory with Id '{dto.ParentCategoryId.Value}' not found.");
        }

        var category = _mapper.Map<Category>(dto);
        category.Slug = dto.Name?.ToSlug() ?? string.Empty;

        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCategoryDto>(category);
        return ApiResponse<GetCategoryDto>.Success(result);
    }

    public async Task<ApiResponse<GetCategoryDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetWithAsNoTrackingAsync(
            x => x.Id == id,
            new Expression<Func<Category, object>>[] { x => x.SubCategories },
            cancellationToken);

        if (category is null)
            throw new NotFoundException(nameof(Category), id);

        var result = _mapper.Map<GetCategoryDto>(category);
        return ApiResponse<GetCategoryDto>.Success(result);
    }

    public async Task<ApiResponse<GetCategoryDto>> GetAdminDetailByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var selector = CategorySelector.GetCategoryAdminDetailSelector();
        var category = await _categoryRepository.GetAsNoTrackingWithSelectorAsync(
            x => x.Id == id,
            selector,
            cancellationToken);

        if (category is null)
            throw new NotFoundException(nameof(Category), id);

        var result = _mapper.Map<GetCategoryDto>(category);

        return ApiResponse<GetCategoryDto>.Success(result);
    }

    public async Task<ApiResponse<GetCategoryDto>> UpdateAsync(UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetAsync(x => x.Id == dto.Id);

        if (category is null)
            throw new NotFoundException(nameof(Category), dto.Id);

        _mapper.Map(dto, category);
        category.Slug = dto.Name.ToSlug();

        await _categoryRepository.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await InvalidateProductCachesByCategoryAsync(dto.Id, cancellationToken);

        var result = _mapper.Map<GetCategoryDto>(category);
        return ApiResponse<GetCategoryDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetAsync(x => x.Id == id);

        if (category is null)
            throw new NotFoundException(nameof(Category), id);

        await _categoryRepository.SoftDeleteAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await InvalidateProductCachesByCategoryAsync(id, cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    private async Task InvalidateProductCachesByCategoryAsync(int categoryId, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(x => x.CategoryId == categoryId, cancellationToken: cancellationToken);
        foreach (var product in products)
            await _cacheService.RemoveAsync(ProductCacheKey(product.Id), cancellationToken);
    }
}
