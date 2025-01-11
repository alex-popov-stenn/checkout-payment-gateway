using PaymentGateway.Domain;

namespace PaymentGateway.ApplicationCore.Repositories
{
    public interface IPaymentRepository
    {
        Task SavePaymentAsync(Payment payment, CancellationToken cancellationToken);
        Task<Payment?> FindPaymentByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}