using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;

namespace FpaManagement.Domain.Entities;

public class FinancialCategory : BaseAuditableEntity
{
    public string Name
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public FinancialCategoryType Type
    {
        get; private set;
    } // Revenue or Expense
    public Guid? ParentId
    {
        get; private set;
    }
    public bool IsActive
    {
        get; private set;
    }

    public virtual FinancialCategory? Parent
    {
        get; private set;
    }
    public virtual ICollection<FinancialCategory> Children { get; private set; } = new List<FinancialCategory>();
    public virtual ICollection<Revenue> Revenues { get; private set; } = new List<Revenue>();
    public virtual ICollection<Expense> Expenses { get; private set; } = new List<Expense>();

    private FinancialCategory()
    {
    }

    public FinancialCategory(string name, FinancialCategoryType type, string? description = null, Guid? parentId = null)
    {
        Name = name;
        Type = type;
        Description = description;
        ParentId = parentId;
        IsActive = true;
    }
}
