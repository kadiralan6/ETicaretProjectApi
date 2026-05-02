using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Basket.Application.Orders;
using ETicaretAPI.Services.Basket.Application.Predicates;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.DTOs.OrderDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Services.OrderService;

public class OrderManager : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderManager(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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