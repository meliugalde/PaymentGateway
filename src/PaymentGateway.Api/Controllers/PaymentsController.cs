using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Responses;
using PaymentGateway.Domain.Services;

using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;


namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateInputAttribute))]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentsService _paymentsService;
    private readonly ILogger<PaymentsController> _logger;


    public PaymentsController(IPaymentsService paymentsService, ILogger<PaymentsController> logger)
    {
        _paymentsService = paymentsService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> ProcessPaymentAsync([FromBody] PostPaymentRequest postPaymentRequest)
    {
        if (!postPaymentRequest.IsValid())
        {
            return new BadRequestObjectResult(new PaymentResponse { Status = Domain.Enum.PaymentStatus.Rejected });
        }
        try
        {
            PaymentResponse postPaymentResponse = await _paymentsService.ProcessPayment(postPaymentRequest);
            
            _paymentsService.SavePayment(postPaymentResponse);

            if (postPaymentResponse.Status == Domain.Enum.PaymentStatus.Declined || postPaymentResponse.Status == Domain.Enum.PaymentStatus.Rejected)
            {
                return new BadRequestObjectResult(postPaymentResponse);
            }

            return new OkObjectResult(postPaymentResponse);
            
        }
        catch (Exception ex) 
        {
            _logger.LogError("Error: ProcessPaymentAsync");

            return StatusCode(500, "Internal server error");
        }
        
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentResponse>> GetPaymentAsync(Guid id)
    {

        if (id == Guid.Empty)
        {
            _logger.LogError("Id must be provided.");

            return new BadRequestObjectResult("Id must be provided.");
        }
        try
        {
            _logger.LogInformation("GetPaymentAsync: Id : {id}", id);

            var payment = _paymentsService.GetPayment(id);
            if (payment == null) 
            {
                _logger.LogError("Payment with id: {id} not found.", id);

                return new NotFoundObjectResult("Payment not found.");
            }

            return new OkObjectResult(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error: GetPaymentAsync: Id : {id}", id);

            return StatusCode(500, "Internal server error");
        }
        
    }
}