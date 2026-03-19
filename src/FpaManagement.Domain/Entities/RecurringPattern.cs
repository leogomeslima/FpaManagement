using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class RecurringPattern : BaseAuditableEntity
{
    public string Name
    {
        get; private set;
    }
    public RecurrenceType Recurrence
    {
        get; private set;
    }
    public int Interval
    {
        get; private set;
    } // e.g., every 2 months
    public DateTime StartDate
    {
        get; private set;
    }
    public DateTime? EndDate
    {
        get; private set;
    }
    public Money Amount
    {
        get; private set;
    }
    public string Description
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

    public virtual FinancialCategory? Category
    {
        get; private set;
    }
    public virtual CostCenter? CostCenter
    {
        get; private set;
    }
    public virtual ICollection<Revenue> Revenues { get; private set; } = new List<Revenue>();
    public virtual ICollection<Expense> Expenses { get; private set; } = new List<Expense>();

    private RecurringPattern()
    {
    }

    public RecurringPattern(string name, RecurrenceType recurrence, int interval, DateTime startDate, Money amount, string description, Guid? categoryId = null, Guid? costCenterId = null)
    {
        Name = name;
        Recurrence = recurrence;
        Interval = interval;
        StartDate = startDate;
        Amount = amount;
        Description = description;
        CategoryId = categoryId;
        CostCenterId = costCenterId;
    }
}
