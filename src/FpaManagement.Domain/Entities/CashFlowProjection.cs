using FpaManagement.Domain.Common;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class CashFlowProjection : BaseAuditableEntity
{
    public DateTime Date
    {
        get; private set;
    }
    public Money ProjectedInflow
    {
        get; private set;
    }
    public Money ProjectedOutflow
    {
        get; private set;
    }
    public Money ProjectedBalance
    {
        get; private set;
    }
    public Money ActualBalance
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

    private CashFlowProjection()
    {
    }

    public CashFlowProjection(DateTime date, Money projectedInflow, Money projectedOutflow, Guid? costCenterId = null)
    {
        Date = date;
        ProjectedInflow = projectedInflow;
        ProjectedOutflow = projectedOutflow;
        ProjectedBalance = projectedInflow.Subtract(projectedOutflow);
        ActualBalance = new Money(0);
        CostCenterId = costCenterId;
    }

    public void UpdateActual(Money actualBalance)
    {
        ActualBalance = actualBalance;
    }
}
