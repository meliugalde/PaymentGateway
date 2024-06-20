using System;

using PaymentGateway.Domain.Enum;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Responses;
using PaymentGateway.Domain.Helper;

namespace PaymentGateway.Domain.Services
{
    public class PaymentsService : IPaymentsService
    {
        public IPaymentsRepository _paymentsRepository { get; set; }
        public IBankService _bankService { get; set; }

        public PaymentsService(IPaymentsRepository paymentsRepository, IBankService bankService)
        {
            _paymentsRepository = paymentsRepository;
            _bankService = bankService;
        }
        public PaymentResponse GetPayment(Guid id)
        {
            return _paymentsRepository.Get(id);
        }

        public async Task<PaymentResponse> ProcessPayment(PostPaymentRequest postPaymentRequest)
        {
            PostPaymentBankRequest postPaymentBankRequest = new PostPaymentBankRequest
            {
                Card_Number = postPaymentRequest.CardNumber,
                Amount = postPaymentRequest.Amount,
                Currency = postPaymentRequest.Currency,
                Cvv = postPaymentRequest.Cvv,
                Expiry_Date = FormatHelper.FormatDate(postPaymentRequest.ExpiryMonth, postPaymentRequest.ExpiryYear)
            };

            PostPaymentBankResponse postPaymentBankResponse = await _bankService.MakePayment(postPaymentBankRequest);

            PaymentResponse postPaymentResponse = new PaymentResponse
            {
                Id = Guid.NewGuid(),
                Amount = postPaymentRequest.Amount,
                CardNumberLastFour = FormatHelper.GetLastFourDigits(postPaymentRequest.CardNumber),
                Currency = postPaymentRequest.Currency,
                ExpiryMonth = postPaymentRequest.ExpiryMonth,
                ExpiryYear = postPaymentRequest.ExpiryYear,
            };

            if (postPaymentBankResponse == null)
            {
                postPaymentResponse.Status = PaymentStatus.Rejected;
            }
            else
            {
                postPaymentResponse.Status = postPaymentBankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
                postPaymentResponse.StatusString = postPaymentBankResponse.Authorized ? PaymentStatus.Authorized.ToString() : PaymentStatus.Declined.ToString();
            }

            return postPaymentResponse;
        }

        public void SavePayment(PaymentResponse paymentResponse)
        {
            _paymentsRepository.Add(paymentResponse);
        }
    }
}
