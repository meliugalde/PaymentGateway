using System;

using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Domain.Models.Responses;

namespace PaymentGateway.Infrastructure.Repositories;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly List<PaymentResponse> Payments;

    public PaymentsRepository()
    {
        Payments = new List<PaymentResponse>();
    }
    public void Add(PaymentResponse payment)
    {
        try
        {
            Payments.Add(payment);
        }
        catch (Exception ex)
        {
            //Logging PaymentResponse Add Exception
            throw new Exception("Error adding payment");
        }
       
    }

    public PaymentResponse Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}