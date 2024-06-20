using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Domain.Interfaces;

namespace PaymentGateway.Api.Tests
{
    public class PaymentControllerFixture
    {
        public readonly Random _random = new();
        public Mock<IPaymentsService> _mockPaymentsService;
        public Mock<ILogger<PaymentsController>> _mockLogger;

        public PaymentsController _paymentsController;

        public PaymentControllerFixture()
        {
            _mockPaymentsService = new Mock<IPaymentsService>();
            _mockLogger = new Mock<ILogger<PaymentsController>>();
            _paymentsController = new PaymentsController(_mockPaymentsService.Object, _mockLogger.Object);
        }


    }
}
