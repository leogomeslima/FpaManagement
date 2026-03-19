using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class CashFlowEntry : BaseAuditableEntity
{
    public DateTime Date
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
    public CashFlowType Type
    {
        get; private set;
    }
    public Guid? CategoryId
    {
        get; private set;
    }
    public Guid? CostCenterId
    {
        get; private set;
    }
    public string? DocumentNumber
    {
        get; private set;
    }
    public bool IsReconciled
    {
        get; private set;
    }
    public DateTime? ReconciliationDate
    {
        get; private set;
    }

    public virtual CashFlowCategory? Category
    {
        get; private set;
    }
    public virtual CostCenter? CostCenter
    {
        get; private set;
    }

    private CashFlowEntry()
    {
    }

    public CashFlowEntry(DateTime date,
        string description,
        Money amount,
        CashFlowType type,
        Guid? categoryId = null,
        Guid? costCenterId = null,
        string? documentNumber = null)
    {
        Date = date;
        Description = description;
        Amount = amount;
        Type = type;
        CategoryId = categoryId;
        CostCenterId = costCenterId;
        DocumentNumber = documentNumber;
        IsReconciled = false;
    }

    public void Reconcile()
    {
        IsReconciled = true;
        ReconciliationDate = DateTime.UtcNow;
    }
}
