using FpaManagement.Domain.Common;
using FpaManagement.Domain.Exceptions;

namespace FpaManagement.Domain.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount
    {
        get;
    }
    public string Currency
    {
        get;
    }

    private Money()
    {
    } // For EF Core

    public Money(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative", "INVALID_AMOUNT");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required", "INVALID_CURRENCY");

        Amount = Math.Round(amount, 2);
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        if (other.Currency != Currency)
            throw new DomainException("Cannot add money with different currencies", "CURRENCY_MISMATCH");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (other.Currency != Currency)
            throw new DomainException("Cannot subtract money with different currencies", "CURRENCY_MISMATCH");

        if (other.Amount > Amount)
            throw new DomainException("Insufficient amount for subtraction", "INSUFFICIENT_AMOUNT");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        if (factor < 0)
            throw new DomainException("Multiplication factor cannot be negative", "INVALID_FACTOR");

        return new Money(Amount * factor, Currency);
    }

    public Money Divide(decimal divisor)
    {
        if (divisor <= 0)
            throw new DomainException("Divisor must be greater than zero", "INVALID_DIVISOR");

        return new Money(Amount / divisor, Currency);
    }

    public bool IsGreaterThan(Money other)
    {
        if (other.Currency != Currency)
            throw new DomainException("Cannot compare money with different currencies", "CURRENCY_MISMATCH");

        return Amount > other.Amount;
    }

    public bool IsLessThan(Money other)
    {
        if (other.Currency != Currency)
            throw new DomainException("Cannot compare money with different currencies", "CURRENCY_MISMATCH");

        return Amount < other.Amount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:N2}";
}
