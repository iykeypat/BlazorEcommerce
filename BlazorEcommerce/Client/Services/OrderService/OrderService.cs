using Microsoft.AspNetCore.Components;

namespace BlazorEcommerce.Client.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly NavigationManager _navigationManager;

        public OrderService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, NavigationManager navigationManager)
        {
            _httpClient= httpClient;
            _authStateProvider = authStateProvider;
            _navigationManager = navigationManager;
        }

        //Returns details of a placed order
        public async Task<OrderDetailsResponse> GetOrderDetails(int orderId)
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<OrderDetailsResponse>>($"api/order/orders/{orderId}");

            return result.Data;
        }

        //returns all orders placed by a user
        public async Task<List<OrderOverviewResponse>> GetOrders()
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<OrderOverviewResponse>>>("api/order/get-orders");

            return result.Data;
        }


        //allows a user to place orders towards checkout
        public async Task PlaceOrder()
        {
            if (await IsUserAuthenticated())
            {
                await _httpClient.PostAsync("api/order/place-order", null);
            }
            else
            {
                _navigationManager.NavigateTo("login");
            }
        }

        private async Task<bool> IsUserAuthenticated()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }
    }
}
