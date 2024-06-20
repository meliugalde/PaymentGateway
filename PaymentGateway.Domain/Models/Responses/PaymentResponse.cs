using PaymentGateway.Domain.Enum;

namespace PaymentGateway.Domain.Models.Responses;

public class PaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string StatusString { get; set; }
    public string CardNumberLastFour { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
}
