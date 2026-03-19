using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;

namespace FpaManagement.Domain.Entities;

public class DfcCategory : BaseAuditableEntity
{
    public string Name
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public DfcActivityType ActivityType
    {
        get; private set;
    }
    public bool IsActive
    {
        get; private set;
    }

    private DfcCategory()
    {
    }

    public DfcCategory(string name, DfcActivityType activityType, string? description = null)
    {
        Name = name;
        ActivityType = activityType;
        Description = description;
        IsActive = true;
    }
}
