using data_aparta_.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Contracts
{
    public interface IPaymentRepository
    {
        Task<StripeSessionResponse> CreatePaymentSession(int quantity, string inmuebleId);
        Task<bool> ProcessPayment(string sessionId);
        Task<PaymentStatusResponse> GetPaymentStatus(string inmuebleId);
        Task<ManualPaymentResponse> ProcessManualPayment(ManualPaymentRequest request);
    }
}
