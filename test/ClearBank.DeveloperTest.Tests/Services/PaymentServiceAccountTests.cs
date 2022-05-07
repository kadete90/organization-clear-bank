using AutoFixture.Xunit2;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Moq;
using Xunit;
// ReSharper disable ExpressionIsAlwaysNull

namespace ClearBank.DeveloperTest.Tests.Services
{
    public sealed class PaymentServiceTests
    {
        private readonly Mock<IDataStore> _dataStore;

        private readonly IPaymentService _sut;

        public PaymentServiceTests()
        {
            _dataStore = new Mock<IDataStore>();

            _sut = new PaymentService(_dataStore.Object);
        }

        [Fact]
        public void Given_null_request_When_MakePayment_Then_should_throw()
        {
            // Given
            MakePaymentRequest request = null;

            // When
            Action result = () => _sut.MakePayment(request);

            // Then
            result.Should().ThrowExactly<ArgumentNullException>()
                .WithParameterName("request");
        }

        [Theory, AutoData]
        public void Given_null_account_When_MakePayment_Then_should_return_failure(
            MakePaymentRequest request)
        {
            // Given
            _dataStore.Setup(d => d.GetAccount(
                    It.Is<string>(s => s == request.DebtorAccountNumber))
                )
                .Returns((Account)null);

            // When
            var result = _sut.MakePayment(request);

            // Then
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
        }

        [Theory, AutoData]
        public void Given_valid_request_with_bacs_scheme_When_MakePayment_Then_should_return_success(
            MakePaymentRequest request, Account account)
        {
            // Given
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs;
            request.PaymentScheme = PaymentScheme.Bacs;

            _dataStore.Setup(d => d.GetAccount(
                    It.Is<string>(s => s == request.DebtorAccountNumber))
                )
                .Returns(account);

            // When
            var result = _sut.MakePayment(request);

            // Then
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Theory, AutoData]
        public void Given_valid_request_with_faster_payments_scheme_When_MakePayment_Then_should_return_success(
            MakePaymentRequest request, Account account)
        {
            // Given
            account.Balance = 20;
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;

            request.Amount = 10;
            request.PaymentScheme = PaymentScheme.FasterPayments;

            _dataStore.Setup(d => d.GetAccount(
                    It.Is<string>(s => s == request.DebtorAccountNumber))
                )
                .Returns(account);

            // When
            var result = _sut.MakePayment(request);

            // Then
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Theory, AutoData]
        public void Given_valid_request_with_chaps_scheme_When_MakePayment_Then_should_return_success(
            MakePaymentRequest request, Account account)
        {
            // Given
            account.Status = AccountStatus.Live;
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            request.PaymentScheme = PaymentScheme.Chaps;

            _dataStore.Setup(d => d.GetAccount(
                    It.Is<string>(s => s == request.DebtorAccountNumber))
                )
                .Returns(account);

            // When
            var result = _sut.MakePayment(request);

            // Then
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Theory, AutoData]
        public void Given_insufficient_balance_on_faster_payments_scheme_When_MakePayment_Then_should_return_failure(
           MakePaymentRequest request, Account account)
        {
            // Given
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;
            request.PaymentScheme = PaymentScheme.FasterPayments;

            account.Balance = 10;
            request.Amount = 20;

            _dataStore.Setup(d => d.GetAccount(
                    It.Is<string>(s => s == request.DebtorAccountNumber))
                )
                .Returns(account);

            // When
            var result = _sut.MakePayment(request);

            // Then
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
        }

        [Theory]
        [InlineAutoData(AccountStatus.InboundPaymentsOnly)]
        [InlineAutoData(AccountStatus.Disabled)]
        public void Given_non_live_status_on_chaps_scheme_When_MakePayment_Then_should_return_failure(
            AccountStatus accountStatus,
            MakePaymentRequest request, Account account)
        {
            // Given
            account.Status = accountStatus;
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            request.PaymentScheme = PaymentScheme.Chaps;

            _dataStore.Setup(d => d.GetAccount(
                    It.Is<string>(s => s == request.DebtorAccountNumber))
                )
                .Returns(account);

            // When
            var result = _sut.MakePayment(request);

            // Then
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
        }


        [Theory, AutoData]
        public void Given_invalid_PaymentScheme_When_MakePayment_Then_should_return_failure(
            MakePaymentRequest request, Account account)
        {
            // Given
            request.PaymentScheme = 0;

            _dataStore.Setup(d => d.GetAccount(
                    It.Is<string>(s => s == request.DebtorAccountNumber))
                )
                .Returns(account);

            // When
            var result = _sut.MakePayment(request);

            // Then
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
        }

        [Theory]
        [InlineAutoData(AllowedPaymentSchemes.FasterPayments, PaymentScheme.Bacs)]
        [InlineAutoData(AllowedPaymentSchemes.FasterPayments, PaymentScheme.Chaps)]
        [InlineAutoData(AllowedPaymentSchemes.Bacs, PaymentScheme.Chaps)]
        [InlineAutoData(AllowedPaymentSchemes.Bacs, PaymentScheme.FasterPayments)]
        [InlineAutoData(AllowedPaymentSchemes.Chaps, PaymentScheme.Bacs)]
        [InlineAutoData(AllowedPaymentSchemes.Chaps, PaymentScheme.FasterPayments)]
        public void Given_invalid_scheme_When_MakePayment_Then_should_return_failure(
            AllowedPaymentSchemes allowedPaymentSchemes, PaymentScheme paymentScheme,
            MakePaymentRequest request, Account account)
        {
            // Given
            account.AllowedPaymentSchemes = allowedPaymentSchemes;
            request.PaymentScheme = paymentScheme;

            _dataStore.Setup(d => d.GetAccount(
                    It.Is<string>(s => s == request.DebtorAccountNumber))
                )
                .Returns(account);

            // When
            var result = _sut.MakePayment(request);

            // Then
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
        }
    }
}