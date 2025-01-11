using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.Utils;

namespace PaymentGateway.WebApi.Tests;

public sealed class PaymentTests
{
    private static readonly Faker Faker = new();

    [Fact]
    public void CreatePayment_WithInvalidInput_ReturnsPaymentObject()
    {
        //arrange
        var merchantId = Guid.NewGuid().ToString();
        var cardDetails = CardDetails;
        var amount = Money.Create(150, "840").GetValue();

        //act
        var payment = new Payment(cardDetails, amount, merchantId);

        //assert
        payment.Should().NotBeNull();
        payment.Id.Should().NotBeEmpty();
        payment.MerchantId.Should().Be(merchantId);
        payment.Amount.Amount.Should().Be(150);
        payment.Amount.Currency.Code.Should().Be("840");
        payment.Amount.Currency.Name.Should().Be("USD");
        payment.CardDetails.CardNumber.Should().Be(cardDetails.CardNumber);
        payment.CardDetails.ExpiryMonth.Should().Be(cardDetails.ExpiryMonth);
        payment.CardDetails.ExpiryYear.Should().Be(cardDetails.ExpiryYear);
        payment.CardDetails.Cvv.Should().Be(cardDetails.Cvv);
        payment.CardDetails.GetMaskedCardNumber().Should().Be(cardDetails.CardNumber[^4..]);
        payment.Status.Should().Be(PaymentStatus.New);
    }

    [Fact]
    public void SetPaymentAuthorized_MovesPaymentToAuthorizedStatus()
    {
        //arrange
        var merchantId = Guid.NewGuid().ToString();
        var cardDetails = CardDetails;
        var amount = Money.Create(150, "840").GetValue();
        var payment = new Payment(cardDetails, amount, merchantId);
        var code = Guid.NewGuid().ToString();
        var paymentAuthorization = new PaymentAuthorization(code);

        //act
        payment.SetAuthorized(paymentAuthorization);

        //assert
        payment.Status.Should().Be(PaymentStatus.Authorized);
        payment.AuthorizationCode.Should().Be(code);
    }

    [Fact]
    public void SetPaymentDeclined_MovesPaymentToDeclinedStatus()
    {
        //arrange
        var merchantId = Guid.NewGuid().ToString();
        var cardDetails = CardDetails;
        var amount = Money.Create(150, "840").GetValue();
        var payment = new Payment(cardDetails, amount, merchantId);

        //act
        payment.SetDeclined();

        //assert
        payment.Status.Should().Be(PaymentStatus.Declined);
        payment.AuthorizationCode.Should().BeNull();
    }

    [Fact]
    public void SetPaymentDeclined_ForAlreadyAuthorizedPayment_ThrowsException()
    {
        //arrange
        var merchantId = Guid.NewGuid().ToString();
        var cardDetails = CardDetails;
        var amount = Money.Create(150, "840").GetValue();
        var payment = new Payment(cardDetails, amount, merchantId);
        var code = Guid.NewGuid().ToString();
        var paymentAuthorization = new PaymentAuthorization(code);
        payment.SetAuthorized(paymentAuthorization);

        //act
        var exception = Record.Exception(() => payment.SetDeclined());

        //assert
        exception.Should().NotBeNull();
    }

    private CardDetails CardDetails => CardDetails.Create(
        new FakeTimeProvider(DateTimeOffset.UtcNow),
        Faker.Random.String2(14, 19, "012345789"),
        Faker.Random.Number(1, 12),
        DateTimeOffset.UtcNow.Year + 1,
        Faker.Random.String2(3, 4, "012345789")).AsT0;
}