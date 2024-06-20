using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using PaymentGateway.Domain.Enum;
using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Responses;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests : IClassFixture<PaymentControllerFixture>
{
    
    private PaymentControllerFixture fixture;

    public PaymentsControllerTests(PaymentControllerFixture _fixture)
    {
        fixture = _fixture;
    }

    [Fact]
    public async Task GetPaymentAsync_ReturnsPaymentAndOkStatus_whenIdValid()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var payment = new PaymentResponse
        {
            Id = id,
            ExpiryYear = fixture._random.Next(2025, 2030).ToString(),
            ExpiryMonth = fixture._random.Next(1, 12).ToString(),
            Amount = fixture._random.Next(1, 10000),
            CardNumberLastFour = fixture._random.Next(1111, 9999).ToString(),
            Currency = "GBP",
            Status = PaymentStatus.Authorized,
        };

        fixture._mockPaymentsService.Setup(x => x.GetPayment(id)).Returns(payment);
        
        // Act
        var response = await fixture._paymentsController.GetPaymentAsync(id);

        // Assert
        Assert.IsType<OkObjectResult>(response.Result);
        var paymentResponse = response.Result as OkObjectResult;
        
        Assert.Equal(paymentResponse.Value, payment);

        fixture._mockLogger.Verify(
            x => x.Log(
               It.Is<LogLevel>(l => l == LogLevel.Information),
               It.IsAny<EventId>(),
               It.IsAny<It.IsAnyType>(),
               It.IsAny<Exception>(),
               It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
    }

    [Fact]
    public async Task GetPaymentAsync_ReturnsBadRequest_WhenIdEmpty()
    {
        
        // Arrange
        Guid id = Guid.Empty;
       
        // Act
        var response = await fixture._paymentsController.GetPaymentAsync(id);

        // Assert
        Assert.IsType<BadRequestObjectResult>(response.Result);
        var paymentResponse = response.Result as BadRequestObjectResult;

        Assert.Equal((int)HttpStatusCode.BadRequest, paymentResponse.StatusCode.Value);

        fixture._mockLogger.Verify(
            x => x.Log(
               It.Is<LogLevel>(l => l == LogLevel.Error),
               It.IsAny<EventId>(),
               It.IsAny<It.IsAnyType>(),
               It.IsAny<Exception>(),
               It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
    }

    [Fact]
    public async Task GetPaymentAsync_ReturnsInternalServerError_whenErrorOcurrs()
    {
       
        // Arrange
        Guid id = Guid.NewGuid();
        fixture._mockPaymentsService.Setup(x => x.GetPayment(id))
            .Throws(new Exception("Internal server error"));

        // Act
        var response = await fixture._paymentsController.GetPaymentAsync(id);

        // Assert
        Assert.IsType<ObjectResult>(response.Result);
        var paymentResponse = response.Result as ObjectResult;

        Assert.Equal((int)HttpStatusCode.InternalServerError, paymentResponse.StatusCode.Value);

        fixture._mockLogger.Verify(
            x => x.Log(
               It.Is<LogLevel>(l => l == LogLevel.Error),
               It.IsAny<EventId>(),
               It.IsAny<It.IsAnyType>(),
               It.IsAny<Exception>(),
               It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
    }

    [Fact]
    public async Task GetPaymentAsync_Returns404_WhenPaymentNotFound()
    {
        PaymentResponse payment = null;
        // Arrange
        Guid id = Guid.NewGuid();
        fixture._mockPaymentsService.Setup(x => x.GetPayment(id))
            .Returns(payment);

        // Act
        var response = await fixture._paymentsController.GetPaymentAsync(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(response.Result);
        var paymentResponse = response.Result as NotFoundObjectResult;

        Assert.Equal((int)HttpStatusCode.NotFound, paymentResponse.StatusCode.Value);

        fixture._mockLogger.Verify(
            x => x.Log(
               It.Is<LogLevel>(l => l == LogLevel.Error),
               It.IsAny<EventId>(),
               It.IsAny<It.IsAnyType>(),
               It.IsAny<Exception>(),
               It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsOk_WhenPaymentAuthorized()
    {
        //Arrange
        PostPaymentRequest PostPaymentRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = "12",
            ExpiryYear = "2025",
            Amount = 100,
            Currency = "GBP",
            Cvv = "123"
        };
        PaymentResponse processPaymentResponse = new PaymentResponse
        {
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = "8877",
            ExpiryMonth = "12",
            ExpiryYear = "2025",
            Amount = 100,
            Currency = "GBP",
            Id = Guid.NewGuid(),
        };
        fixture._mockPaymentsService.Setup(x => x.ProcessPayment(PostPaymentRequest))
            .Returns(Task.FromResult(processPaymentResponse));

        //Act
        var response = await fixture._paymentsController.ProcessPaymentAsync(PostPaymentRequest);

        //Assert
        Assert.IsType<OkObjectResult>(response.Result);
        var paymentResponse = response.Result as OkObjectResult;
        
        var processPaymentResult = paymentResponse.Value as PaymentResponse;

        Assert.Equal((int)HttpStatusCode.OK, paymentResponse.StatusCode.Value);
        Assert.Equal(processPaymentResult.Status, PaymentStatus.Authorized);
    }

    [Fact]
    public async Task ProcessPaymentAsync_SavePayment_WhenPaymentProcessed()
    {
        //Arrange
        PostPaymentRequest postPaymentRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = "1",
            ExpiryYear = "2026",
            Amount = 60000,
            Currency = "USD",
            Cvv = "456"
        };
        PaymentResponse processPaymentResponse = new PaymentResponse { Status = Domain.Enum.PaymentStatus.Declined };
        fixture._mockPaymentsService.Setup(x => x.ProcessPayment(postPaymentRequest))
            .Returns(Task.FromResult(processPaymentResponse));

        //Act
        var response = await fixture._paymentsController.ProcessPaymentAsync(postPaymentRequest);

        //Assert
        Assert.IsType<BadRequestObjectResult>(response.Result);
        var paymentResponse = response.Result as BadRequestObjectResult;

        var processPaymentResult = paymentResponse.Value as PaymentResponse;

        Assert.Equal((int)HttpStatusCode.BadRequest, paymentResponse.StatusCode.Value);
        Assert.Equal(processPaymentResult.Status, Domain.Enum.PaymentStatus.Declined);

        fixture._mockPaymentsService.Verify(x => x.SavePayment(processPaymentResult), Times.Once());
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsBadRequest_WhenPaymentDeclined()
    {
        //Arrange
        PostPaymentRequest postPaymentRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = "1",
            ExpiryYear = "2026",
            Amount = 60000,
            Currency = "USD",
            Cvv = "456"
        };
        
        PaymentResponse processPaymentResponse = new PaymentResponse 
        { 
            Status = PaymentStatus.Declined,
            CardNumberLastFour = "8112",
            ExpiryMonth = "1",
            ExpiryYear = "2026",
            Amount = 60000,
            Currency = "USD",
            Id = Guid.NewGuid(),
        };
        fixture._mockPaymentsService.Setup(x => x.ProcessPayment(postPaymentRequest))
            .Returns(Task.FromResult(processPaymentResponse));

        //Act
        var response = await fixture._paymentsController.ProcessPaymentAsync(postPaymentRequest);

        //Assert
        Assert.IsType<BadRequestObjectResult>(response.Result);
        var paymentResponse = response.Result as BadRequestObjectResult;

        var processPaymentResult = paymentResponse.Value as PaymentResponse;

        Assert.Equal((int)HttpStatusCode.BadRequest, paymentResponse.StatusCode.Value);
        Assert.Equal(processPaymentResult.Status, PaymentStatus.Declined);
    }

   
}