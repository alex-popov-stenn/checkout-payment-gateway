using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;
using PaymentGateway.ApplicationCore.Adapters;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.Adapters.AcquiringBank;

public sealed class AcquiringBankAdapter(HttpClient httpClient) : IAcquiringBankAdapter
{
    public async Task<OneOf<PaymentAuthorization, Declined>> ProcessPaymentAsync(
        CardDetails cardDetails, 
        Money amount, 
        CancellationToken cancellationToken)
    {
        var processPaymentDto = new ProcessPaymentDto
        {
            Amount = amount.Amount,
            Currency = amount.Currency.Name,
            CardNumber = cardDetails.CardNumber,
            Cvv = cardDetails.Cvv,
            ExpiryDate = $"{cardDetails.ExpiryMonth.ToString().PadLeft(2, '0')}/{cardDetails.ExpiryYear}"
        };

        var response = await httpClient.PostAsJsonAsync("/payments", processPaymentDto, cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var paymentProcessingResult = JsonSerializer.Deserialize<PaymentProcessingResultDto>(content);

        if (paymentProcessingResult is null)
            throw new ArgumentException(nameof(paymentProcessingResult));

        if (!paymentProcessingResult.Authorized)
            return new Declined();

        if (paymentProcessingResult.AuthorizationCode is null)
            throw new ArgumentException(nameof(paymentProcessingResult.AuthorizationCode));

        return new PaymentAuthorization(paymentProcessingResult.AuthorizationCode);
    }

    private class ProcessPaymentDto
    {
        [JsonPropertyName("card_number")]
        public string CardNumber { get; init; } = null!;
        [JsonPropertyName("expiry_date")]
        public string ExpiryDate { get; init; } = null!;
        [JsonPropertyName("currency")]
        public string Currency { get; init; } = null!;
        [JsonPropertyName("amount")]
        public long Amount { get; init; }
        [JsonPropertyName("cvv")]
        public string Cvv { get; init; } = null!;
    }

    private class PaymentProcessingResultDto
    {
        [JsonPropertyName("authorized")]
        public bool Authorized { get; init; }
        [JsonPropertyName("authorization_code")]
        public string? AuthorizationCode { get; init; }
    }
}