using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.Entities;

public class SeasonalityFactor : BaseAuditableEntity
{
    public Guid DemandPatternId
    {
        get; private set;
    }
    public int Month
    {
        get; private set;
    } // 1-12
    public decimal Factor
    {
        get; private set;
    } // multiplier

    public virtual DemandPattern DemandPattern { get; private set; } = null!;

    private SeasonalityFactor()
    {
    }

    public SeasonalityFactor(Guid demandPatternId, int month, decimal factor)
    {
        if (month < 1 || month > 12)
            throw new ArgumentException("Month must be between 1 and 12");
        DemandPatternId = demandPatternId;
        Month = month;
        Factor = factor;
    }
}
