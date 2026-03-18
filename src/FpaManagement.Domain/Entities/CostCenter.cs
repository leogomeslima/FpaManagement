using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.Entities;

public class CostCenter : BaseAuditableEntity
{
    public string Code
    {
        get; private set;
    }
    public string Name
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public bool IsActive
    {
        get; private set;
    }
    public Guid DepartmentId
    {
        get; private set;
    }
    public Guid? ManagerId
    {
        get; private set;
    }
    public decimal? AnnualBudget
    {
        get; private set;
    }

    // Navigation properties
    public virtual Department Department { get; private set; } = null!;
    public virtual User? Manager
    {
        get; private set;
    }
    public virtual ICollection<BudgetItem> BudgetItems { get; private set; } = new List<BudgetItem>();
    public virtual ICollection<Revenue> Revenues { get; private set; } = new List<Revenue>();
    public virtual ICollection<Expense> Expenses { get; private set; } = new List<Expense>();

    private CostCenter()
    {
    } // For EF Core

    public CostCenter(string code, string name, Guid departmentId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Cost center code is required", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Cost center name is required", nameof(name));

        Code = code.ToUpperInvariant();
        Name = name;
        DepartmentId = departmentId;
        Description = description;
        IsActive = true;
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Cost center name is required", nameof(name));

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetManager(Guid managerId)
    {
        ManagerId = managerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAnnualBudget(decimal budget)
    {
        if (budget < 0)
            throw new ArgumentException("Budget cannot be negative", nameof(budget));

        AnnualBudget = budget;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
