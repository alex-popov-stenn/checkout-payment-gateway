using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.ApplicationCore.Services;

namespace PaymentGateway.ApplicationCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationCore(this IServiceCollection services)
        {
            services.AddSingleton(TimeProvider.System);
            services.AddScoped<IPaymentService, PaymentService>();
            return services;
        } 
    }
}