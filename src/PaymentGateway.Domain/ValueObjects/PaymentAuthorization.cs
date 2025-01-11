namespace PaymentGateway.Domain.ValueObjects;

public sealed class PaymentAuthorization
{
    public PaymentAuthorization(string authorizationCode)
    {
        if (string.IsNullOrEmpty(authorizationCode))
            throw new ArgumentNullException(nameof(authorizationCode));

        AuthorizationCode = authorizationCode;
    }

    public string AuthorizationCode { get; }
}