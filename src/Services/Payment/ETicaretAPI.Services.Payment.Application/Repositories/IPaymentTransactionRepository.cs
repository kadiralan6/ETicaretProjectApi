using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Payment.Domain.Entities;

namespace ETicaretAPI.Services.Payment.Application.Repositories;

public interface IPaymentTransactionRepository : IEntityRepository<PaymentTransaction>
{
    Task<bool> IsTransactionIdExistsAsync(string transactionId, int? excludeId = null, CancellationToken cancellationToken = default);
}
