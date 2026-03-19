using FpaManagement.Domain.Common;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class Revenue : BaseAuditableEntity
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
    public Guid? CategoryId
    {
        get; private set;
    }
    public Guid? CostCenterId
    {
        get; private set;
    }
    public Guid? RecurringPatternId
    {
        get; private set;
    }
    public string? DocumentNumber
    {
        get; private set;
    }
    public string? AttachmentPath
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
    public virtual RecurringPattern? RecurringPattern
    {
        get; private set;
    }

    private Revenue()
    {
    }

    public Revenue(DateTime date, string description, Money amount, Guid? categoryId = null, Guid? costCenterId = null, Guid? recurringPatternId = null, string? documentNumber = null)
    {
        Date = date;
        Description = description;
        Amount = amount;
        CategoryId = categoryId;
        CostCenterId = costCenterId;
        RecurringPatternId = recurringPatternId;
        DocumentNumber = documentNumber;
    }
}
