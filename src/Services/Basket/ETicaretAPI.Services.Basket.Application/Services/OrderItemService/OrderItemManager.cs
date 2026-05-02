using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Basket.Application.Orders;
using ETicaretAPI.Services.Basket.Application.Predicates;
using ETicaretAPI.Services.Basket.Application.Repositories;
using ETicaretAPI.Services.Basket.Domain.DTOs.OrderItemDtos;
using ETicaretAPI.Services.Basket.Domain.Entities;

namespace ETicaretAPI.Services.Basket.Application.Services.OrderItemService;

public class OrderItemManager : IOrderItemService
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderItemManager(
        IOrderItemRepository orderItemRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _orderItemRepository = orderItemRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<GetOrderItemDto>>> GetOrderItemsFilterAsync(
        GetOrderItemForAdminFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        var predicate = OrderItemPredicate.GetExpression(filterDto);
        var orders = OrderItemOrder.GetOrder(filterDto.OrderBy, filterDto.OrderType);

        var entities = await _orderItemRepository.GetAllAsNoTrackingWithPaginationAsync(
            filterDto.Page, filterDto.PageSize, predicate, orders, cancellationToken: cancellationToken);

        var result = _mapper.Map<PagedResult<GetOrderItemDto>>(entities);
        return ApiResponse<PagedResult<GetOrderItemDto>>.Success(result);
    }

    public async Task<ApiResponse<GetOrderItemDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var orderItem = await _orderItemRepository.GetWithAsNoTrackingAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken);

        if (orderItem is null)
            throw new NotFoundException(nameof(OrderItem), id);

        var result = _mapper.Map<GetOrderItemDto>(orderItem);
        return ApiResponse<GetOrderItemDto>.Success(result);
    }

    public async Task<ApiResponse<GetOrderItemDto>> CreateAsync(CreateOrderItemDto dto, CancellationToken cancellationToken = default)
    {
        var orderItem = _mapper.Map<OrderItem>(dto);

        await _orderItemRepository.AddAsync(orderItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetOrderItemDto>(orderItem);
        return ApiResponse<GetOrderItemDto>.Success(result);
    }

    public async Task<ApiResponse<GetOrderItemDto>> UpdateAsync(UpdateOrderItemDto dto, CancellationToken cancellationToken = default)
    {
        var orderItem = await _orderItemRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: cancellationToken);

        if (orderItem is null)
            throw new NotFoundException(nameof(OrderItem), dto.Id);

        _mapper.Map(dto, orderItem);

        await _orderItemRepository.UpdateAsync(orderItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetOrderItemDto>(orderItem);
        return ApiResponse<GetOrderItemDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var orderItem = await _orderItemRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (orderItem is null)
            throw new NotFoundException(nameof(OrderItem), id);

        await _orderItemRepository.SoftDeleteAsync(orderItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }
}