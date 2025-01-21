using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using data_aparta_.Repos.Payments;

namespace aparta_.Types
{

    [MutationType]
    public class PaymentMutations
{
        private readonly IPaymentRepository _paymentRepository;
        private readonly InvoiceRepository invoiceRepo;

        public PaymentMutations(IPaymentRepository paymentRepository, InvoiceRepository invoiceRepo)
        {
            _paymentRepository = paymentRepository;
            this.invoiceRepo = invoiceRepo;
        }

        public async Task<StripeSessionResponse> CreatePaymentSession(int quantity, string inmuebleId)
        {
            try
            {
                return await _paymentRepository.CreatePaymentSession(quantity, inmuebleId);
            }catch(Exception e)
            {
                throw new GraphQLException(e.Message);
            }

        }

        public async Task<bool> ConfirmPayment(string sessionId)
        {
            try
            {
                return await _paymentRepository.ProcessPayment(sessionId);
            }
            catch (Exception e)
            {
                throw new GraphQLException(e.Message);
            }
        }

        public async Task<List<Factura>> GenerateMonthlyInvoices()
        {
            try
            {
                return await invoiceRepo.GenerateMonthlyInvoices();
            }
            catch (Exception e)
            {
                throw new GraphQLException(e.Message);
            }
        }

        public async Task<PaymentStatusResponse> GetPaymentStatus(string inmuebleId)
        {
            try
            {
                return await _paymentRepository.GetPaymentStatus(inmuebleId);
            }
            catch (Exception e)
            {
                throw new GraphQLException(e.Message);
            }
        }
        public async Task<ManualPaymentResponse> ManualPayment(ManualPaymentRequest input)
        {
            try
            {
                return await _paymentRepository.ProcessManualPayment(input);
            }
            catch (Exception e)
            {
                throw new GraphQLException(e.Message);
            }
        }
}
}
