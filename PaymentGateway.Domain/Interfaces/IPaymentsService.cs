using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Responses;

namespace PaymentGateway.Domain.Interfaces
{
    public interface IPaymentsService
    {
        PaymentResponse GetPayment(Guid id);
        Task<PaymentResponse> ProcessPayment(PostPaymentRequest postPaymentRequest);
        void SavePayment(PaymentResponse postPaymentResponse);
    }
}
