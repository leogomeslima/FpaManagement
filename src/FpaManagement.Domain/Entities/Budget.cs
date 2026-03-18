using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class Budget : BaseAuditableEntity
{
    public string Name
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public int FiscalYear
    {
        get; private set;
    }
    public BudgetStatus Status
    {
        get; private set;
    }
    public Guid? DepartmentId
    {
        get; private set;
    }
    public Guid? CostCenterId
    {
        get; private set;
    }
    public Money? TotalAmount
    {
        get; private set;
    }

    // Navigation properties
    public virtual Department? Department
    {
        get; private set;
    }
    public virtual CostCenter? CostCenter
    {
        get; private set;
    }
    public virtual ICollection<BudgetVersion> Versions { get; private set; } = new List<BudgetVersion>();
    public virtual ICollection<BudgetApproval> Approvals { get; private set; } = new List<BudgetApproval>();

    private Budget()
    {
    } // For EF Core

    public Budget(string name, int fiscalYear, Guid? departmentId = null, Guid? costCenterId = null, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Budget name is required", nameof(name));

        if (fiscalYear < 2000 || fiscalYear > 2100)
            throw new ArgumentException("Invalid fiscal year", nameof(fiscalYear));

        Name = name;
        Description = description;
        FiscalYear = fiscalYear;
        DepartmentId = departmentId;
        CostCenterId = costCenterId;
        Status = BudgetStatus.Draft;
    }

    public BudgetVersion CreateVersion(string versionName, string? description = null)
    {
        var version = new BudgetVersion(Id, versionName, description, Versions.Count + 1);
        Versions.Add(version);
        UpdatedAt = DateTime.UtcNow;
        return version;
    }

    public void SubmitForApproval()
    {
        if (Status != BudgetStatus.Draft && Status != BudgetStatus.Rejected)
            throw new InvalidOperationException($"Cannot submit budget in {Status} status");

        if (!Versions.Any(v => v.IsCurrent))
            throw new InvalidOperationException("Budget must have at least one version");

        Status = BudgetStatus.Pending;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string approverId, string? comments = null)
    {
        if (Status != BudgetStatus.Pending)
            throw new InvalidOperationException($"Cannot approve budget in {Status} status");

        Status = BudgetStatus.Approved;

        var approval = new BudgetApproval(Id, Guid.Parse(approverId), ApprovalStatus.Approved, comments);
        Approvals.Add(approval);

        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = approverId;
    }

    public void Reject(string approverId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required", nameof(reason));

        if (Status != BudgetStatus.Pending)
            throw new InvalidOperationException($"Cannot reject budget in {Status} status");

        Status = BudgetStatus.Rejected;

        var approval = new BudgetApproval(Id, Guid.Parse(approverId), ApprovalStatus.Rejected, reason);
        Approvals.Add(approval);

        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = approverId;
    }

    public void Archive()
    {
        if (Status == BudgetStatus.Archived)
            return;

        Status = BudgetStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTotalAmount()
    {
        var currentVersion = Versions.FirstOrDefault(v => v.IsCurrent);
        if (currentVersion != null)
        {
            TotalAmount = currentVersion.CalculateTotal();
        }
    }
}
