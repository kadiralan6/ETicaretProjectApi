using ETicaretAPI.Services.Payment.Domain.Enums;

namespace ETicaretAPI.Services.Payment.Domain.DTOs.PaymentTransactionDtos;

public class GetPaymentTransactionForAdminFilterDto : BaseFilterDto<PaymentTransactionOrderByEnum>
{
    public int? OrderId { get; set; }
    public string? UserId { get; set; }
    public string? TransactionId { get; set; }
    public string? Currency { get; set; }
    public PaymentMethodEnum? Method { get; set; }
    public PaymentStatusEnum? Status { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}
