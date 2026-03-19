using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class DfcEntry : BaseAuditableEntity
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
    public DfcActivityType ActivityType
    {
        get; private set;
    } // Operating, Investing, Financing
    public Guid? CashFlowEntryId
    {
        get; private set;
    } // Optional link to cash flow entry

    public virtual CashFlowEntry? CashFlowEntry
    {
        get; private set;
    }

    private DfcEntry()
    {
    }

    public DfcEntry(DateTime date, string description, Money amount, DfcActivityType activityType, Guid? cashFlowEntryId = null)
    {
        Date = date;
        Description = description;
        Amount = amount;
        ActivityType = activityType;
        CashFlowEntryId = cashFlowEntryId;
    }
}
