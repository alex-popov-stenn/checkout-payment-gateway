namespace PaymentGateway.WebApi.Models;

public record CardDetailsModel(string MaskedCardNumber, int ExpiryMonth, int ExpiryYear);