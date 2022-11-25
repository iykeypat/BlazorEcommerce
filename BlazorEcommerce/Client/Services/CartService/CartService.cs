using Blazored.LocalStorage;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;

        //ctor
        public CartService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        //monitors change event
        public event Action OnChange;

        //adds an item to the cart
        public async Task AddToCart(CartItem cartItem)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                cart = new List<CartItem>();
            }
            cart.Add(cartItem);

            await _localStorage.SetItemAsync("cart", cart);
        }

        //gets all items added to the cart
        public async Task<List<CartItem>> GetCartItems()
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            return cart;
        }
    }
}
