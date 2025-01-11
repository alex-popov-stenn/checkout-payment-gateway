using PaymentGateway.Domain;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.ApplicationCore
{
    public interface IPaymentService
    {
        Task<Payment> ProcessPaymentAsync(
            string merchantId,
            CardDetails cardDetails, 
            Money amount, 
            CancellationToken cancellationToken);
    }
}