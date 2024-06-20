using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;
using Moq.Protected;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Responses;
using PaymentGateway.Domain.Services;
using PaymentGateway.Infrastructure.Repositories;
namespace PaymentGateway.Api.Tests;
public class PaymentGatewayIntegrationTests
{
    private readonly Random _random = new();
    private Mock<HttpMessageHandler> _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2025, 2030).ToString(),
            ExpiryMonth = _random.Next(1, 12).ToString(),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999).ToString(),
            Currency = "GBP",
            Status= Domain.Enum.PaymentStatus.Authorized,
        };

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://www.testing.com")
        };

        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        mockHttpClientFactory.Setup(_ => _.CreateClient("Bank_Client")).Returns(httpClient);

        IPaymentsRepository paymentsRepository = new PaymentsRepository();

        IBankService bankService = new BankService(mockHttpClientFactory.Object);

        IPaymentsService paymentService = new PaymentsService(paymentsRepository, bankService);
        paymentsRepository.Add(payment);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();

        var client = webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(paymentsRepository);
                services.AddSingleton(bankService);
                services.AddSingleton(paymentService);
            });
        })
        .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.True(payment.Id.Equals(paymentResponse.Id));
        Assert.True(paymentResponse.Status == Domain.Enum.PaymentStatus.Authorized);
    }

    [Fact]
    public async Task PostAPaymentSuccessfully()
    {
        // Arrange
        PostPaymentRequest postPaymentRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = "12",
            ExpiryYear = "2025",
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

        _httpMessageHandlerMock.Protected()
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

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://www.testing.com")
        };

        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        mockHttpClientFactory.Setup(_ => _.CreateClient("Bank_Client")).Returns(httpClient);

        IPaymentsRepository paymentsRepository = new PaymentsRepository();

        IBankService bankService = new BankService(mockHttpClientFactory.Object);

        IPaymentsService paymentService = new PaymentsService(paymentsRepository, bankService);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();

        var client = webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(paymentsRepository);
                services.AddSingleton(bankService);
                services.AddSingleton(paymentService);
            });
        })
        .CreateClient();

        // Act
        var response = await client.PostAsJsonAsync($"/api/Payments/", postPaymentRequest); 
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.False(paymentResponse.Id == Guid.Empty);
        Assert.True(paymentResponse.Status == Domain.Enum.PaymentStatus.Authorized);
    }

    [Fact]
    public async Task PostAPayment_ValidationReturnsBadRequest_Rejected()
    {
        // Arrange
        PostPaymentRequest postPaymentRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = "1",     
            ExpiryYear = "2020",    // Year in the past
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

        _httpMessageHandlerMock.Protected()
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

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://www.testing.com")
        };

        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        mockHttpClientFactory.Setup(_ => _.CreateClient("Bank_Client")).Returns(httpClient);

        IPaymentsRepository paymentsRepository = new PaymentsRepository();

        IBankService bankService = new BankService(mockHttpClientFactory.Object);

        IPaymentsService paymentService = new PaymentsService(paymentsRepository, bankService);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();

        var client = webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(paymentsRepository);
                services.AddSingleton(bankService);
                services.AddSingleton(paymentService);
            });
        })
        .CreateClient();

        // Act
        var response = await client.PostAsJsonAsync($"/api/Payments/", postPaymentRequest);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(paymentResponse.Status == Domain.Enum.PaymentStatus.Rejected);
    }

    [Fact]
    public async Task PostAPayment_Validation_ReturnsBadRequest()
    {
        // Arrange
        PostPaymentRequest postPaymentRequest = new PostPaymentRequest
        {
            CardNumber = "22224053", //invalid card number
            ExpiryMonth = "13",     // invalid month
            ExpiryYear = "2025",
            Amount = 100,
            Currency = "currency", // invalid currency
            Cvv = "12"  //invalid cvv
        };

        var expectedResponse = new PostPaymentBankResponse
        {
            Authorized = true,
            Authorization_code = new Guid("0bb07405-6d44-4b50-a14f-7ae0beff13ad")
        };

        var jsonStringResponse = JsonSerializer.Serialize(expectedResponse);

        _httpMessageHandlerMock.Protected()
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

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://www.testing.com")
        };

        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        mockHttpClientFactory.Setup(_ => _.CreateClient("Bank_Client")).Returns(httpClient);

        IPaymentsRepository paymentsRepository = new PaymentsRepository();

        IBankService bankService = new BankService(mockHttpClientFactory.Object);

        IPaymentsService paymentService = new PaymentsService(paymentsRepository, bankService);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();

        var client = webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(paymentsRepository);
                services.AddSingleton(bankService);
                services.AddSingleton(paymentService);
            });
        })
        .CreateClient();

        // Act
        var response = await client.PostAsJsonAsync($"/api/Payments/", postPaymentRequest);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
