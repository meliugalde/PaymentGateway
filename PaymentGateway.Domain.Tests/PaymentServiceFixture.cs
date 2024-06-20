using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Domain.Services;

namespace PaymentGateway.Api.Tests
{
   
    public class PaymentServiceFixture
    {

        public IPaymentsService _paymentsService;
        public Mock<IPaymentsRepository> _mockPaymentsRepository;
        public Mock<IBankService> _mockBankService;

        public PaymentServiceFixture()
        {
            _mockPaymentsRepository = new Mock<IPaymentsRepository>();
            _mockBankService = new Mock<IBankService>();
            _paymentsService = new PaymentsService(_mockPaymentsRepository.Object, _mockBankService.Object);
        }


    }
}
