using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.Entities;

public class Report : BaseAuditableEntity
{
    public string Name
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public string ReportType
    {
        get; private set;
    } // e.g., "BudgetVsActual", "CashFlow", "DFC"
    public string Parameters
    {
        get; private set;
    } // JSON
    public string? FilePath
    {
        get; private set;
    }
    public Guid? CreatedByUserId
    {
        get; private set;
    }
    public bool IsScheduled
    {
        get; private set;
    }
    public string? ScheduleCron
    {
        get; private set;
    }

    public virtual User? CreatedByUser
    {
        get; private set;
    }

    private Report()
    {
    }

    public Report(string name, string reportType, string parameters, Guid? createdByUserId = null)
    {
        Name = name;
        ReportType = reportType;
        Parameters = parameters;
        CreatedByUserId = createdByUserId;
        IsScheduled = false;
    }
}
