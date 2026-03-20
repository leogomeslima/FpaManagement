using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.Enums;
using FpaManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Infrastructure.Repositories;

public class BudgetRepository : GenericRepository<Budget>, IBudgetRepository
{
    public BudgetRepository(IApplicationDbContext context) : base(context)
    {
    }

    public async Task<Budget?> GetBudgetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Department)
            .Include(b => b.CostCenter)
            .Include(b => b.Versions.Where(v => !v.IsDeleted))
                .ThenInclude(v => v.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.CostCenter)
            .Include(b => b.Approvals.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.Approver)
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Budget>> GetBudgetsByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Department)
            .Include(b => b.CostCenter)
            .Where(b => b.FiscalYear == year && !b.IsDeleted)
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Budget>> GetBudgetsByStatusAsync(BudgetStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Department)
            .Include(b => b.CostCenter)
            .Where(b => b.Status == status && !b.IsDeleted)
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<BudgetVersion?> GetVersionWithItemsAsync(Guid versionId, CancellationToken cancellationToken = default)
    {
        return await _context.BudgetVersions
            .Include(v => v.Items.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.CostCenter)
            .FirstOrDefaultAsync(v => v.Id == versionId && !v.IsDeleted, cancellationToken);
    }

    public async Task<bool> HasPendingApprovalsAsync(Guid budgetId, CancellationToken cancellationToken = default)
    {
        return await _context.BudgetApprovals
            .AnyAsync(a => a.BudgetId == budgetId &&
                          a.Status == ApprovalStatus.Pending &&
                          !a.IsDeleted,
                   cancellationToken);
    }
}
