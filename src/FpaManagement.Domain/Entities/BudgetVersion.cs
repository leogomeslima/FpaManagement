using FpaManagement.Domain.Common;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class BudgetVersion : BaseAuditableEntity
{
    public Guid BudgetId
    {
        get; private set;
    }
    public string VersionName
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public int VersionNumber
    {
        get; private set;
    }
    public bool IsCurrent
    {
        get; private set;
    }
    public DateTime? ApprovedAt
    {
        get; private set;
    }
    public string? ApprovedBy
    {
        get; private set;
    }

    // Navigation properties
    public virtual Budget Budget { get; private set; } = null!;
    public virtual ICollection<BudgetItem> Items { get; private set; } = new List<BudgetItem>();

    private BudgetVersion()
    {
    } // For EF Core

    public BudgetVersion(Guid budgetId, string versionName, string? description, int versionNumber)
    {
        if (string.IsNullOrWhiteSpace(versionName))
            throw new ArgumentException("Version name is required", nameof(versionName));

        BudgetId = budgetId;
        VersionName = versionName;
        Description = description;
        VersionNumber = versionNumber;
        IsCurrent = versionNumber == 1; // First version is current by default
    }

    public void AddItem(BudgetItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        Items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            Items.Remove(item);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void SetAsCurrent()
    {
        IsCurrent = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearCurrentFlag()
    {
        IsCurrent = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string approvedBy)
    {
        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = approvedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public Money CalculateTotal()
    {
        if (!Items.Any())
            return new Money(0);

        var total = Items.Select(i => i.PlannedAmount).Aggregate((a, b) => a.Add(b));
        return total;
    }

    public Money CalculateTotalByCategory(string category)
    {
        var items = Items.Where(i => i.Category == category);
        if (!items.Any())
            return new Money(0);

        return items.Select(i => i.PlannedAmount).Aggregate((a, b) => a.Add(b));
    }
}
