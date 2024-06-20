using System.Net.Http;
using System.Text;
using System.Text.Json;

using Moq;
using Moq.Protected;

using PaymentGateway.Api.Tests;
using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Responses;
using PaymentGateway.Infrastructure.Repositories;

using Xunit;

namespace PaymentGateway.Infra.Tests
{
    public class BankServiceTests : IClassFixture<BankServiceFixture>
    {
        private BankServiceFixture fixture;

        public BankServiceTests(BankServiceFixture _fixture)
        {
            fixture = _fixture;   
        }

        [Fact]
        public async Task MakePayment_CallsApi_ReturnsPaymentResponse()
        {
            //Arrange
            PostPaymentBankRequest paymentBankRequest = new PostPaymentBankRequest
            {
                Card_Number = "2222405343248877",
                Expiry_Date = "4/2025",
                Amount = 100,
                Currency = "GBP",
                Cvv = "123"
            };

            var expectedResponse = new PostPaymentBankResponse
            {
                Authorized = true,
                Authorization_code = new Guid("0bb07405-6d44-4b50-a14f-7ae0beff13ad")
            };

            var jsonStringResponse = JsonSerializer.Serialize(expectedResponse);

            fixture._httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("http://www.testing.com")
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(jsonStringResponse, Encoding.UTF8, "application/json")
                });

            //Act
            PostPaymentBankResponse result = await fixture.bankService.MakePayment(paymentBankRequest);

            //Assert
            Assert.Equal(expectedResponse.Authorized, result.Authorized); 
            Assert.Equal(expectedResponse.Authorization_code, result.Authorization_code);
        }


        [Fact]
        public async Task MakePayment_CallsApi_ThrowsException()
        {
            //Arrange
            PostPaymentBankRequest paymentBankRequest = new PostPaymentBankRequest
            {
                Card_Number = "2222405343248877",
                Expiry_Date = "4/2025",
                Amount = 100,
                Currency = "GBP",
                Cvv = "123"
            };

            var expectedResponse = new PostPaymentBankResponse
            {
                Authorized = true,
                Authorization_code = new Guid("0bb07405-6d44-4b50-a14f-7ae0beff13ad")
            };

            var jsonStringResponse = JsonSerializer.Serialize(expectedResponse);

            fixture._httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("http://www.testing.com")
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException());

            //Act
            Assert.ThrowsAsync<HttpRequestException>(() => fixture.bankService.MakePayment(paymentBankRequest));
        }
    }
}