using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class Scenario : BaseAuditableEntity
{
    public string Name
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public ScenarioType Type
    {
        get; private set;
    }
    public int FiscalYear
    {
        get; private set;
    }
    public bool IsActive
    {
        get; private set;
    }

    public virtual ICollection<ScenarioAssumption> Assumptions { get; private set; } = new List<ScenarioAssumption>();
    public virtual ICollection<ScenarioResult> Results { get; private set; } = new List<ScenarioResult>();

    private Scenario()
    {
    }

    public Scenario(string name, ScenarioType type, int fiscalYear, string? description = null)
    {
        Name = name;
        Type = type;
        FiscalYear = fiscalYear;
        Description = description;
        IsActive = true;
    }

    public void AddAssumption(string variable, decimal value, string? notes = null)
    {
        Assumptions.Add(new ScenarioAssumption(Id, variable, value, notes));
    }
}
