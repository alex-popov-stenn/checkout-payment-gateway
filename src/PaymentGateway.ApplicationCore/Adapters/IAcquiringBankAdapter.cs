using OneOf;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.ApplicationCore.Adapters
{
    public interface IAcquiringBankAdapter
    {
        Task<OneOf<PaymentAuthorization, Declined>> ProcessPaymentAsync(
            CardDetails cardDetails,
            Money amount, 
            CancellationToken cancellationToken);
    }
}