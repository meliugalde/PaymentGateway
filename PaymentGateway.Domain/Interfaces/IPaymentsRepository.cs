using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PaymentGateway.Domain.Models.Responses;

namespace PaymentGateway.Domain.Interfaces
{
    public interface IPaymentsRepository
    {
        PaymentResponse Get(Guid id);
        void Add(PaymentResponse payment);
    }
}
