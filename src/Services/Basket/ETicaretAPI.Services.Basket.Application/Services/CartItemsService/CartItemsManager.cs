using AutoMapper;
using ETicaretAPI.Common.Application.DTOs.CatalogDtos;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Infrastructure.ApiService;
using ETicaretAPI.Common.Infrastructure.Configuration;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRestApiService _restApiService;
    private readonly ICurrentUserService _currentUserService;
    private readonly string _catalogServiceUrl;

    public CartItemsManager(
        ICartItemsRepository cartItemsRepository,
        ICouponRepository couponRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IRestApiService restApiService,
        ICurrentUserService currentUserService)
    {
        _cartItemsRepository = cartItemsRepository;
        _couponRepository = couponRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _restApiService = restApiService;
        _currentUserService = currentUserService;
        _catalogServiceUrl = CoreConfig.GetValue<string>("ExternalApi:CatalogApi:Url");
    }

    public async Task<ApiResponse<List<GetBasketItemDto>>> GetBasketByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var cartItems = await _cartItemsRepository.GetByUserIdAsync(userId, cancellationToken);

        if (cartItems is null || cartItems.Count == 0)
            return ApiResponse<List<GetBasketItemDto>>.Success([]);

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

        return ApiResponse<List<GetBasketItemDto>>.Success(basketItems);
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
