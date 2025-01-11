using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.Domain;

public sealed class Payment
{
    public Payment(CardDetails cardDetails, Money amount, string merchantId)
    {
        Id = Guid.NewGuid();
        Status = PaymentStatus.New;
        CardDetails = cardDetails;
        Amount = amount;
        MerchantId = merchantId ?? throw new ArgumentNullException(nameof(merchantId));
    }

    public void SetDeclined()
    {
        EnsureNotProcessedYet();
        Status = PaymentStatus.Declined;
    }

    public void SetAuthorized(PaymentAuthorization paymentAuthorization)
    {
        EnsureNotProcessedYet();
        AuthorizationCode = paymentAuthorization.AuthorizationCode;
        Status = PaymentStatus.Authorized;
    }

    public Guid Id { get; private set; }
    public string? AuthorizationCode { get; private set; }
    public string MerchantId { get; private set; }
    public PaymentStatus Status { get; private set; }
    public CardDetails CardDetails { get; private set; }
    public Money Amount { get; private set; }

    private void EnsureNotProcessedYet()
    {
        if (Status != PaymentStatus.New)
            throw new InvalidOperationException("Payment is already processed");
    }
}