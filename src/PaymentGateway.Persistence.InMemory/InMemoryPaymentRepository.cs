using System.Collections.Concurrent;
using PaymentGateway.ApplicationCore.Repositories;
using PaymentGateway.Domain;

namespace PaymentGateway.Persistence.InMemory;

public sealed class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<Guid, Payment> _payments = new();

    public Task SavePaymentAsync(Payment payment, CancellationToken cancellationToken)
    {
        _payments.AddOrUpdate(payment.Id, _ => payment, ((_, _) => payment));
        return Task.CompletedTask;
    }

    public Task<Payment?> FindPaymentByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _payments.TryGetValue(id, out Payment? payment);
        return Task.FromResult(payment);
    }
}