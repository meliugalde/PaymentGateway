using Moq;

using PaymentGateway.Api.Tests;
using PaymentGateway.Domain.Enum;
using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Responses;

namespace PaymentGateway.Domain.Tests
{
    public class PaymentsServiceTests : IClassFixture<PaymentServiceFixture>
    {
        private PaymentServiceFixture fixture;

        public PaymentsServiceTests(PaymentServiceFixture _fixture)
        {
            fixture = _fixture;
        }

        [Fact]
        public void GetPayment_ReturnsPayment_whenValidId()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            var payment = new PaymentResponse
            {
                Id = id,
                Status = PaymentStatus.Authorized,
                CardNumberLastFour = "8877",
                ExpiryMonth = "12",
                ExpiryYear = "2025",
                Amount = 100,
                Currency = "GBP",
            };

            fixture._mockPaymentsRepository.Setup(x => x.Get(id))
                .Returns(payment);

            //Act
            var paymentResponse = fixture._paymentsService.GetPayment(id);

            //Assert
            Assert.NotNull(paymentResponse);
            Assert.Equal(paymentResponse, payment);
            fixture._mockPaymentsRepository.Verify(x =>x.Get(id));
        }

        [Fact]
        public void SavePayment_CallsRepository_withPaymentResponse()
        {
          
            //Arrange
            Guid id = Guid.NewGuid();
            var postPayment = new PaymentResponse
            {
                ExpiryYear = "2025",
                ExpiryMonth = "4",
                Amount = 100,
                CardNumberLastFour = "8877",
                Currency = "GBP",
                Status = PaymentStatus.Authorized,
            };

            var getPayment = new PaymentResponse
            {
                Id = id,
                ExpiryYear = "2025",
                ExpiryMonth = "4",
                Amount = 100,
                CardNumberLastFour = "8877",
                Currency = "GBP",
                Status = PaymentStatus.Authorized,
            };

            //Act
            fixture._paymentsService.SavePayment(postPayment);

            //Assert
            fixture._mockPaymentsRepository.Verify(x => x.Add(postPayment), Times.Once());
           
        }

        [Fact]
        public async Task ProcessPayment_CallsBankService_withPaymentRequest_ReturnsAuthorized()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            PostPaymentRequest postPaymentRequest = new PostPaymentRequest
            {
                CardNumber = "2222405343248877",
                ExpiryMonth = "4",
                ExpiryYear = "2025",
                Amount = 100,
                Currency = "GBP",
                Cvv = "123"
            };

            var processPaymentResponse = new PostPaymentBankResponse
            {
                Authorized = true,
                Authorization_code = new Guid("0bb07405-6d44-4b50-a14f-7ae0beff13ad")
            };

            fixture._mockBankService.Setup(x => x.MakePayment(It.IsAny<PostPaymentBankRequest>())).ReturnsAsync(processPaymentResponse);

            //Act
            var response = await fixture._paymentsService.ProcessPayment(postPaymentRequest);

            //Assert
            Assert.True(response.Id != Guid.Empty);
            Assert.True(response.CardNumberLastFour == "8877");
            Assert.True(response.Status == PaymentStatus.Authorized);

        }

        [Fact]
        public async Task ProcessPayment_CallsBankService_withPaymentRequest_ReturnsDecline()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            PostPaymentRequest postPaymentRequest = new PostPaymentRequest
            {
                CardNumber = "2222405343248877",
                ExpiryMonth = "4",
                ExpiryYear = "2025",
                Amount = 100,
                Currency = "GBP",
                Cvv = "123"
            };

            var processPaymentResponse = new PostPaymentBankResponse
            {
                Authorized = false,
                Authorization_code = new Guid("0bb07405-6d44-4b50-a14f-7ae0beff13ad")
            };

            fixture._mockBankService.Setup(x => x.MakePayment(It.IsAny<PostPaymentBankRequest>())).ReturnsAsync(processPaymentResponse);

            //Act
            var response = await fixture._paymentsService.ProcessPayment(postPaymentRequest);

            //Assert
            Assert.True(response.Id != Guid.Empty);
            Assert.True(response.CardNumberLastFour == "8877");
            Assert.True(response.Status == PaymentStatus.Declined);

        }

        [Fact]
        public async Task ProcessPayment_CallsBankService_withPaymentRequest_ReturnsRejected()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            PostPaymentRequest postPaymentRequest = new PostPaymentRequest
            {
                CardNumber = "2222405343248877",
                ExpiryMonth = "4",
                ExpiryYear = "2025",
                Amount = 100,
                Currency = "GBP",
                Cvv = "123"
            };

            PostPaymentBankResponse processPaymentResponse = null;

            fixture._mockBankService.Setup(x => x.MakePayment(It.IsAny<PostPaymentBankRequest>())).ReturnsAsync(processPaymentResponse);

            //Act
            var response = await fixture._paymentsService.ProcessPayment(postPaymentRequest);

            //Assert
            Assert.True(response.Id != Guid.Empty);
            Assert.True(response.CardNumberLastFour == "8877");
            Assert.True(response.Status == PaymentStatus.Rejected);

        }
    }
}