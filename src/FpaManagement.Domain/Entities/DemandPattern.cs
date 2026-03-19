using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.Entities;

public class DemandPattern : BaseAuditableEntity
{
    public string Name
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public string Product
    {
        get; private set;
    }
    public string? Service
    {
        get; private set;
    }
    public decimal BaseDemand
    {
        get; private set;
    } // average
    public decimal Trend
    {
        get; private set;
    } // growth rate
    public decimal SeasonalityFactor
    {
        get; private set;
    }
    public Guid? CostCenterId
    {
        get; private set;
    }

    public virtual CostCenter? CostCenter
    {
        get; private set;
    }
    public virtual ICollection<SeasonalityFactor> SeasonalityFactors { get; private set; } = new List<SeasonalityFactor>();

    private DemandPattern()
    {
    }

    public DemandPattern(string name, string product, decimal baseDemand, decimal trend = 0, decimal seasonalityFactor = 1, string? service = null, string? description = null, Guid? costCenterId = null)
    {
        Name = name;
        Product = product;
        Service = service;
        BaseDemand = baseDemand;
        Trend = trend;
        SeasonalityFactor = seasonalityFactor;
        Description = description;
        CostCenterId = costCenterId;
    }
}
