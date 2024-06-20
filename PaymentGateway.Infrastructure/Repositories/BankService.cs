using System.Net.Http.Json;

using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Models.Responses;

namespace PaymentGateway.Infrastructure.Repositories
{
    public class BankService : IBankService
    {
        private readonly IHttpClientFactory _clientFactory;
        HttpClient _httpClient => _clientFactory.CreateClient("Bank_Client");
        public BankService(IHttpClientFactory httpClientFactory)
        {
            _clientFactory = httpClientFactory;
        }

        public async Task<PostPaymentBankResponse> MakePayment(PostPaymentBankRequest request)
        {
            try
            {
                using var httpResponseMessage = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress, request);
                PostPaymentBankResponse paymentResponse = null;
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    paymentResponse = await httpResponseMessage.Content.ReadFromJsonAsync<PostPaymentBankResponse>();
                }
                return paymentResponse;
            }
            catch (HttpRequestException) // Non success
            {
                //Logging: error calling the api
                throw;
            }
            catch (Exception) 
            {
                //Logging: exception
                throw;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
