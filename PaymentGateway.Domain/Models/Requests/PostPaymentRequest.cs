using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Domain.Models.Requests;

public class PostPaymentRequest
{
    [Required(ErrorMessage = "The field is required.")]
    [RegularExpression("^[0-9]{14,19}$", ErrorMessage = "Value must be between 1-12 characters long")]
    public string CardNumber { get; set; }
    
    [Required]
    [Range(1, 12, ErrorMessage = "Value must be between 1 and 12.")]
    public string ExpiryMonth { get; set; }
    
    [Required]
    public string ExpiryYear { get; set; }
    
    [Required]
    [RegularExpression("(USD|GBP|EUR)", ErrorMessage = "Value must be USD, GBP or EUR")]
    public string Currency { get; set; }
    
    [Required]
    public int Amount { get; set; }
    
    [Required]
    [RegularExpression("^[0-9]{3,4}$",ErrorMessage = "Value must be between 3 and 4 characters long")]
    public string Cvv { get; set; }


    public bool IsValid()
    {
        //TODO: Add amount validation
        return Helper.FormatHelper.ValidateMonth(ExpiryMonth, out int expireMonthInt) &&
                Helper.FormatHelper.ValidateYear(ExpiryYear, out int expireYearInt) &&
                Helper.FormatHelper.ValidateExpiryDate(expireMonthInt, expireYearInt);
    }
   
}