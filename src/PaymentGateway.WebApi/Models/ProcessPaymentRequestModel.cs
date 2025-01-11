namespace PaymentGateway.WebApi.Models;

public sealed class ProcessPaymentRequestModel
{
    public string CardNumber { get; init; } = null!;
    public int ExpiryMonth { get; init; }
    public int ExpiryYear { get; init; }
    public string Cvv { get; init; } = null!;
    public MoneyModel Amount { get; init; } = null!;
}

public record ProcessPaymentResponseModel(Guid Id, PaymentStatusModel PaymentStatus);