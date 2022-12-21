using System.Security.Claims;

namespace BlazorEcommerce.Server.Services.OrderService
{
    public class OrderService : IOrderService
    {
        DataContext _context;
        ICartService _cartService;
        IAuthService _authService;

        public OrderService(DataContext context, ICartService cartService, IAuthService authService)
        {
            _context = context;
            _cartService = cartService;
            _authService = authService;
        }

        //For placing order in view of cashout
        public async Task<ServiceResponse<bool>> PlaceOrder()
        {
            var products = (await _cartService.GetDbCartProducts()).Data;
            decimal totalPrice = 0;
            products.ForEach(p => totalPrice += p.Price * p.Quantity);

            var orderItems = new List<OrderItem>();

            products.ForEach(p => orderItems.Add(
                new OrderItem
                {
                    ProductId = p.ProductId,
                    ProductTypeId = p.ProductTypeId,
                    Quantity = p.Quantity,
                    TotalPrice = p.Price * p.Quantity
                }
                ));

            var order = new Order
            {
                UserId= _authService.GetUserId(),
                OrderDate= DateTime.Now,
                TotalPrice= totalPrice,
                OrderItems= orderItems
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(_context.CartItems.Where(ci => ci.UserId == _authService.GetUserId()));

            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data= true };
        }
    }
}
