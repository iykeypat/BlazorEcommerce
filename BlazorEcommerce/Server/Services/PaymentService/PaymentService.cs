using Stripe;
using Stripe.Checkout;

namespace BlazorEcommerce.Server.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        ICartService _cartService;
        IAuthService _authService;
        IOrderService _orderService;

        public PaymentService(ICartService cartService, IAuthService authService, IOrderService orderService) {

            StripeConfiguration.ApiKey = "sk_test_51MIx0uB77Y3LjlzdfFcfJPHfn9aI676GELezaX9hlm7jMwQY8jBfVnpEClQLKrr6jcf0cZVbX0huYNkXFANCOs8V006XS3Xoh3";
            _cartService = cartService; 
            _authService = authService;
            _orderService = orderService;
        }



        //create and return the checkout session for stripe  
        public async Task<Session> CreateCheckOutSession()
        {
            var products = (await _cartService.GetDbCartProducts()).Data;
            var lineItems = new List<SessionLineItemOptions>();

            products.ForEach(p => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = p.Price * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = p.Title,
                        Images = new List<string> { p.ImageUrl }
                    }
                },
                Quantity= p.Quantity,
            }));

            var options = new SessionCreateOptions
            {
                CustomerEmail = _authService.GetUserEmail(),
                PaymentMethodTypes = new List<string> { "card"},
                LineItems= lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7209/order-success",
                CancelUrl = "https://localhost:7209/cart"
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return session;
        }
    }
}
