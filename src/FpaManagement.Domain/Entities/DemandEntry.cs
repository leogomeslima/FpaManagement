using FpaManagement.Domain.Common;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class DemandEntry : BaseAuditableEntity
{
    public DateTime Date
    {
        get; private set;
    }
    public string Product
    {
        get; private set;
    }
    public string? Service
    {
        get; private set;
    }
    public int Quantity
    {
        get; private set;
    }
    public Money UnitPrice
    {
        get; private set;
    }
    public Money TotalAmount => UnitPrice.Multiply(Quantity);
    public Guid? CostCenterId
    {
        get; private set;
    }

    public virtual CostCenter? CostCenter
    {
        get; private set;
    }

    private DemandEntry()
    {
    }

    public DemandEntry(DateTime date, string product, int quantity, Money unitPrice, string? service = null, Guid? costCenterId = null)
    {
        Date = date;
        Product = product;
        Service = service;
        Quantity = quantity;
        UnitPrice = unitPrice;
        CostCenterId = costCenterId;
    }
}
