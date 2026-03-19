using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.Entities;

public class ForecastJustification : BaseAuditableEntity
{
    public Guid ForecastId
    {
        get; private set;
    }
    public string Reason
    {
        get; private set;
    }
    public decimal VariancePercentage
    {
        get; private set;
    }
    public string? Comments
    {
        get; private set;
    }

    public virtual Forecast Forecast { get; private set; } = null!;

    private ForecastJustification()
    {
    }

    public ForecastJustification(Guid forecastId, string reason, decimal variancePercentage, string? comments = null)
    {
        ForecastId = forecastId;
        Reason = reason;
        VariancePercentage = variancePercentage;
        Comments = comments;
    }
}
