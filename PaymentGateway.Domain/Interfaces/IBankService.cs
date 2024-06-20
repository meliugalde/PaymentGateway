using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Responses;

namespace PaymentGateway.Domain.Interfaces
{
    public interface IBankService : IDisposable
    {
        Task<PostPaymentBankResponse> MakePayment(PostPaymentBankRequest request);
    }
}