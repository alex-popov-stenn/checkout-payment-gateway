namespace PaymentGateway.WebApi.Models;

public record PaymentModel(Guid Id, PaymentStatusModel PaymentStatusModel, CardDetailsModel CardDetails, MoneyModel Amount);