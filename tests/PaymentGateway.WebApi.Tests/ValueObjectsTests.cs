using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.Utils;

namespace PaymentGateway.WebApi.Tests;

public sealed class ValueObjectsTests
{
    private static readonly Faker Faker = new();

    [Theory]
    [InlineData(150, "840", false)]
    [InlineData(1000, "978", false)]
    [InlineData(2500, "826", false)]
    [InlineData(100, "124", true)]
    [InlineData(-100, "840", true)]
    public void CreateMoney(long amount, string currencyCode, bool isInvalid)
    {
        //act
        var money = Money.Create(amount, currencyCode);

        //assert
        money.IsInvalid().Should().Be(isInvalid);
    }

    [Theory]
    [InlineData("0bb07405-6d44-4b50-a14f-7ae0beff13ad", false)]
    [InlineData("", true)]
    public void CreatePaymentAuthorization(string code, bool shouldThrow)
    {
        //arrange
        var createPaymentAuthorization = () => new PaymentAuthorization(code);

        //act
        var exception = Record.Exception(createPaymentAuthorization);

        //assert
        if (shouldThrow)
            exception.Should().NotBeNull();
        else
            exception.Should().BeNull();

    }

    [Theory]
    [MemberData(nameof(CardDetailsTestCases))]
    public void CreateCardDetails_WithSpecificInput_ReturnsEitherCardDetailsInstanceOrError(
        TimeProvider timeProvider,
        string cardNumber,
        int expiryMonth,
        int expiryYear,
        string cvv,
        bool isInvalid)
    {
        //act
        var cardDetailsOrError = CardDetails.Create(timeProvider, cardNumber, expiryMonth, expiryYear, cvv);

        //assert
        cardDetailsOrError.IsInvalid().Should().Be(isInvalid);
    }

    public static IEnumerable<object[]> CardDetailsTestCases => new List<object[]>
    {
        // Valid test cases
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(14, 19), NumberFromRange(1, 12), NextYear, DigitSequence(3, 4), false },
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(14, 19), CurrentMonth, CurrentYear, DigitSequence(3, 4), false },
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(14, 19), NextMonth, CurrentYear, DigitSequence(3, 4), false },

        // Invalid test cases
        // -- Invalid card number patterns
        new object[] { new FakeTimeProvider(DateTime.UtcNow), "", CurrentMonth, NextYear, DigitSequence(3, 4), true },
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(13), CurrentMonth, NextYear, DigitSequence(3, 4), true },
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(20), CurrentMonth, NextYear, DigitSequence(3, 4), true },
        new object[] { new FakeTimeProvider(DateTime.UtcNow), LetterDigitsSequence(14, 19), CurrentMonth, NextYear, DigitSequence(3, 4), true },

        // -- Expiry month out of range
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(14, 19), PreviousMonth, CurrentYear, DigitSequence(3, 4), true },

        // -- Expiry year in the past
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(14, 19), CurrentMonth, PreviousYear, DigitSequence(3, 4), true },

        // -- Invalid CVV
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(14, 19), CurrentMonth, NextYear, "", true },
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(14, 19), CurrentMonth, NextYear, DigitSequence(2), true },
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(14, 19), CurrentMonth, NextYear, DigitSequence(5), true },
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(14, 19), CurrentMonth, NextYear, LetterDigitsSequence(3, 4), true },

        // -- All invalid
        new object[] { new FakeTimeProvider(DateTime.UtcNow), DigitSequence(5, 13), PreviousMonth, CurrentYear, DigitSequence(2), true }
    };

    private static string DigitSequence(int length) => DigitSequence(length, length);

    private static string DigitSequence(int minLength, int maxLength)
    {
        return Faker.Random.String2(minLength, maxLength, "0123456789");
    }

    private static string LetterDigitsSequence(int minLength, int maxLength)
    {
        return Faker.Random.String2(minLength, maxLength, "abcdefghijklmnopqrstuvwxyz0123456789");
    }

    private static int NumberFromRange(int min, int max)
    {
        return Faker.Random.Number(min, max);
    }

    private static int CurrentMonth => DateTime.UtcNow.Month;
    private static int CurrentYear => DateTime.UtcNow.Year;
    private static int NextYear => DateTime.UtcNow.Year + 1;
    private static int NextMonth => DateTime.UtcNow.Month + 1;
    private static int PreviousMonth => DateTime.UtcNow.Month - 1;
    private static int PreviousYear => DateTime.UtcNow.Year - 1;
}