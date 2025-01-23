using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.DTOs
{
    public class StripeSessionResponse
    {
        public string SessionId { get; set; }
        public string Url { get; set; }
    }
    public class PaymentStatusResponse
    {
        public bool IsCurrentMonthPaid { get; set; }
        public bool HasPendingCurrentMonth { get; set; }
        public bool HasDebt { get; set; }
        public int DebtMonths { get; set; }
        public decimal DebtAmount { get; set; }
        public bool WillHaveMora { get; set; }
        public decimal MoraAmount { get; set; }
        public string InmuebleId { get; set; }
    }
    public class ManualPaymentResponse
    {
        public bool Success { get; set; }
        public decimal DeudaTotal { get; set; }
        public string Message { get; set; }
        public string InmuebleId { get; set; }
    }

    public class ManualPaymentRequest
    {
        public string InmuebleId { get; set; }
    }


}
