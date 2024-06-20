using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Domain.Models.Requests;

public class PostPaymentBankRequest
{
    [Required]
    public string Card_Number { get; set; }
    [Required]
    public string Expiry_Date { get; set; }
    [Required]
    public string Currency { get; set; }
    [Required]
    public int Amount { get; set; }
    [Required]
    public string Cvv { get; set; }
}