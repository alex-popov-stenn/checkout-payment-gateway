using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.ApplicationCore.Repositories;

namespace PaymentGateway.Persistence.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryPersistence(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
        return services;
    }
}