using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Payment.Application.Orders;
using ETicaretAPI.Services.Payment.Application.Predicates;
using ETicaretAPI.Services.Payment.Application.Repositories;
using ETicaretAPI.Services.Payment.Domain.DTOs.PaymentTransactionDtos;
using ETicaretAPI.Services.Payment.Domain.Entities;
using ETicaretAPI.Services.Payment.Domain.Enums;

namespace ETicaretAPI.Services.Payment.Application.Services.PaymentTransactionService;

public class PaymentTransactionManager : IPaymentTransactionService
{
    private readonly IPaymentTransactionRepository _paymentTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentTransactionManager(
        IPaymentTransactionRepository paymentTransactionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _paymentTransactionRepository = paymentTransactionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<GetPaymentTransactionDto>>> GetPaymentTransactionsFilterAsync(GetPaymentTransactionForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var predicate = PaymentTransactionPredicate.GetExpression(filterDto);
        var orders = PaymentTransactionOrder.GetOrder(filterDto.OrderBy, filterDto.OrderType);

        var transactions = await _paymentTransactionRepository.GetAllAsNoTrackingWithPaginationAsync(
            filterDto.Page, filterDto.PageSize, predicate, orders, cancellationToken: cancellationToken);

        var result = _mapper.Map<PagedResult<GetPaymentTransactionDto>>(transactions);
        return ApiResponse<PagedResult<GetPaymentTransactionDto>>.Success(result);
    }

    public async Task<ApiResponse<GetPaymentTransactionDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var transaction = await _paymentTransactionRepository.GetWithAsNoTrackingAsync(
            x => x.Id == id, cancellationToken: cancellationToken);

        if (transaction is null)
            throw new NotFoundException(nameof(PaymentTransaction), id);

        var result = _mapper.Map<GetPaymentTransactionDto>(transaction);
        return ApiResponse<GetPaymentTransactionDto>.Success(result);
    }

    public async Task<ApiResponse<GetPaymentTransactionDto>> CreateAsync(CreatePaymentTransactionDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.TransactionId))
            throw new ValidationException(["TransactionId boş olamaz."]);

        if (await _paymentTransactionRepository.IsTransactionIdExistsAsync(dto.TransactionId, cancellationToken: cancellationToken))
            throw new ValidationException([$"'{dto.TransactionId}' transaction id zaten kullanılmış."]);

        var transaction = _mapper.Map<PaymentTransaction>(dto);
        transaction.Status = PaymentStatusEnum.Pending;

        await _paymentTransactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetPaymentTransactionDto>(transaction);
        return ApiResponse<GetPaymentTransactionDto>.Success(result);
    }

    public async Task<ApiResponse<GetPaymentTransactionDto>> ProcessPaymentAsync(ProcessPaymentDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
            throw new ValidationException(["UserId boş olamaz."]);

        if (dto.Items is null || dto.Items.Count == 0)
            throw new ValidationException(["Ödeme için en az bir ürün gerekli."]);

        var transaction = new PaymentTransaction
        {
            OrderId = dto.OrderId,
            UserId = dto.UserId,
            TransactionId = Guid.NewGuid().ToString("N"),
            Currency = string.IsNullOrWhiteSpace(dto.Currency) ? "TRY" : dto.Currency,
            Method = dto.PaymentMethod,
            Status = PaymentStatusEnum.Pending,
            Amount = dto.Amount,
            Details = dto.Items.Select(item => new PaymentDetail
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                AddressId = dto.AddressId,
                CardNumber = dto.CardInfo?.CardNumber ?? string.Empty,
                ExpiryMonth = dto.CardInfo?.ExpiryMonth ?? string.Empty,
                ExpiryYear = dto.CardInfo?.ExpiryYear ?? string.Empty,
                Cvv = dto.CardInfo?.Cvv ?? string.Empty,
                CardHolderName = dto.CardInfo?.CardHolderName ?? string.Empty
            }).ToList()
        };

        await _paymentTransactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetPaymentTransactionDto>(transaction);
        return ApiResponse<GetPaymentTransactionDto>.Success(result);
    }

    public async Task<ApiResponse<GetPaymentTransactionDto>> UpdateAsync(UpdatePaymentTransactionDto dto, CancellationToken cancellationToken = default)
    {
        var transaction = await _paymentTransactionRepository.GetAsync(x => x.Id == dto.Id, cancellationToken: cancellationToken);

        if (transaction is null)
            throw new NotFoundException(nameof(PaymentTransaction), dto.Id);

        _mapper.Map(dto, transaction);

        if (dto.Status == PaymentStatusEnum.Completed && transaction.CompletedDate is null)
            transaction.CompletedDate = DateTime.UtcNow;

        await _paymentTransactionRepository.UpdateAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<GetPaymentTransactionDto>(transaction);
        return ApiResponse<GetPaymentTransactionDto>.Success(result);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var transaction = await _paymentTransactionRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (transaction is null)
            throw new NotFoundException(nameof(PaymentTransaction), id);

        await _paymentTransactionRepository.SoftDeleteAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success(true);
    }
}
