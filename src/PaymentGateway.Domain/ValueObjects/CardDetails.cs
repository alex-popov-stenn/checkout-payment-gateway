using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;

namespace PaymentGateway.Domain.ValueObjects;

public sealed class CardDetails
{
    public static OneOf<CardDetails, Error> Create(TimeProvider timeProvider, string cardNumber, int expiryMonth, int expiryYear, string cvv)
    {
        var instance = new CardDetails(cardNumber, expiryMonth, expiryYear, cvv);
        var validator = new CardDetailsValidator(timeProvider);
        var result = validator.Validate(instance);
        return result.IsValid ? instance : new Error();
    }

    public string CardNumber { get; }
    public int ExpiryMonth { get; }
    public int ExpiryYear { get; }
    public string Cvv { get; }
    public string GetMaskedCardNumber() => CardNumber[^4..];

    private CardDetails(string cardNumber, int expiryMonth, int expiryYear, string cvv)
    {
        CardNumber = cardNumber;
        ExpiryMonth = expiryMonth;
        ExpiryYear = expiryYear;
        Cvv = cvv;
    }
}

public sealed class CardDetailsValidator : AbstractValidator<CardDetails>
{
    public CardDetailsValidator(TimeProvider timeProvider)
    {
        var utcNow = timeProvider.GetUtcNow();

        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Matches(@"^\d{14,19}$");

        RuleFor(x => x.ExpiryYear)
            .InclusiveBetween(utcNow.Year, 2040);

        RuleFor(x => x.ExpiryMonth)
            .InclusiveBetween(1, 12)
            .Custom((expiryMonth, context) =>
            {
                if (expiryMonth < utcNow.Month && utcNow.Year == context.InstanceToValidate.ExpiryYear)
                {
                    context.AddFailure(new ValidationFailure());
                }
            });

        RuleFor(x => x.Cvv)
            .NotEmpty()
            .Matches(@"^\d{3,4}$");
    }
}