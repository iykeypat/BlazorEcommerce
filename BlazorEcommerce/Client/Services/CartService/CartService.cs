using Blazored.LocalStorage;


namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

        //ctor
        public CartService(ILocalStorageService localStorage, HttpClient httpclient)
        {
            _localStorage = localStorage;
            _httpClient = httpclient;
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
            OnChange.Invoke();
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

        //Gets list of products in the cart
        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItems);

            var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

            return cartProducts.Data;
        }
    }
}
