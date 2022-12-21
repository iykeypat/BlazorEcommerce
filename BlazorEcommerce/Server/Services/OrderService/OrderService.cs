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

        public async Task<ServiceResponse<List<OrderOverviewResponse>>> GetOrders()
        {
            var response = new ServiceResponse<List<OrderOverviewResponse>>();
            var orders = await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).Where(o => o.UserId == _authService.GetUserId()).OrderByDescending(o => o.OrderDate).ToListAsync();
            var orderResponse = new List<OrderOverviewResponse>();

            orders.ForEach(o => orderResponse.Add(new OrderOverviewResponse {
                Id = o.Id,
                OrderDate= o.OrderDate,
                TotalPrice=o.TotalPrice,
                Product = o.OrderItems.Count >1 ? $"{o.OrderItems.First().Product.Title} and " + $"{o.OrderItems.Count -1} more..." : o.OrderItems.First().Product.Title,
                ProductImageUrl = o.OrderItems.First().Product.ImageUrl
            }));

            response.Data = orderResponse;
            return response;
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
