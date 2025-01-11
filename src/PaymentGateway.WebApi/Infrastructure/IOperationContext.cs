namespace PaymentGateway.WebApi.Infrastructure
{
    public interface IOperationContext
    {
        string GetMerchantId();
    }
}