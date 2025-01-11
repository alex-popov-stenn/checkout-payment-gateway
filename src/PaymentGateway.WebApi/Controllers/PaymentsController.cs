using Microsoft.AspNetCore.Mvc;
using PaymentGateway.ApplicationCore;
using PaymentGateway.ApplicationCore.Repositories;
using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.Utils;
using PaymentGateway.WebApi.Infrastructure;
using PaymentGateway.WebApi.Models;

namespace PaymentGateway.WebApi.Controllers;

[Route("api/[controller]")]
public class PaymentsController(
    IOperationContext operationContext,
    IPaymentService paymentService,
    IPaymentRepository paymentRepository,
    TimeProvider timeProvider)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequestModel request, CancellationToken cancellationToken)
    {
        var merchantId = operationContext.GetMerchantId();

        var cardDetails = CardDetails.Create(timeProvider, request.CardNumber, request.ExpiryMonth, request.ExpiryYear, request.Cvv);
        var amount = Money.Create(request.Amount.Amount, request.Amount.CurrencyCode);

        if (cardDetails.IsInvalid() || amount.IsInvalid())
        {
            return BadRequest("Rejected");
        }

        var payment = await paymentService.ProcessPaymentAsync(merchantId, cardDetails.GetValue(), amount.GetValue(), cancellationToken);

        return CreatedAtAction(
            nameof(GetPayment),
            new { id = payment.Id },
            new ProcessPaymentResponseModel(payment.Id, payment.Status.ToModel())
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPayment([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var merchantId = operationContext.GetMerchantId();
        var payment = await paymentRepository.FindPaymentByIdAsync(id, cancellationToken);

        if (payment is null || payment.MerchantId != merchantId)
            return NotFound(); 

        return Ok(new PaymentModel(payment.Id, payment.Status.ToModel(), payment.CardDetails.ToModel(), payment.Amount.ToModel()));
    }
}