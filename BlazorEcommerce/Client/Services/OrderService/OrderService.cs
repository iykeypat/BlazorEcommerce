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

        public async Task<List<OrderOverviewResponse>> GetOrders()
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<OrderOverviewResponse>>>("api/order/get-orders");

            return result.Data;
        }

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
