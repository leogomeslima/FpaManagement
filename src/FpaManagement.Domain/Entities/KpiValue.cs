using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.Entities;

public class KpiValue : BaseAuditableEntity
{
    public Guid KpiDefinitionId
    {
        get; private set;
    }
    public DateTime Period
    {
        get; private set;
    }
    public decimal Value
    {
        get; private set;
    }
    public decimal? TargetValue
    {
        get; private set;
    }
    public decimal? Variance
    {
        get; private set;
    }
    public decimal? VariancePercentage
    {
        get; private set;
    }

    public virtual KpiDefinition KpiDefinition { get; private set; } = null!;

    private KpiValue()
    {
    }

    public KpiValue(Guid kpiDefinitionId, DateTime period, decimal value, decimal? targetValue = null)
    {
        KpiDefinitionId = kpiDefinitionId;
        Period = period;
        Value = value;
        TargetValue = targetValue;
        if (targetValue.HasValue)
        {
            Variance = value - targetValue.Value;
            VariancePercentage = targetValue.Value != 0 ? (value / targetValue.Value - 1) * 100 : 0;
        }
    }
}
