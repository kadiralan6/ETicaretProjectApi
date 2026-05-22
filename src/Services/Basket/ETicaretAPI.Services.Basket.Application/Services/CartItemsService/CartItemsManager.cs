using AutoMapper;
using ETicaretAPI.Common.Application.DTOs.CatalogDtos;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Infrastructure.ApiService;
using ETicaretAPI.Common.Infrastructure.Configuration;
using ETicaretAPI.Common.SharedLibrary.Enums.BasketEnums;
using ETicaretAPI.Services.Basket.Application.DTOs;
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
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRestApiService _restApiService;
    private readonly ICurrentUserService _currentUserService;
    private readonly string _catalogServiceUrl;

    public CartItemsManager(
        ICartItemsRepository cartItemsRepository,
        ICouponRepository couponRepository,
        ICampaignRepository campaignRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IRestApiService restApiService,
        ICurrentUserService currentUserService)
    {
        _cartItemsRepository = cartItemsRepository;
        _couponRepository = couponRepository;
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _restApiService = restApiService;
        _currentUserService = currentUserService;
        _catalogServiceUrl = CoreConfig.GetValue<string>("ExternalApi:CatalogApi:Url");
    }

    public async Task<ApiResponse<GetBasketDto>> GetBasketByUserIdAsync(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            return ApiResponse<GetBasketDto>.Fail("Kullanıcı kimliği doğrulanamadı.", 401);

        if (!int.TryParse(_currentUserService.UserId, out var userId))
            return ApiResponse<GetBasketDto>.Fail("Geçersiz kullanıcı kimliği.", 400);

        var cartItems = await _cartItemsRepository.GetByUserIdAsync(userId, cancellationToken);

        if (cartItems is null || cartItems.Count == 0)
            return ApiResponse<GetBasketDto>.Success(new GetBasketDto());

        var basketItems = new List<GetBasketItemDto>();

        foreach (var cartItem in cartItems)
        {
            var endpoint = $"{_catalogServiceUrl}/api/catalog/products/getById/{cartItem.ProductId}";
            var product = await _restApiService.GetDataResultAsync<GetCatalogProductDto>(endpoint);

            var basketItem = new GetBasketItemDto
            {
                CartItemId = cartItem.Id,
                UserId = cartItem.UserId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                CouponId = cartItem.CouponId,
                OrderNumber = cartItem.OrderNumber,
                ProductName = product?.Name,
                ProductCode = product?.Code,
                ProductSlug = product?.Slug,
                UnitPrice = product?.Price ?? 0m,
                StockQuantity = product?.StockQuantity ?? 0,
                IsActive = product?.IsActive ?? false,
                CategoryName = product?.CategoryName,
                BrandName = product?.BrandName,
                Images = product?.Images ?? []
            };

            basketItems.Add(basketItem);
        }

        var basket = await CalculateBasketSummaryAsync(basketItems, cartItems, cancellationToken);
        return ApiResponse<GetBasketDto>.Success(basket);
    }

    private async Task<GetBasketDto> CalculateBasketSummaryAsync(
        List<GetBasketItemDto> items,
        IReadOnlyList<CartItems> cartItems,
        CancellationToken cancellationToken)
    {
        var subTotal = items.Sum(x => x.LineTotal);
        var totalQuantity = items.Sum(x => x.Quantity);
        var uniqueItemCount = items.Count;

        var appliedCoupon = await ResolveAppliedCouponAsync(cartItems, subTotal, cancellationToken);
        var appliedCampaign = await ResolveAppliedCampaignAsync(subTotal, cancellationToken);

        var totalDiscount = (appliedCoupon?.DiscountAmount ?? 0m) + (appliedCampaign?.DiscountAmount ?? 0m);
        if (totalDiscount > subTotal)
            totalDiscount = subTotal;

        const decimal shippingCost = 0m;
        var total = subTotal - totalDiscount + shippingCost;

        return new GetBasketDto
        {
            Items = items,
            TotalQuantity = totalQuantity,
            UniqueItemCount = uniqueItemCount,
            SubTotal = decimal.Round(subTotal, 2),
            ShippingCost = shippingCost,
            AppliedCoupon = appliedCoupon,
            AppliedCampaign = appliedCampaign,
            TotalDiscount = decimal.Round(totalDiscount, 2),
            Total = decimal.Round(total, 2)
        };
    }

    private async Task<AppliedCouponDto?> ResolveAppliedCouponAsync(
        IReadOnlyList<CartItems> cartItems,
        decimal subTotal,
        CancellationToken cancellationToken)
    {
        var couponId = cartItems.FirstOrDefault(x => x.CouponId.HasValue)?.CouponId;
        if (couponId is null)
            return null;

        var coupon = await _couponRepository.GetWithAsNoTrackingAsync(
            x => x.Id == couponId.Value && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (coupon is null || !coupon.IsActive || coupon.ExpirationDate <= DateTime.UtcNow)
            return null;

        if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
            return null;

        if (coupon.MinimumOrderAmount.HasValue && subTotal < coupon.MinimumOrderAmount.Value)
            return null;

        var discount = CalculateDiscount(coupon.Type, coupon.DiscountValue, subTotal);

        return new AppliedCouponDto
        {
            Id = coupon.Id,
            Code = coupon.Code,
            Type = coupon.Type,
            DiscountValue = coupon.DiscountValue,
            MinimumOrderAmount = coupon.MinimumOrderAmount,
            ExpirationDate = coupon.ExpirationDate,
            DiscountAmount = decimal.Round(discount, 2)
        };
    }

    private async Task<AppliedCampaignDto?> ResolveAppliedCampaignAsync(
        decimal subTotal,
        CancellationToken cancellationToken)
    {
        var activeCampaigns = await _campaignRepository.GetActiveCampaignsOnDateAsync(DateTime.UtcNow, cancellationToken);
        if (activeCampaigns is null || activeCampaigns.Count == 0)
            return null;

        var eligible = activeCampaigns
            .Where(c => !c.MinimumOrderAmount.HasValue || subTotal >= c.MinimumOrderAmount.Value)
            .Where(c => !c.UsageLimit.HasValue || c.UsageCount < c.UsageLimit.Value)
            .Select(c => new
            {
                Campaign = c,
                Discount = CalculateDiscount(c.Type, c.DiscountValue, subTotal)
            })
            .OrderByDescending(x => x.Discount)
            .FirstOrDefault();

        if (eligible is null || eligible.Discount <= 0m)
            return null;

        return new AppliedCampaignDto
        {
            Id = eligible.Campaign.Id,
            Name = eligible.Campaign.Name,
            Type = eligible.Campaign.Type,
            DiscountValue = eligible.Campaign.DiscountValue,
            MinimumOrderAmount = eligible.Campaign.MinimumOrderAmount,
            StartDate = eligible.Campaign.StartDate,
            EndDate = eligible.Campaign.EndDate,
            DiscountAmount = decimal.Round(eligible.Discount, 2)
        };
    }

    private static decimal CalculateDiscount(CampaignTypeCommonEnum type, decimal value, decimal subTotal)
    {
        if (subTotal <= 0m || value <= 0m)
            return 0m;

        return type switch
        {
            CampaignTypeCommonEnum.Percentage => Math.Min(subTotal * value / 100m, subTotal),
            CampaignTypeCommonEnum.FixedAmount => Math.Min(value, subTotal),
            _ => 0m
        };
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

    public async Task<ApiResponse<GetCartItemDto>> AddItemAsync(AddCartItemDto dto, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            return ApiResponse<GetCartItemDto>.Fail("Kullanıcı kimliği doğrulanamadı.", 401);

        if (!int.TryParse(_currentUserService.UserId, out var userId))
            return ApiResponse<GetCartItemDto>.Fail("Geçersiz kullanıcı kimliği.", 400);

        var existingItem = await _cartItemsRepository.GetByUserAndProductAsync(userId, dto.ProductId, cancellationToken);

        if (existingItem is not null)
        {
            existingItem.Quantity += dto.Quantity;
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

    public async Task<ApiResponse<GetCartItemDto>> UpdateItemAsync(UpdateCartItemDto dto, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(_currentUserService.UserId, out var userId))
            return ApiResponse<GetCartItemDto>.Fail("Geçersiz kullanıcı kimliği.", 400);

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
        await _cartItemsRepository.UpdateAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetCartItemDto>(item);
        return ApiResponse<GetCartItemDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> RemoveItemAsync(int cartItemId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(_currentUserService.UserId, out var userId))
            return ApiResponse<bool>.Fail("Geçersiz kullanıcı kimliği.", 400);

        var item = await _cartItemsRepository.GetAsync(
            x => x.Id == cartItemId && x.UserId == userId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (item is null)
            throw new NotFoundException(nameof(CartItems), cartItemId);

        await _cartItemsRepository.SoftDeleteAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> ApplyCouponAsync(ApplyCouponDto dto, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(_currentUserService.UserId, out var userId))
            return ApiResponse<bool>.Fail("Geçersiz kullanıcı kimliği.", 400);

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

    public async Task<ApiResponse<bool>> RemoveCouponAsync(int cartItemId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(_currentUserService.UserId, out var userId))
            return ApiResponse<bool>.Fail("Geçersiz kullanıcı kimliği.", 400);

        var item = await _cartItemsRepository.GetAsync(
            x => x.Id == cartItemId && x.UserId == userId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (item is null)
            throw new NotFoundException(nameof(CartItems), cartItemId);

        if (!item.CouponId.HasValue)
            throw new ValidationException(["Bu sepet ürününe uygulanmış bir kupon bulunmuyor."]);

        item.CouponId = null;
        await _cartItemsRepository.UpdateAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> ClearCartAsync(CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(_currentUserService.UserId, out var userId))
            return ApiResponse<bool>.Fail("Geçersiz kullanıcı kimliği.", 400);

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

    public async Task<ApiResponse<GetCartItemCountDto>> GetItemCountAsync(CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(_currentUserService.UserId, out var userId))
            return ApiResponse<GetCartItemCountDto>.Fail("Geçersiz kullanıcı kimliği.", 400);

        var totalQuantity = await _cartItemsRepository.CountAsync(
                    x => x.UserId == userId && !x.IsDeleted,
                    cancellationToken: cancellationToken);

        var uniqueItemCount = await _cartItemsRepository.CountAsync(
            x => x.UserId == userId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        var result = new GetCartItemCountDto
        {
            UserId = userId,
            TotalQuantity = totalQuantity,
            UniqueItemCount = uniqueItemCount
        };

        return ApiResponse<GetCartItemCountDto>.Success(result);
    }
}
