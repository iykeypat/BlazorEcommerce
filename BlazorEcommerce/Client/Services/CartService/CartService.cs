using BlazorEcommerce.Shared;
using Blazored.LocalStorage;


namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        //ctor
        public CartService(ILocalStorageService localStorage, HttpClient httpclient, IAuthService authService)
        {
            _localStorage = localStorage;
            _httpClient = httpclient;
            _authService = authService;
        }

        //monitors change event
        public event Action OnChange;

        //adds an item to the cart
        public async Task AddToCart(CartItem cartItem)
        {
            if (await _authService.IsUserAuthenticated())
            {
                await _httpClient.PostAsJsonAsync("api/cart/addproducts", cartItem);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    cart = new List<CartItem>();
                }
                var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);

                if (sameItem == null)
                {
                    cart.Add(cartItem);
                }
                else
                {
                    sameItem.Quantity += cartItem.Quantity;
                }


                await _localStorage.SetItemAsync("cart", cart);

            }

            await GetCartItemsCount();
        }


        //Gets list of products in the cart
        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            if (await _authService.IsUserAuthenticated())
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>>("api/cart/products");

                return response.Data;
            }
            else
            {

                var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");

                if (cartItems ==null)
                {
                    return new List<CartProductResponse>();
                }


                var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItems);
                var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

                return cartProducts.Data;
            }

        }

        //remove item from cart
        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            if (await _authService.IsUserAuthenticated())
            {
                await _httpClient.DeleteAsync($"api/cart/delete/{productId}/{productTypeId}");
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

                if (cart == null)
                {
                    return;
                }
                var cartItem = cart.Find(item => item.ProductId == productId && item.ProductTypeId == productTypeId);

                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    await _localStorage.SetItemAsync("cart", cart);
                }
            }

        }

        //this stores the cartitems in the Db
        public async Task StoreCartItems(bool emptyLocalCart = false)
        {
            var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (localCart == null)
            {
                return;
            }

            await _httpClient.PostAsJsonAsync("api/cart/store-products",localCart);

            if (emptyLocalCart)
            {
                await _localStorage.RemoveItemAsync("cart");
            }
        }


        //This updates the total number of items in the cart
        public async Task UpdateQuantity(CartProductResponse product)
        {
            if (await _authService.IsUserAuthenticated())
            {
                var request = new CartItem
                {
                    ProductId= product.ProductId,
                    Quantity = product.Quantity,
                    ProductTypeId = product.ProductTypeId
                };

                await _httpClient.PutAsJsonAsync("api/cart/update-quantity", request);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    return;
                }
                var cartItem = cart.Find(item => item.ProductId == product.ProductId && item.ProductTypeId == product.ProductTypeId);

                if (cartItem != null)
                {
                    cartItem.Quantity = product.Quantity;
                    await _localStorage.SetItemAsync("cart", cart);
                }

            }
        }

             

        //This returns the number of items in the cart 
        public async Task GetCartItemsCount()
        {
            if (await _authService.IsUserAuthenticated())
            {
                var result = await _httpClient.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;

                await _localStorage.SetItemAsync<int>("cartItemsCount",count);
                
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                await _localStorage.SetItemAsync<int>("cartItemsCount", cart != null ? cart.Count: 0);
            }

            OnChange.Invoke();
        }
    }
}
