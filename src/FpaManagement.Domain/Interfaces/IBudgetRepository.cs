using FpaManagement.Domain.Entities;

namespace FpaManagement.Domain.Interfaces
{
    public interface IBudgetRepository : IGenericRepository<Budget>
    {
        Task<Budget?> GetBudgetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Budget>> GetBudgetsByYearAsync(int year, CancellationToken cancellationToken = default);
        Task<IEnumerable<Budget>> GetBudgetsByStatusAsync(Domain.Enums.BudgetStatus status, CancellationToken cancellationToken = default);
        Task<BudgetVersion?> GetVersionWithItemsAsync(Guid versionId, CancellationToken cancellationToken = default);
        Task<bool> HasPendingApprovalsAsync(Guid budgetId, CancellationToken cancellationToken = default);
    }

}


