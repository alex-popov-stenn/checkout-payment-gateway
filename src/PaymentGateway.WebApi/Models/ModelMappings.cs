using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.WebApi.Models;

public static class ModelMappings
{
    public static PaymentStatusModel ToModel(this PaymentStatus paymentStatus)
    {
        return paymentStatus switch
        {
            PaymentStatus.Authorized => PaymentStatusModel.Authorized,
            PaymentStatus.Declined => PaymentStatusModel.Declined,
            _ => throw new NotSupportedException(nameof(paymentStatus))
        };
    }

    public static CardDetailsModel ToModel(this CardDetails cardDetails)
    {
        return new CardDetailsModel(cardDetails.GetMaskedCardNumber(), cardDetails.ExpiryMonth, cardDetails.ExpiryYear);
    }

    public static MoneyModel ToModel(this Money amount)
    {
        return new MoneyModel(amount.Amount, amount.Currency.Code);
    }
}