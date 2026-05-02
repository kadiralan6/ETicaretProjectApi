using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Basket.Application.Orders;
using ETicaretAPI.Services.Basket.Application.Predicates;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Services.CartItemsService;

public class CartItemsManager : ICartItemsService
{
    private readonly ICartItemsRepository _cartItemsRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartItemsManager(
        ICartItemsRepository cartItemsRepository,
        ICouponRepository couponRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _cartItemsRepository = cartItemsRepository;
        _couponRepository = couponRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<GetCartItemDto>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var items = await _cartItemsRepository.GetByUserIdAsync(userId, cancellationToken);
        var result = _mapper.Map<List<GetCartItemDto>>(items);
        return ApiResponse<List<GetCartItemDto>>.Success(result);
    }

    public async Task<ApiResponse<PagedResult<GetCartItemDto>>> GetCartItemsFilterAsync(GetCartForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var predicate = CartItemsPredicate.GetExpression(filterDto);
        var orders = CartItemsOrder.GetOrder(filterDto.OrderBy, filterDto.OrderType);

        var cartItems = await _cartItemsRepository.GetAllAsNoTrackingWithPaginationAsync(
            filterDto.Page, filterDto.PageSize, predicate, orders, cancellationToken: cancellationToken);

        var result = _mapper.Map<PagedResult<GetCartItemDto>>(cartItems);
        return ApiResponse<PagedResult<GetCartItemDto>>.Success(result);
    }

    public async Task<ApiResponse<GetCartItemDto>> AddItemAsync(int userId, AddCartItemDto dto, CancellationToken cancellationToken = default)
    {
        var existingItem = await _cartItemsRepository.GetByUserAndProductAsync(userId, dto.ProductId, cancellationToken);

        if (existingItem is not null)
        {
            existingItem.Quantity += dto.Quantity;
            existingItem.CouponId = dto.CouponId;
            existingItem.OrderNumber = dto.OrderNumber;
            await _cartItemsRepository.UpdateAsync(existingItem, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var existingResult = _mapper.Map<GetCartItemDto>(existingItem);
            return ApiResponse<GetCartItemDto>.Success(existingResult);
        }

        var newItem = _mapper.Map<CartItems>(dto);
        newItem.UserId = userId;

        await _cartItemsRepository.AddAsync(newItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCartItemDto>(newItem);
        return ApiResponse<GetCartItemDto>.Success(result);
    }

    public async Task<ApiResponse<GetCartItemDto>> UpdateItemAsync(int userId, UpdateCartItemDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _cartItemsRepository.GetAsync(
            x => x.Id == dto.CartItemId && x.UserId == userId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (item is null)
            throw new NotFoundException(nameof(CartItems), dto.CartItemId);

        if (dto.Quantity <= 0)
        {
            await _cartItemsRepository.SoftDeleteAsync(item, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ApiResponse<GetCartItemDto>.Success(new GetCartItemDto());
        }

        item.Quantity = dto.Quantity;
        item.CouponId = dto.CouponId;
        item.OrderNumber = dto.OrderNumber;
        await _cartItemsRepository.UpdateAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCartItemDto>(item);
        return ApiResponse<GetCartItemDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> RemoveItemAsync(int userId, int cartItemId, CancellationToken cancellationToken = default)
    {
        var item = await _cartItemsRepository.GetAsync(
            x => x.Id == cartItemId && x.UserId == userId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (item is null)
            throw new NotFoundException(nameof(CartItems), cartItemId);

        await _cartItemsRepository.SoftDeleteAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> ApplyCouponAsync(int userId, ApplyCouponDto dto, CancellationToken cancellationToken = default)
    {
        var items = await _cartItemsRepository.GetAllAsync(
            x => x.UserId == userId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (!items.Any())
            throw new NotFoundException(nameof(CartItems), $"UserId: {userId}");

        var coupon = await _couponRepository.GetValidCouponByCodeAsync(dto.CouponCode, cancellationToken);

        if (coupon is null)
            throw new ValidationException([$"'{dto.CouponCode}' kodu geçerli veya aktif bir kupona ait değil."]);

        if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
            throw new ValidationException(["Bu kuponun kullanım limiti dolmuştur."]);

        foreach (var item in items)
        {
            item.CouponId = coupon.Id;
            await _cartItemsRepository.UpdateAsync(item, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> RemoveCouponAsync(int userId, CancellationToken cancellationToken = default)
    {
        var items = await _cartItemsRepository.GetAllAsync(
            x => x.UserId == userId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (!items.Any())
            throw new NotFoundException(nameof(CartItems), $"UserId: {userId}");

        foreach (var item in items)
        {
            item.CouponId = null;
            await _cartItemsRepository.UpdateAsync(item, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> ClearCartAsync(int userId, CancellationToken cancellationToken = default)
    {
        var items = await _cartItemsRepository.GetAllAsync(
            x => x.UserId == userId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (!items.Any())
            return ApiResponse<bool>.Success(true);

        foreach (var item in items)
            await _cartItemsRepository.SoftDeleteAsync(item, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<GetCartItemCountDto>> GetItemCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        var (totalQuantity, uniqueItemCount) = await _cartItemsRepository.GetItemCountByUserIdAsync(userId, cancellationToken);

        var result = new GetCartItemCountDto
        {
            UserId = userId,
            TotalQuantity = totalQuantity,
            UniqueItemCount = uniqueItemCount
        };

        return ApiResponse<GetCartItemCountDto>.Success(result);
    }
}
