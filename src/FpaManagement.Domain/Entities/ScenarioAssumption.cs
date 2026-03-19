using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.Entities;

public class ScenarioAssumption : BaseAuditableEntity
{
    public Guid ScenarioId
    {
        get; private set;
    }
    public string Variable
    {
        get; private set;
    }
    public decimal Value
    {
        get; private set;
    }
    public string? Notes
    {
        get; private set;
    }

    public virtual Scenario Scenario { get; private set; } = null!;

    private ScenarioAssumption()
    {
    }

    public ScenarioAssumption(Guid scenarioId, string variable, decimal value, string? notes = null)
    {
        ScenarioId = scenarioId;
        Variable = variable;
        Value = value;
        Notes = notes;
    }
}
