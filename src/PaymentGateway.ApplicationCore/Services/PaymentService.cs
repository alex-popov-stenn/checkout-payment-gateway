using PaymentGateway.ApplicationCore.Adapters;
using PaymentGateway.ApplicationCore.Repositories;
using PaymentGateway.Domain;
using PaymentGateway.Domain.ValueObjects;

namespace PaymentGateway.ApplicationCore.Services;

public sealed class PaymentService(IAcquiringBankAdapter acquiringBankAdapter, IPaymentRepository paymentRepository) : IPaymentService
{
    public async Task<Payment> ProcessPaymentAsync(string merchantId, CardDetails cardDetails, Money amount, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(merchantId))
            throw new ArgumentNullException(nameof(merchantId));

        var payment = new Payment(cardDetails, amount, merchantId);

        var result = await acquiringBankAdapter.ProcessPaymentAsync(cardDetails, amount, cancellationToken);

        if (result.IsDeclined())
        {
            payment.SetDeclined();
        }
        else
        {
            var authorizationCode = result.GetCode();
            payment.SetAuthorized(authorizationCode);
        }

        await paymentRepository.SavePaymentAsync(payment, cancellationToken);

        return payment;
    }
}