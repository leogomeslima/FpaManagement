using FpaManagement.Domain.Entities;

namespace FpaManagement.Domain.Interfaces;

public interface ICashFlowRepository : IGenericRepository<CashFlowEntry>
{
    Task<IEnumerable<CashFlowEntry>> GetEntriesByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<decimal> GetBalanceUntilAsync(DateTime date, CancellationToken cancellationToken = default);
}
