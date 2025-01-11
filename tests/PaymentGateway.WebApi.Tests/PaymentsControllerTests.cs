using System.Net.Http.Json;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PaymentGateway.WebApi.Controllers;
using PaymentGateway.WebApi.Infrastructure;
using PaymentGateway.WebApi.Models;

namespace PaymentGateway.WebApi.Tests;

[Collection("BankSimulator collection")]
public sealed class PaymentsControllerTests(WebApplicationFactory<PaymentsController> factory) 
    : IClassFixture<WebApplicationFactory<PaymentsController>>
{
    private readonly HttpClient _client = factory.CreateClient();
 
    [Theory]
    [InlineData(PaymentStatusModel.Authorized)]
    [InlineData(PaymentStatusModel.Declined)]
    public async Task ProcessPayment_WithValidCardDetails_ReturnsCreatedWithCorrectData(PaymentStatusModel paymentStatus)
    {
        //arrange
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var merchantId = Guid.NewGuid().ToString();
        var requestModel = paymentStatus == PaymentStatusModel.Authorized ? GetPaymentAuthorizedRequestModel() : GetPaymentDeclinedRequestModel();

        //act
        var responseModel = await ProcessPaymentAsync(requestModel, merchantId, tokenSource.Token);

        //assert
        responseModel.Should().NotBeNull();
        responseModel!.Id.Should().NotBeEmpty();
        responseModel.PaymentStatus.Should().Be(paymentStatus);
    }

    [Fact]
    public async Task ProcessPayment_ForInvalidCardDetails_ReturnsBadRequest()
    {
        //arrange
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var merchantId = Guid.NewGuid().ToString();
        var requestModel = GetPaymentInvalidRequestModel();

        //act
        var response = await CallProcessPaymentAsync(requestModel, merchantId, tokenSource.Token);

        //assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(PaymentStatusModel.Authorized)]
    [InlineData(PaymentStatusModel.Declined)]
    public async Task GetPayment_ForExistingPayment_ReturnsPaymentWithCorrectData(PaymentStatusModel paymentStatus)
    {
        //arrange
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var merchantId = Guid.NewGuid().ToString();
        var processPaymentRequestModel = paymentStatus == PaymentStatusModel.Authorized ? GetPaymentAuthorizedRequestModel() : GetPaymentDeclinedRequestModel();
        var responseModel = await ProcessPaymentAsync(processPaymentRequestModel, merchantId, tokenSource.Token);
        var paymentId = responseModel.Id;

        //act
        var paymentModel = await GetPaymentAsync(paymentId, merchantId, tokenSource.Token);

        //arrange
        paymentModel.Id.Should().Be(paymentId);
        paymentModel.Amount.Should().Be(processPaymentRequestModel.Amount);
        paymentModel.CardDetails.MaskedCardNumber.Should().Be(processPaymentRequestModel.CardNumber[^4..]);
        paymentModel.CardDetails.ExpiryMonth.Should().Be(processPaymentRequestModel.ExpiryMonth);
        paymentModel.CardDetails.ExpiryYear.Should().Be(processPaymentRequestModel.ExpiryYear);
    }

    [Fact]
    public async Task GetPayment_WithWrongMerchantId_ReturnsNotFound()
    {
        //arrange
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var merchantId1 = Guid.NewGuid().ToString();
        var merchantId2 = Guid.NewGuid().ToString();
        var paymentResponseModel = await ProcessPaymentAsync(GetPaymentAuthorizedRequestModel(), merchantId1, tokenSource.Token);

        //act
        var response = await CallGetPaymentAsync(paymentResponseModel.Id, merchantId2, tokenSource.Token);

        //arrange
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    private async Task<ProcessPaymentResponseModel> ProcessPaymentAsync(ProcessPaymentRequestModel requestModel, string merchantId, CancellationToken cancellationToken)
    {
        var response = await CallProcessPaymentAsync(requestModel, merchantId, cancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseModel = await response.Content.ReadFromJsonAsync<ProcessPaymentResponseModel>(cancellationToken: cancellationToken);
        responseModel.Should().NotBeNull();

        return responseModel!;
    }

    private async Task<HttpResponseMessage> CallProcessPaymentAsync(ProcessPaymentRequestModel requestModel,
        string merchantId, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/payments")
        {
            Content = JsonContent.Create(requestModel)
        };
        request.Headers.Add(ApiConstants.MerchantClientIdHeader, merchantId);

        return await _client.SendAsync(request, cancellationToken);
    }

    private async Task<PaymentModel> GetPaymentAsync(Guid paymentId, string merchantId, CancellationToken cancellationToken)
    {
        var response = await CallGetPaymentAsync(paymentId, merchantId, cancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var paymentModel = await response.Content.ReadFromJsonAsync<PaymentModel>();
        paymentModel.Should().NotBeNull();

        return paymentModel!;
    }
    
    private async Task<HttpResponseMessage> CallGetPaymentAsync(Guid paymentId, string merchantId, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/payments/{paymentId}");
        request.Headers.Add(ApiConstants.MerchantClientIdHeader, merchantId);

        return await _client.SendAsync(request, cancellationToken);
    }

    private ProcessPaymentRequestModel GetPaymentAuthorizedRequestModel()
    {
        return new ProcessPaymentRequestModel
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Cvv = "123",
            Amount = new MoneyModel(100, "826")
        };
    }

    private ProcessPaymentRequestModel GetPaymentDeclinedRequestModel()
    {
        return new ProcessPaymentRequestModel
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 1,
            ExpiryYear = 2026,
            Cvv = "456",
            Amount = new MoneyModel(60_000, "840")
        };
    }

    private ProcessPaymentRequestModel GetPaymentInvalidRequestModel()
    {
        return new ProcessPaymentRequestModel
        {
            CardNumber = "123", // wrong card number
            ExpiryMonth = 1,
            ExpiryYear = 2026,
            Cvv = "456",
            Amount = new MoneyModel(60_000, "840")
        };
    }
}