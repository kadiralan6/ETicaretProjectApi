using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.Payment.Domain.DTOs.PaymentTransactionDtos;

namespace ETicaretAPI.Services.Payment.Application.Services.PaymentTransactionService;

public interface IPaymentTransactionService
{
    Task<ApiResponse<PagedResult<GetPaymentTransactionDto>>> GetPaymentTransactionsFilterAsync(GetPaymentTransactionForAdminFilterDto filterDto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetPaymentTransactionDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetPaymentTransactionDto>> CreateAsync(CreatePaymentTransactionDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetPaymentTransactionDto>> ProcessPaymentAsync(ProcessPaymentDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<GetPaymentTransactionDto>> UpdateAsync(UpdatePaymentTransactionDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
