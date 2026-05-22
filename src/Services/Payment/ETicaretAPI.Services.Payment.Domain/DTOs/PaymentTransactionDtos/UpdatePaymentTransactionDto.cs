using ETicaretAPI.Common.Domain.Entities;
using ETicaretAPI.Services.Payment.Domain.Enums;

namespace ETicaretAPI.Services.Payment.Domain.DTOs.PaymentTransactionDtos;

public class UpdatePaymentTransactionDto : BaseEmptyDto
{
    public int Id { get; set; }
    public PaymentStatusEnum Status { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? CompletedDate { get; set; }
}
