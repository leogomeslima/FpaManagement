using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.ValueObjects;

public class Address : ValueObject
{
    public string Street
    {
        get;
    }
    public string Number
    {
        get;
    }
    public string? Complement
    {
        get;
    }
    public string Neighborhood
    {
        get;
    }
    public string City
    {
        get;
    }
    public string State
    {
        get;
    }
    public string ZipCode
    {
        get;
    }
    public string Country
    {
        get;
    }

    private Address()
    {
    } // For EF Core

    public Address(
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string zipCode,
        string? complement = null,
        string country = "Brasil")
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        Number = number ?? throw new ArgumentNullException(nameof(number));
        Neighborhood = neighborhood ?? throw new ArgumentNullException(nameof(neighborhood));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
        Complement = complement;
        Country = country ?? throw new ArgumentNullException(nameof(country));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return Number;
        yield return Complement ?? string.Empty;
        yield return Neighborhood;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }

    public override string ToString()
    {
        var address = $"{Street}, {Number}";
        if (!string.IsNullOrWhiteSpace(Complement))
            address += $" - {Complement}";
        address += $", {Neighborhood}, {City} - {State}, {ZipCode}";
        return address;
    }
}
