using data_aparta_.DTOs;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Utils
{
    public  class StripeService
    {
        private readonly string _apiKey;

        public StripeService(IOptions<StripeOptions> stripeOptions)
        {
            _apiKey = stripeOptions.Value.ApiKey;
            StripeConfiguration.ApiKey = _apiKey;
        }

        public async Task<StripeSessionResponse> CreatePaymentSession(decimal price, string description, int quantity)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = description,
                    },
                    UnitAmountDecimal = price * 100, // Centavos
                },
                Quantity = quantity,
            },
        },
                Mode = "payment",
                SuccessUrl = "https://example.com/success",
                CancelUrl = "https://example.com/cancel",
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            StripeSessionResponse response = new StripeSessionResponse
            {
                SessionId = session.Id,
                Url = session.Url
            };

            return response;
        }


    }
}
