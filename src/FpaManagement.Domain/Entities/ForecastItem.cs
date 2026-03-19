using FpaManagement.Domain.Common;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class ForecastItem : BaseAuditableEntity
{
    public Guid ForecastVersionId
    {
        get; private set;
    }
    public string Category
    {
        get; private set;
    }
    public string? SubCategory
    {
        get; private set;
    }
    public string Description
    {
        get; private set;
    }
    public Money Amount
    {
        get; private set;
    }
    public Guid? CostCenterId
    {
        get; private set;
    }
    public string? AccountCode
    {
        get; private set;
    }

    public virtual ForecastVersion ForecastVersion { get; private set; } = null!;
    public virtual CostCenter? CostCenter
    {
        get; private set;
    }

    private ForecastItem()
    {
    }

    public ForecastItem(Guid forecastVersionId, string category, string description, Money amount, string? subCategory = null, Guid? costCenterId = null, string? accountCode = null)
    {
        ForecastVersionId = forecastVersionId;
        Category = category;
        SubCategory = subCategory;
        Description = description;
        Amount = amount;
        CostCenterId = costCenterId;
        AccountCode = accountCode;
    }
}
