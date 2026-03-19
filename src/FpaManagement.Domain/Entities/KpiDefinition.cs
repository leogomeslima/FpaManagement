using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;

namespace FpaManagement.Domain.Entities;

public class KpiDefinition : BaseAuditableEntity
{
    public string Name
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public string Formula
    {
        get; private set;
    } // e.g., "(Revenue - Expense)/Revenue"
    public KpiTargetType TargetType
    {
        get; private set;
    } // Percentage, Value
    public decimal? TargetValue
    {
        get; private set;
    }
    public decimal? MinThreshold
    {
        get; private set;
    }
    public decimal? MaxThreshold
    {
        get; private set;
    }
    public string Unit
    {
        get; private set;
    } // "%", "R$", etc.
    public int DisplayOrder
    {
        get; private set;
    }
    public bool IsActive
    {
        get; private set;
    }

    public virtual ICollection<KpiValue> Values { get; private set; } = new List<KpiValue>();
    public virtual ICollection<KpiAlert> Alerts { get; private set; } = new List<KpiAlert>();

    private KpiDefinition()
    {
    }

    public KpiDefinition(string name, string formula, string unit, KpiTargetType targetType = KpiTargetType.None, decimal? targetValue = null, string? description = null)
    {
        Name = name;
        Formula = formula;
        Unit = unit;
        TargetType = targetType;
        TargetValue = targetValue;
        Description = description;
        IsActive = true;
    }
}
