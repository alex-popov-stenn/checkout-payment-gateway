using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.ApplicationCore.Adapters;

namespace PaymentGateway.Adapters.AcquiringBank;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAcquiringBankAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration["AcquiringBankServiceBaseUrl"];

        if (baseUrl is null)
            throw new ArgumentException("The url for acquiring bank service is empty");

        services.AddHttpClient<IAcquiringBankAdapter, AcquiringBankAdapter>(options =>
        {
            options.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}