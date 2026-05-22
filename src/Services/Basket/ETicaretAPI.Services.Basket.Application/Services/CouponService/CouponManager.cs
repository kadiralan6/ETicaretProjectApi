using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Basket.Application.Orders;
using ETicaretAPI.Services.Basket.Application.Predicates;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.DTOs.CouponDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Services.CouponService;

public class CouponManager : ICouponService
{
    private readonly ICouponRepository _couponRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CouponManager(
        ICouponRepository couponRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _couponRepository = couponRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<GetCouponDto>>> GetCouponsFilterAsync(
        GetCouponForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var predicate = CouponPredicate.GetExpression(filterDto);
        var orders = CouponOrder.GetOrder(filterDto.OrderBy, filterDto.OrderType);

        var coupons = await _couponRepository.GetAllAsNoTrackingWithPaginationAsync(
            filterDto.Page, filterDto.PageSize, predicate, orders, cancellationToken: cancellationToken);

        var result = _mapper.Map<PagedResult<GetCouponDto>>(coupons);
        return ApiResponse<PagedResult<GetCouponDto>>.Success(result);
    }

    public async Task<ApiResponse<GetCouponDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var coupon = await _couponRepository.GetWithAsNoTrackingAsync(
            x => x.Id == id, cancellationToken: cancellationToken);

        if (coupon is null)
            throw new NotFoundException(nameof(Coupon), id);

        var result = _mapper.Map<GetCouponDto>(coupon);
        return ApiResponse<GetCouponDto>.Success(result);
    }

    public async Task<ApiResponse<GetCouponDto>> CreateAsync(CreateCouponDto dto, CancellationToken cancellationToken = default)
    {
        if (await _couponRepository.IsCodeExistsAsync(dto.Code, cancellationToken: cancellationToken))
            throw new ValidationException([$"'{dto.Code}' kodu zaten kullanılıyor."]);

        if (dto.ExpirationDate <= DateTime.UtcNow)
            throw new ValidationException(["Son kullanma tarihi geçmiş bir kupon oluşturulamaz."]);

        var coupon = _mapper.Map<Coupon>(dto);
        coupon.IsActive = true;
        coupon.UsageCount = 0;

        await _couponRepository.AddAsync(coupon, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCouponDto>(coupon);
        return ApiResponse<GetCouponDto>.Success(result);
    }

    public async Task<ApiResponse<GetCouponDto>> UpdateAsync(UpdateCouponDto dto, CancellationToken cancellationToken = default)
    {
        var coupon = await _couponRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: cancellationToken);

        if (coupon is null)
            throw new NotFoundException(nameof(Coupon), dto.Id);

        if (await _couponRepository.IsCodeExistsAsync(dto.Code, dto.Id, cancellationToken))
            throw new ValidationException([$"'{dto.Code}' kodu zaten kullanılıyor."]);

        _mapper.Map(dto, coupon);

        await _couponRepository.UpdateAsync(coupon, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCouponDto>(coupon);
        return ApiResponse<GetCouponDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var coupon = await _couponRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (coupon is null)
            throw new NotFoundException(nameof(Coupon), id);

        await _couponRepository.SoftDeleteAsync(coupon, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }
}
