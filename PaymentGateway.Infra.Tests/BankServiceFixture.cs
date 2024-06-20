using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Domain.Services;
using PaymentGateway.Infrastructure.Repositories;

namespace PaymentGateway.Api.Tests
{
   
    public class BankServiceFixture
    {

        public BankService bankService;
        public Mock<HttpMessageHandler> _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);


        public BankServiceFixture()
        {
            //_httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://www.testing.com")
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            mockHttpClientFactory.Setup(_ => _.CreateClient("Bank_Client")).Returns(httpClient);

            bankService = new BankService(mockHttpClientFactory.Object);
        }


    }
}
