using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.ValueObjects;

public class FinancialRatio : ValueObject
{
    public decimal Value
    {
        get;
    }
    public string Name
    {
        get;
    }

    public FinancialRatio(decimal value, string name)
    {
        Value = value;
        Name = name;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Name;
    }
}
