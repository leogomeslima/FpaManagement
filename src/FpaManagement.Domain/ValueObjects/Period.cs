using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;
using FpaManagement.Domain.Exceptions;

namespace FpaManagement.Domain.ValueObjects;

public class Period : ValueObject
{
    public DateTime StartDate
    {
        get;
    }
    public DateTime EndDate
    {
        get;
    }
    public PeriodType Type
    {
        get;
    }

    private Period()
    {
    } // For EF Core

    public Period(DateTime startDate, DateTime endDate, PeriodType type)
    {
        if (startDate >= endDate)
            throw new DomainException("Start date must be before end date", "INVALID_PERIOD");

        StartDate = startDate.Date;
        EndDate = endDate.Date;
        Type = type;
    }

    public static Period CreateMonthly(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return new Period(startDate, endDate, PeriodType.Monthly);
    }

    public static Period CreateQuarterly(int year, int quarter)
    {
        if (quarter < 1 || quarter > 4)
            throw new DomainException("Quarter must be between 1 and 4", "INVALID_QUARTER");

        var startMonth = (quarter - 1) * 3 + 1;
        var startDate = new DateTime(year, startMonth, 1);
        var endDate = startDate.AddMonths(3).AddDays(-1);
        return new Period(startDate, endDate, PeriodType.Quarterly);
    }

    public static Period CreateYearly(int year)
    {
        var startDate = new DateTime(year, 1, 1);
        var endDate = new DateTime(year, 12, 31);
        return new Period(startDate, endDate, PeriodType.Yearly);
    }

    public bool Contains(DateTime date)
    {
        return date.Date >= StartDate && date.Date <= EndDate;
    }

    public bool OverlapsWith(Period other)
    {
        return StartDate <= other.EndDate && other.StartDate <= EndDate;
    }

    public int GetDays() => (EndDate - StartDate).Days + 1;

    public int GetMonths()
    {
        return ((EndDate.Year - StartDate.Year) * 12) + EndDate.Month - StartDate.Month + 1;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
        yield return Type;
    }

    public override string ToString() => $"{StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}";
}
