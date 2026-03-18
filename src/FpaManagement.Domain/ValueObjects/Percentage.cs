using FpaManagement.Domain.Common;
using FpaManagement.Domain.Exceptions;

namespace FpaManagement.Domain.ValueObjects;

public class Percentage : ValueObject
{
    public decimal Value
    {
        get;
    }

    private Percentage()
    {
    } // For EF Core

    public Percentage(decimal value)
    {
        if (value < 0 || value > 100)
            throw new DomainException("Percentage must be between 0 and 100", "INVALID_PERCENTAGE");

        Value = Math.Round(value, 2);
    }

    public decimal AsDecimal() => Value / 100;

    public Percentage Add(Percentage other) => new(Value + other.Value);
    public Percentage Subtract(Percentage other) => new(Value - other.Value);

    public static Percentage FromDecimal(decimal value) => new(value * 100);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => $"{Value:F2}%";
}
