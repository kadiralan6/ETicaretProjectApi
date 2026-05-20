using AutoMapper;
using ETicaretAPI.Common.Application.DTOs.CatalogDtos;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Infrastructure.ApiService;
using ETicaretAPI.Common.Infrastructure.Configuration;
using ETicaretAPI.Services.Basket.Application.Orders;
using ETicaretAPI.Services.Basket.Application.Predicates;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Domain.Enums;

namespace ETicaretAPI.Services.Basket.Application.Services.OrderService;

public class OrderManager : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly ICartItemsRepository _cartItemsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRestApiService _restApiService;
    private readonly string _paymentServiceUrl;
    private readonly string _catalogServiceUrl;
    private readonly string _identityServiceUrl;

    public OrderManager(
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        ICartItemsRepository cartItemsRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IRestApiService restApiService)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _cartItemsRepository = cartItemsRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _restApiService = restApiService;
        _paymentServiceUrl = CoreConfig.GetValue<string>("ExternalApi:PaymentApi:Url");
        _catalogServiceUrl = CoreConfig.GetValue<string>("ExternalApi:CatalogApi:Url");
        _identityServiceUrl = CoreConfig.GetValue<string>("ExternalApi:IdentityApi:Url");
    }

    public async Task<ApiResponse<PagedResult<GetOrderDto>>> GetOrdersFilterAsync(
        GetOrderForAdminFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        var predicate = OrderPredicate.GetExpression(filterDto);
        var orders = OrderOrder.GetOrder(filterDto.OrderBy, filterDto.OrderType);

        var entities = await _orderRepository.GetAllAsNoTrackingWithPaginationAsync(
            filterDto.Page, filterDto.PageSize, predicate, orders, cancellationToken: cancellationToken);

        var result = _mapper.Map<PagedResult<GetOrderDto>>(entities);
        return ApiResponse<PagedResult<GetOrderDto>>.Success(result);
    }

    public async Task<ApiResponse<GetOrderDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithAsNoTrackingAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken);

        if (order is null)
            throw new NotFoundException(nameof(Order), id);

        var result = _mapper.Map<GetOrderDto>(order);
        return ApiResponse<GetOrderDto>.Success(result);
    }

    public async Task<ApiResponse<GetOrderDto>> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var order = _mapper.Map<Order>(dto);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetOrderDto>(order);
        return ApiResponse<GetOrderDto>.Success(result);
    }

    public async Task<ApiResponse<GetOrderDto>> UpdateAsync(UpdateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: cancellationToken);

        if (order is null)
            throw new NotFoundException(nameof(Order), dto.Id);

        _mapper.Map(dto, order);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetOrderDto>(order);
        return ApiResponse<GetOrderDto>.Success(result);
    }

    public async Task<ApiResponse<PlaceOrderResultDto>> PlaceOrderAsync(PlaceOrderDto dto, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
            return ApiResponse<PlaceOrderResultDto>.Fail("Kullanıcı kimliği doğrulanamadı.", 401);

        if (dto.Items is null || dto.Items.Count == 0)
            throw new ValidationException(["Sipariş için en az bir ürün gerekli."]);

        if (!int.TryParse(_currentUserService.UserId, out var userId))
            return ApiResponse<PlaceOrderResultDto>.Fail("Geçersiz kullanıcı kimliği.", 400);

        const decimal vatRate = 0.20m;
        var orderNumber = $"ORD-{Guid.NewGuid():N}"[..20];

        var order = new Order
        {
            UserId = userId,
            ProductId = dto.Items.First().ProductId,
            TotalPrice = dto.Amount,
            Price = dto.Amount,
            Quantity = dto.Items.Sum(x => x.Quantity),
            OrderNumber = orderNumber,
            AddressId = dto.AddressId > 0 ? dto.AddressId : null,
            Status = OrderStatusEnum.Pending
        };

        await _orderRepository.AddAsync(order, cancellationToken);

        var orderItems = new List<OrderItem>();
        foreach (var item in dto.Items)
        {
            var productEndpoint = $"{_catalogServiceUrl}/api/catalog/products/getById/{item.ProductId}";
            var product = await _restApiService.GetDataResultAsync<GetCatalogProductDto>(productEndpoint);

            if (product is null)
                throw new NotFoundException(nameof(GetCatalogProductDto), item.ProductId);

            var unitPrice = product.Price;
            var lineTotal = unitPrice * item.Quantity;
            var vatAmount = decimal.Round(lineTotal * vatRate / (1m + vatRate), 2);

            orderItems.Add(new OrderItem
            {
                UserId = userId,
                ProductId = item.ProductId,
                Price = unitPrice,
                TotalPrice = decimal.Round(lineTotal, 2),
                TotalNetPrice = decimal.Round(lineTotal, 2),
                VatAmount = vatAmount,
                Quantity = item.Quantity,
                OrderNumber = orderNumber
            });
        }

        await _orderItemRepository.AddRangeAsync(orderItems, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var paymentRequest = new
        {
            userId = _currentUserService.UserId,
            orderId = order.Id,
            addressId = dto.AddressId,
            paymentMethod = (int)dto.PaymentMethod,
            cardInfo = dto.CardInfo,
            amount = dto.Amount,
            currency = string.IsNullOrWhiteSpace(dto.Currency) ? "TRY" : dto.Currency,
            items = dto.Items.Select(x => new { productId = x.ProductId, quantity = x.Quantity })
        };

        var endpoint = $"{_paymentServiceUrl}/api/payment/paymentTransactions/process";
        var paymentResponse = await _restApiService.PostDataResultAsync<PaymentTransactionResponseDto>(endpoint, paymentRequest);

        if (paymentResponse is null || paymentResponse.Id == 0)
        {
            order.Status = OrderStatusEnum.PaymentFailed;
            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ApiResponse<PlaceOrderResultDto>.Fail("Ödeme servisi yanıt vermedi.", 502);
        }

        order.Status = OrderStatusEnum.Confirmed;
        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var cartItems = await _cartItemsRepository.GetAllAsync(
            x => x.UserId == userId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (cartItems.Any())
        {
            foreach (var item in cartItems)
                await _cartItemsRepository.SoftDeleteAsync(item, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var result = new PlaceOrderResultDto
        {
            OrderId = order.Id,
            OrderStatus = order.Status,
            PaymentTransactionId = paymentResponse.Id,
            TransactionId = paymentResponse.TransactionId,
            PaymentStatus = paymentResponse.Status,
            Amount = paymentResponse.Amount,
            Currency = paymentResponse.Currency
        };

        return ApiResponse<PlaceOrderResultDto>.Success(result);
    }

    public async Task<ApiResponse<List<GetMyOrderSummaryDto>>> GetMyOrdersAsync(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
            return ApiResponse<List<GetMyOrderSummaryDto>>.Fail("Kullanıcı kimliği doğrulanamadı.", 401);

        if (!int.TryParse(_currentUserService.UserId, out var userId))
            return ApiResponse<List<GetMyOrderSummaryDto>>.Fail("Geçersiz kullanıcı kimliği.", 400);

        var orders = await _orderRepository.GetAllAsync(
            x => x.UserId == userId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        var summaries = new List<GetMyOrderSummaryDto>();

        foreach (var order in orders.OrderByDescending(x => x.CreatedAt))
        {
            string? productName = null;
            try
            {
                var productEndpoint = $"{_catalogServiceUrl}/api/catalog/products/getById/{order.ProductId}";
                var product = await _restApiService.GetDataResultAsync<GetCatalogProductDto>(productEndpoint);
                productName = product?.Name;
            }
            catch { }

            summaries.Add(new GetMyOrderSummaryDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                ProductName = productName,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            });
        }

        return ApiResponse<List<GetMyOrderSummaryDto>>.Success(summaries);
    }

    public async Task<ApiResponse<GetOrderDetailDto>> GetOrderDetailAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
            return ApiResponse<GetOrderDetailDto>.Fail("Kullanıcı kimliği doğrulanamadı.", 401);

        if (!int.TryParse(_currentUserService.UserId, out var userId))
            return ApiResponse<GetOrderDetailDto>.Fail("Geçersiz kullanıcı kimliği.", 400);

        var order = await _orderRepository.GetWithAsNoTrackingAsync(
            x => x.OrderNumber == orderNumber && x.UserId == userId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (order is null)
            throw new NotFoundException(nameof(Order), orderNumber);

        var orderItems = await _orderItemRepository.GetAllAsync(x => x.UserId == userId && !x.IsDeleted,cancellationToken: cancellationToken);

        var detailItems = new List<GetOrderDetailItemDto>();

        foreach (var item in orderItems)
        {
            GetCatalogProductDto? product = null;
            try
            {
                var productEndpoint = $"{_catalogServiceUrl}/api/catalog/products/getById/{item.ProductId}";
                product = await _restApiService.GetDataResultAsync<GetCatalogProductDto>(productEndpoint);
            }
            catch { }

            detailItems.Add(new GetOrderDetailItemDto
            {
                ProductId = item.ProductId,
                ProductName = product?.Name,
                BrandName = product?.BrandName,
                CategoryName = product?.CategoryName,
                Images = product?.Images?.Select(img => new OrderProductImageDto
                {
                    Id = img.Id,
                    Url = img.Url,
                    IsCover = img.IsCover,
                    AltText = img.AltText
                }).ToList() ?? [],
                Quantity = item.Quantity,
                Price = item.Price,
                TotalNetPrice = item.TotalNetPrice
            });
        }

        OrderAddressDto? address = null;
        if (order.AddressId.HasValue)
        {
            try
            {
                var addressEndpoint = $"{_identityServiceUrl}/api/identity/addresses/getById/{order.AddressId.Value}";
                var addressResponse = await _restApiService.GetDataResultAsync<OrderAddressDto>(addressEndpoint);
                address = addressResponse;
            }
            catch { }
        }

        OrderBuyerDto? buyer = null;
        try
        {
            var userEndpoint = $"{_identityServiceUrl}/api/identity/users/getById/{userId}";
            var userResponse = await _restApiService.GetDataResultAsync<OrderBuyerDto>(userEndpoint);
            buyer = userResponse;
            if (buyer is not null)
                buyer.UserId = userId;
        }
        catch { }

        var detail = new GetOrderDetailDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt,
            Items = detailItems,
            Address = address,
            Buyer = buyer
        };

        return ApiResponse<GetOrderDetailDto>.Success(detail);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (order is null)
            throw new NotFoundException(nameof(Order), id);

        await _orderRepository.SoftDeleteAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }
}
