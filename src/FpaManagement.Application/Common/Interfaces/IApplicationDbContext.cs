using Microsoft.EntityFrameworkCore;
using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Department> Departments { get; }
    DbSet<CostCenter> CostCenters { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<Budget> Budgets { get; }
    DbSet<BudgetVersion> BudgetVersions { get; }
    DbSet<BudgetItem> BudgetItems { get; }
    DbSet<BudgetApproval> BudgetApprovals { get; }
    DbSet<Forecast> Forecasts { get; }
    DbSet<ForecastVersion> ForecastVersions { get; }
    DbSet<ForecastItem> ForecastItems { get; }
    DbSet<ForecastJustification> ForecastJustifications { get; }
    DbSet<CashFlowEntry> CashFlowEntries { get; }
    DbSet<CashFlowCategory> CashFlowCategories { get; }
    DbSet<CashFlowProjection> CashFlowProjections { get; }
    DbSet<DfcEntry> DfcEntries { get; }
    DbSet<DfcCategory> DfcCategories { get; }
    DbSet<Revenue> Revenues { get; }
    DbSet<Expense> Expenses { get; }
    DbSet<FinancialCategory> FinancialCategories { get; }
    DbSet<RecurringPattern> RecurringPatterns { get; }
    DbSet<DemandEntry> DemandEntries { get; }
    DbSet<DemandPattern> DemandPatterns { get; }
    DbSet<SeasonalityFactor> SeasonalityFactors { get; }
    DbSet<Scenario> Scenarios { get; }
    DbSet<ScenarioAssumption> ScenarioAssumptions { get; }
    DbSet<ScenarioResult> ScenarioResults { get; }
    DbSet<KpiDefinition> KpiDefinitions { get; }
    DbSet<KpiValue> KpiValues { get; }
    DbSet<KpiAlert> KpiAlerts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}
