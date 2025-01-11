using FluentValidation;
using OneOf;
using OneOf.Types;
using PaymentGateway.Utils;

namespace PaymentGateway.Domain.ValueObjects;

public sealed class Money
{
    public static OneOf<Money, Error> Create(long amount, string currencyCode)
    {
        var currency = Currency.Create(currencyCode);

        if (currency.IsInvalid())
            return new Error();

        var instance = new Money(amount, currency.GetValue());
        var validator = new MoneyValidator();
        var result = validator.Validate(instance);
        return result.IsValid ? instance : new Error();
    }

    private Money(long amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public long Amount { get; }
    public Currency Currency { get; }
}

public sealed class MoneyValidator : AbstractValidator<Money>
{
    public MoneyValidator()
    {
        RuleFor(x => x.Amount)
            .Must(amount => amount > 0);

        RuleFor(x => x.Currency)
            .NotNull();
    }
}