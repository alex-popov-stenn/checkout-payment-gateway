using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace PaymentGateway.WebApi.Infrastructure;

public sealed class MerchantIdHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters =
        [
            new OpenApiParameter
            {
                Name = ApiConstants.MerchantClientIdHeader,
                In = ParameterLocation.Header,
                Description = "Merchant Client Id",
                Required = true
            },
        ];
    }
}