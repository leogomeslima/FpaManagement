using FpaManagement.Domain.Common;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class BudgetItem : BaseAuditableEntity
{
    public Guid BudgetVersionId
    {
        get; private set;
    }
    public string Category
    {
        get; private set;
    }
    public string? SubCategory
    {
        get; private set;
    }
    public string Description
    {
        get; private set;
    }
    public Money PlannedAmount
    {
        get; private set;
    }
    public Money? ActualAmount
    {
        get; private set;
    }
    public Money? Variance
    {
        get; private set;
    }
    public decimal? VariancePercentage
    {
        get; private set;
    }
    public Guid? CostCenterId
    {
        get; private set;
    }
    public Guid? ProjectId
    {
        get; private set;
    }
    public string? AccountCode
    {
        get; private set;
    }
    public Dictionary<string, object>? Metadata
    {
        get; private set;
    }

    // Navigation properties
    public virtual BudgetVersion BudgetVersion { get; private set; } = null!;
    public virtual CostCenter? CostCenter
    {
        get; private set;
    }

    private BudgetItem()
    {
    } // For EF Core

    public BudgetItem(
        Guid budgetVersionId,
        string category,
        string description,
        Money plannedAmount,
        string? subCategory = null,
        Guid? costCenterId = null,
        Guid? projectId = null,
        string? accountCode = null)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category is required", nameof(category));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        if (plannedAmount == null)
            throw new ArgumentNullException(nameof(plannedAmount));

        BudgetVersionId = budgetVersionId;
        Category = category;
        SubCategory = subCategory;
        Description = description;
        PlannedAmount = plannedAmount;
        CostCenterId = costCenterId;
        ProjectId = projectId;
        AccountCode = accountCode;
    }

    public void UpdatePlannedAmount(Money newAmount)
    {
        if (newAmount == null)
            throw new ArgumentNullException(nameof(newAmount));

        PlannedAmount = newAmount;
        UpdatedAt = DateTime.UtcNow;
        RecalculateVariance();
    }

    public void RecordActualAmount(Money actualAmount)
    {
        if (actualAmount == null)
            throw new ArgumentNullException(nameof(actualAmount));

        ActualAmount = actualAmount;
        UpdatedAt = DateTime.UtcNow;
        RecalculateVariance();
    }

    private void RecalculateVariance()
    {
        if (ActualAmount != null)
        {
            Variance = ActualAmount.Subtract(PlannedAmount);
            VariancePercentage = PlannedAmount.Amount != 0
                ? Math.Round((ActualAmount.Amount - PlannedAmount.Amount) / PlannedAmount.Amount * 100, 2)
                : 0;
        }
    }

    public void SetMetadata(string key, object value)
    {
        Metadata ??= new Dictionary<string, object>();
        Metadata[key] = value;
        UpdatedAt = DateTime.UtcNow;
    }
}
