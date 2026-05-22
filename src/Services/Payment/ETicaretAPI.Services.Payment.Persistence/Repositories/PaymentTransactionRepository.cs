using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Payment.Application.Repositories;
using ETicaretAPI.Services.Payment.Domain.Entities;
using ETicaretAPI.Services.Payment.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Payment.Persistence.Repositories;

public class PaymentTransactionRepository : EfEntityRepositoryBase<PaymentTransaction, PaymentDbContext>, IPaymentTransactionRepository
{
    public PaymentTransactionRepository(PaymentDbContext context) : base(context)
    {
    }

    public async Task<bool> IsTransactionIdExistsAsync(string transactionId, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.PaymentTransactions
            .AsNoTracking()
            .Where(x => x.TransactionId == transactionId && !x.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(x => x.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}
