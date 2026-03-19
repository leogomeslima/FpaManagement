using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.ValueObjects;

public class Variance : ValueObject
{
    public decimal Absolute
    {
        get;
    }
    public decimal Percentage
    {
        get;
    }

    public Variance(decimal actual, decimal planned)
    {
        Absolute = actual - planned;
        Percentage = planned != 0 ? (actual / planned - 1) * 100 : 0;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Absolute;
        yield return Percentage;
    }
}
