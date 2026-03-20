using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class Forecast : BaseAuditableEntity
{
    public string Name
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public int FiscalYear
    {
        get; private set;
    }
    public ForecastStatus Status
    {
        get; private set;
    }
    public Guid? DepartmentId
    {
        get; private set;
    }
    public Guid? CostCenterId
    {
        get; private set;
    }
    public Money? TotalAmount
    {
        get; private set;
    }

    // Navigation
    public virtual Department? Department
    {
        get; private set;
    }
    public virtual CostCenter? CostCenter
    {
        get; private set;
    }
    public virtual ICollection<ForecastVersion> Versions { get; private set; } = new List<ForecastVersion>();
    public virtual ICollection<ForecastJustification> Justifications { get; private set; } = new List<ForecastJustification>();

    private Forecast()
    {
    }

    public Forecast(string name, int fiscalYear, Guid? departmentId = null, Guid? costCenterId = null, string? description = null)
    {
        Name = name;
        FiscalYear = fiscalYear;
        DepartmentId = departmentId;
        CostCenterId = costCenterId;
        Description = description;
        Status = ForecastStatus.Draft;
    }

    public ForecastVersion CreateVersion(string versionName, string? description = null)
    {
        var version = new ForecastVersion(Id, versionName, description, Versions.Count + 1);
        Versions.Add(version);
        return version;
    }

    public void Submit()
    {
        if (Status != ForecastStatus.Draft && Status != ForecastStatus.Rejected)
            throw new InvalidOperationException("Forecast must be in Draft or Rejected status to submit.");
        Status = ForecastStatus.Submitted;
    }

    public void Approve()
    {
        if (Status != ForecastStatus.Submitted)
            throw new InvalidOperationException("Forecast must be Submitted to approve.");
        Status = ForecastStatus.Approved;
    }

    public void Reject()
    {
        if (Status != ForecastStatus.Submitted)
            throw new InvalidOperationException("Forecast must be Submitted to reject.");
        Status = ForecastStatus.Rejected;
    }
    public void UpdateTotalAmount(Money total)
    {
        TotalAmount = total ?? throw new ArgumentNullException(nameof(total));
        // Se desejar, você pode adicionar lógica aqui para disparar um 
        // evento de domínio informando que o valor do Forecast mudou.
    }
}
