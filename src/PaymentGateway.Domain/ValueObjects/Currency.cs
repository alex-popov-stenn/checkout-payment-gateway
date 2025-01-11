using OneOf;
using OneOf.Types;

namespace PaymentGateway.Domain.ValueObjects;

public sealed class Currency
{
    //USD, EUR, GBP
    //Each of these currencies has two decimal min. currency unit
    private static readonly IReadOnlyDictionary<string, string> AvailableCurrencies = new Dictionary<string, string>
    {
        { "840", "USD" },
        { "978", "EUR" },
        { "826", "GBP" }
    };

    public static OneOf<Currency, Error> Create(string currencyCode)
    {
        if (!AvailableCurrencies.ContainsKey(currencyCode))
            return new Error();

        var instance = new Currency(currencyCode, AvailableCurrencies[currencyCode]);
        return instance;
    }

    private Currency(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; }
    public string Name { get; }
}