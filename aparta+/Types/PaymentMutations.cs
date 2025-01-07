using data_aparta_.DTOs;
using data_aparta_.Repos.Contracts;
using data_aparta_.Repos.Payments;

namespace aparta_.Types
{

    [MutationType]
    public class PaymentMutations
{
        private readonly IPaymentRepository _paymentRepository;

        public PaymentMutations(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<StripeSessionResponse> CreatePaymentSession(decimal price, string description, int quantity, string inmuebleId)
        {
            try
            {
                return await _paymentRepository.CreatePaymentSession(price, description, quantity, inmuebleId);
            }catch(Exception e)
            {
                throw new GraphQLException(e.Message);
            }

        }
}
}
