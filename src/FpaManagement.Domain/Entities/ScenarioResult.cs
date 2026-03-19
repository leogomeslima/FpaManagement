using FpaManagement.Domain.Common;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class ScenarioResult : BaseAuditableEntity
{
    public Guid ScenarioId
    {
        get; private set;
    }
    public Money ProjectedRevenue
    {
        get; private set;
    }
    public Money ProjectedExpense
    {
        get; private set;
    }
    public Money ProjectedProfit
    {
        get; private set;
    }
    public Money ProjectedCashFlow
    {
        get; private set;
    }
    public decimal ProjectedEbitda
    {
        get; private set;
    }

    public virtual Scenario Scenario { get; private set; } = null!;

    private ScenarioResult()
    {
    }

    public ScenarioResult(Guid scenarioId, Money projectedRevenue, Money projectedExpense, Money projectedCashFlow, decimal projectedEbitda)
    {
        ScenarioId = scenarioId;
        ProjectedRevenue = projectedRevenue;
        ProjectedExpense = projectedExpense;
        ProjectedProfit = projectedRevenue.Subtract(projectedExpense);
        ProjectedCashFlow = projectedCashFlow;
        ProjectedEbitda = projectedEbitda;
    }
}
