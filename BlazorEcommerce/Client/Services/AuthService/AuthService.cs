namespace BlazorEcommerce.Client.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _client;

        public AuthService(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<ServiceResponse<int>> Register(UserRegister request)
        {
            var result = await _client.PostAsJsonAsync("api/auth/register", request);

            return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        }
    }
}
