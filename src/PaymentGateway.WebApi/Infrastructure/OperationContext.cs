namespace PaymentGateway.WebApi.Infrastructure;

public sealed class OperationContext(IHttpContextAccessor httpContextAccessor) : IOperationContext
{
    public string GetMerchantId()
    {
        if (httpContextAccessor.HttpContext is null)
            throw new InvalidOperationException("HttpContext is not initialized");

        var merchantClientId = httpContextAccessor.HttpContext.Request.Headers[ApiConstants.MerchantClientIdHeader];

        if (string.IsNullOrEmpty(merchantClientId))
            throw new InvalidOperationException("No Merchant Client Id is provided");

        return merchantClientId.ToString().Trim();
    }
}