using System.Reflection;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Domain.Common;
using FpaManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FpaManagement.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    string,
    ApplicationUserClaim,
    ApplicationUserRole,
    ApplicationUserLogin,
    ApplicationRoleClaim,
    ApplicationUserToken>, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntityInterceptor;
    private IDbContextTransaction? _currentTransaction;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntityInterceptor)
        : base(options)
    {
        _mediator = mediator;
        _auditableEntityInterceptor = auditableEntityInterceptor;
    }

    // Domain Entities
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<CostCenter> CostCenters => Set<CostCenter>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<BudgetVersion> BudgetVersions => Set<BudgetVersion>();
    public DbSet<BudgetItem> BudgetItems => Set<BudgetItem>();
    public DbSet<BudgetApproval> BudgetApprovals => Set<BudgetApproval>();
    public DbSet<Forecast> Forecasts => Set<Forecast>();
    public DbSet<ForecastVersion> ForecastVersions => Set<ForecastVersion>();
    public DbSet<ForecastItem> ForecastItems => Set<ForecastItem>();
    public DbSet<ForecastJustification> ForecastJustifications => Set<ForecastJustification>();
    public DbSet<CashFlowEntry> CashFlowEntries => Set<CashFlowEntry>();
    public DbSet<CashFlowCategory> CashFlowCategories => Set<CashFlowCategory>();
    public DbSet<CashFlowProjection> CashFlowProjections => Set<CashFlowProjection>();
    public DbSet<DfcEntry> DfcEntries => Set<DfcEntry>();
    public DbSet<DfcCategory> DfcCategories => Set<DfcCategory>();
    public DbSet<Revenue> Revenues => Set<Revenue>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<FinancialCategory> FinancialCategories => Set<FinancialCategory>();
    public DbSet<RecurringPattern> RecurringPatterns => Set<RecurringPattern>();
    public DbSet<DemandEntry> DemandEntries => Set<DemandEntry>();
    public DbSet<DemandPattern> DemandPatterns => Set<DemandPattern>();
    public DbSet<SeasonalityFactor> SeasonalityFactors => Set<SeasonalityFactor>();
    public DbSet<Scenario> Scenarios => Set<Scenario>();
    public DbSet<ScenarioAssumption> ScenarioAssumptions => Set<ScenarioAssumption>();
    public DbSet<ScenarioResult> ScenarioResults => Set<ScenarioResult>();
    public DbSet<KpiDefinition> KpiDefinitions => Set<KpiDefinition>();
    public DbSet<KpiValue> KpiValues => Set<KpiValue>();
    public DbSet<KpiAlert> KpiAlerts => Set<KpiAlert>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);

        // Global query filters for soft delete
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(SetGlobalQueryFilter), BindingFlags.NonPublic | BindingFlags.Static)?
                    .MakeGenericMethod(entityType.ClrType);

                method?.Invoke(null, new object[] { builder });
            }
        }
    }

    private static void SetGlobalQueryFilter<T>(ModelBuilder builder) where T : BaseEntity
    {
        builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await (_currentTransaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await (_currentTransaction?.RollbackAsync(cancellationToken) ?? Task.CompletedTask);
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }
}
