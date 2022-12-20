using System.Security.Claims;

namespace BlazorEcommerce.Server.Services.OrderService
{
    public class OrderService : IOrderService
    {
        DataContext _context;
        ICartService _cartService;
        IHttpContextAccessor _httpContextAccessor;

        public OrderService(DataContext context, ICartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));


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
                UserId= GetUserId(),
                OrderDate= DateTime.Now,
                TotalPrice= totalPrice,
                OrderItems= orderItems
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(_context.CartItems.Where(ci => ci.UserId == GetUserId()).ToList());

            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data= true };
        }
    }
}
