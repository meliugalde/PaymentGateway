using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.Models.Responses
{
    public class PostPaymentBankResponse
    {
        public bool Authorized { get; set; }
        public Guid Authorization_code { get; set; }
    }
}
