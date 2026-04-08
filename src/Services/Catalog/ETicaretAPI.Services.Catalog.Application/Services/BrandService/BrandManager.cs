using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BrandManager(IBrandRepository brandRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

        await _brandRepository.UpdateAsync(brand, cancellationToken);

        var result = _mapper.Map<GetBrandDto>(brand);
        return ApiResponse<GetBrandDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (brand is null)
            throw new NotFoundException(nameof(Brand), id);

        await _brandRepository.SoftDeleteAsync(brand, cancellationToken);

        return ApiResponse<bool>.Success(true);
    }
}
