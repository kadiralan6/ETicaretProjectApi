using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.SharedLibrary.Enums.BasketEnums;
using ETicaretAPI.Services.Basket.Application.Orders;
using ETicaretAPI.Services.Basket.Application.Predicates;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.DTOs.CartDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Services.CartService;

public class CartManager : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartManager(
        ICartRepository cartRepository,
        ICartItemRepository cartItemRepository,
        ICouponRepository couponRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _couponRepository = couponRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<GetCartDto>> GetOrCreateCartAsync(int userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart { UserId = userId };
            await _cartRepository.AddAsync(cart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var result = _mapper.Map<GetCartDto>(cart);
        return ApiResponse<GetCartDto>.Success(result);
    }

    public async Task<ApiResponse<GetCartDto>> GetByIdAsync(int cartId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetCartWithItemsAsync(cartId, cancellationToken);

        if (cart is null)
            throw new NotFoundException(nameof(Cart), cartId);

        var result = _mapper.Map<GetCartDto>(cart);
        return ApiResponse<GetCartDto>.Success(result);
    }

    public async Task<ApiResponse<PagedResult<GetCartDto>>> GetCartsFilterAsync(GetCartForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var predicate = CartPredicate.GetExpression(filterDto);
        var orders = CartOrder.GetOrder(filterDto.OrderBy, filterDto.OrderType);

        var carts = await _cartRepository.GetAllAsNoTrackingWithPaginationAsync(
            filterDto.Page, filterDto.PageSize, predicate, orders, cancellationToken: cancellationToken);

        var result = _mapper.Map<PagedResult<GetCartDto>>(carts);
        return ApiResponse<PagedResult<GetCartDto>>.Success(result);
    }

    public async Task<ApiResponse<GetCartDto>> AddItemAsync(int userId, AddCartItemDto dto, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart { UserId = userId };
            await _cartRepository.AddAsync(cart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

        if (existingItem is not null)
        {
            existingItem.Quantity += dto.Quantity;
            await _cartItemRepository.UpdateFieldAsync(existingItem, [x => x.Quantity], cancellationToken);
        }
        else
        {
            var newItem = _mapper.Map<CartItem>(dto);
            newItem.CartId = cart.Id;
            await _cartItemRepository.AddAsync(newItem, cancellationToken);
            cart.Items.Add(newItem);
        }

        RecalculateTotals(cart);
        await _cartRepository.UpdateFieldAsync(cart, [x => x.Subtotal, x => x.DiscountAmount, x => x.Total], cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCartDto>(cart);
        return ApiResponse<GetCartDto>.Success(result);
    }

    public async Task<ApiResponse<GetCartDto>> UpdateItemAsync(int userId, UpdateCartItemDto dto, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId, cancellationToken);

        if (cart is null)
            throw new NotFoundException(nameof(Cart), $"UserId: {userId}");

        var item = cart.Items.FirstOrDefault(i => i.Id == dto.CartItemId);

        if (item is null)
            throw new NotFoundException(nameof(CartItem), dto.CartItemId);

        if (dto.Quantity <= 0)
        {
            await _cartItemRepository.SoftDeleteAsync(item, cancellationToken);
            cart.Items.Remove(item);
        }
        else
        {
            item.Quantity = dto.Quantity;
            await _cartItemRepository.UpdateFieldAsync(item, [x => x.Quantity], cancellationToken);
        }

        RecalculateTotals(cart);
        await _cartRepository.UpdateFieldAsync(cart, [x => x.Subtotal, x => x.DiscountAmount, x => x.Total], cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCartDto>(cart);
        return ApiResponse<GetCartDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> RemoveItemAsync(int userId, int cartItemId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId, cancellationToken);

        if (cart is null)
            throw new NotFoundException(nameof(Cart), $"UserId: {userId}");

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

        if (item is null)
            throw new NotFoundException(nameof(CartItem), cartItemId);

        await _cartItemRepository.SoftDeleteAsync(item, cancellationToken);
        cart.Items.Remove(item);

        RecalculateTotals(cart);
        await _cartRepository.UpdateFieldAsync(cart, [x => x.Subtotal, x => x.DiscountAmount, x => x.Total], cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<GetCartDto>> ApplyCouponAsync(int userId, ApplyCouponDto dto, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId, cancellationToken);

        if (cart is null)
            throw new NotFoundException(nameof(Cart), $"UserId: {userId}");

        if (cart.Id != dto.CartId)
            throw new ValidationException(["Belirtilen sepet bu kullanıcıya ait değil."]);

        var coupon = await _couponRepository.GetValidCouponByCodeAsync(dto.CouponCode, cancellationToken);

        if (coupon is null)
            throw new ValidationException([$"'{dto.CouponCode}' kodu geçerli veya aktif bir kupona ait değil."]);

        if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
            throw new ValidationException(["Bu kuponun kullanım limiti dolmuştur."]);

        cart.CouponId = coupon.Id;
        cart.Coupon = coupon;

        RecalculateTotals(cart);
        await _cartRepository.UpdateFieldAsync(cart, [x => x.CouponId, x => x.DiscountAmount, x => x.Total], cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCartDto>(cart);
        return ApiResponse<GetCartDto>.Success(result);
    }

    public async Task<ApiResponse<GetCartDto>> RemoveCouponAsync(int userId, int cartId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId, cancellationToken);

        if (cart is null)
            throw new NotFoundException(nameof(Cart), $"UserId: {userId}");

        if (cart.Id != cartId)
            throw new ValidationException(["Belirtilen sepet bu kullanıcıya ait değil."]);

        cart.CouponId = null;
        cart.Coupon = null;
        cart.DiscountAmount = 0;

        RecalculateTotals(cart);
        await _cartRepository.UpdateFieldAsync(cart, [x => x.CouponId, x => x.DiscountAmount, x => x.Total], cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCartDto>(cart);
        return ApiResponse<GetCartDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> ClearCartAsync(int userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId, cancellationToken);

        if (cart is null)
            throw new NotFoundException(nameof(Cart), $"UserId: {userId}");

        foreach (var item in cart.Items)
            await _cartItemRepository.SoftDeleteAsync(item, cancellationToken);

        cart.CouponId = null;
        cart.Subtotal = 0;
        cart.DiscountAmount = 0;
        cart.Total = 0;

        await _cartRepository.UpdateFieldAsync(cart, [x => x.CouponId, x => x.Subtotal, x => x.DiscountAmount, x => x.Total], cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    // -------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------

    private static void RecalculateTotals(Cart cart)
    {
        cart.Subtotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);

        cart.DiscountAmount = cart.Coupon is not null
            ? CalculateDiscount(cart.Subtotal, cart.Coupon)
            : 0;

        cart.Total = cart.Subtotal - cart.DiscountAmount;
        if (cart.Total < 0) cart.Total = 0;
    }

    private static decimal CalculateDiscount(decimal subtotal, Coupon coupon)
    {
        if (coupon.MinimumOrderAmount.HasValue && subtotal < coupon.MinimumOrderAmount.Value)
            return 0;

        return coupon.Type switch
        {
            CampaignTypeCommonEnum.Percentage => Math.Round(subtotal * coupon.DiscountValue / 100, 2),
            CampaignTypeCommonEnum.FixedAmount => Math.Min(coupon.DiscountValue, subtotal),
            _ => 0
        };
    }
}
