using ETicaretAPI.Common.Domain.Entities;
using ETicaretAPI.Services.Payment.Domain.Enums;

namespace ETicaretAPI.Services.Payment.Domain.Entities;


public class PaymentTransaction : Entity<int>
{
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string Currency { get; set; } = "TRY";
    public PaymentMethodEnum Method { get; set; }
    public PaymentStatusEnum Status { get; set; } = PaymentStatusEnum.Pending;
    public decimal Amount { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? CompletedDate { get; set; }

    public ICollection<PaymentDetail> Details { get; set; } = [];
}

