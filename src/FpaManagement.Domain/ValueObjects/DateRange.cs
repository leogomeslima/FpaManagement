using FpaManagement.Domain.Common;
using FpaManagement.Domain.Exceptions;

namespace FpaManagement.Domain.ValueObjects;

public class DateRange : ValueObject
{
    public DateTime Start
    {
        get;
    }
    public DateTime End
    {
        get;
    }

    private DateRange()
    {
    } // For EF Core

    public DateRange(DateTime start, DateTime end)
    {
        if (start >= end)
            throw new DomainException("Start date must be before end date", "INVALID_DATE_RANGE");

        Start = start;
        End = end;
    }

    public bool Includes(DateTime date) => date >= Start && date <= End;

    public bool OverlapsWith(DateRange other) => Start <= other.End && other.Start <= End;

    public TimeSpan Duration => End - Start;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }

    public override string ToString() => $"{Start:dd/MM/yyyy} - {End:dd/MM/yyyy}";
}
