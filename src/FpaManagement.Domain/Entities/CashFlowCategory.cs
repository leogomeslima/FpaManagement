using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;

namespace FpaManagement.Domain.Entities;

public class CashFlowCategory : BaseAuditableEntity
{
    public string Name
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public CashFlowCategoryType Type
    {
        get; private set;
    } // Operating, Investing, Financing
    public bool IsActive
    {
        get; private set;
    }

    public virtual ICollection<CashFlowEntry> Entries { get; private set; } = new List<CashFlowEntry>();

    private CashFlowCategory()
    {
    }

    public CashFlowCategory(string name, CashFlowCategoryType type, string? description = null)
    {
        Name = name;
        Type = type;
        Description = description;
        IsActive = true;
    }
}
