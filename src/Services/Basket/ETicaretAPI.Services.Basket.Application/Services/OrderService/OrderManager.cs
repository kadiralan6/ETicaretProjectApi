using AutoMapper;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRestApiService _restApiService;
    private readonly string _paymentServiceUrl;

    public OrderManager(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IRestApiService restApiService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _restApiService = restApiService;
        _paymentServiceUrl = CoreConfig.GetValue<string>("ExternalApi:PaymentApi:Url");
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

        var order = new Order
        {
            UserId = userId,
            ProductId = dto.Items.First().ProductId,
            TotalPrice = dto.Amount,
            Price = dto.Amount,
            Quantity = dto.Items.Sum(x => x.Quantity),
            OrderNumber = $"ORD-{Guid.NewGuid():N}"[..20],
            Status = OrderStatusEnum.Pending
        };

        await _orderRepository.AddAsync(order, cancellationToken);
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