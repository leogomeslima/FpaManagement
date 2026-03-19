using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.Entities;

public class KpiAlert : BaseAuditableEntity
{
    public Guid KpiDefinitionId
    {
        get; private set;
    }
    public DateTime TriggeredAt
    {
        get; private set;
    }
    public decimal CurrentValue
    {
        get; private set;
    }
    public decimal ThresholdValue
    {
        get; private set;
    }
    public string Message
    {
        get; private set;
    }
    public bool IsResolved
    {
        get; private set;
    }
    public DateTime? ResolvedAt
    {
        get; private set;
    }

    public virtual KpiDefinition KpiDefinition { get; private set; } = null!;

    private KpiAlert()
    {
    }

    public KpiAlert(Guid kpiDefinitionId, decimal currentValue, decimal thresholdValue, string message)
    {
        KpiDefinitionId = kpiDefinitionId;
        TriggeredAt = DateTime.UtcNow;
        CurrentValue = currentValue;
        ThresholdValue = thresholdValue;
        Message = message;
        IsResolved = false;
    }

    public void Resolve()
    {
        IsResolved = true;
        ResolvedAt = DateTime.UtcNow;
    }
}
