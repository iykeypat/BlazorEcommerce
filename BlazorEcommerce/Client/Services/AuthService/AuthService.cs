namespace BlazorEcommerce.Client.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _client;


        //ctor
        public AuthService(HttpClient httpClient)
        {
            _client = httpClient;
        }

        //handles change of password on clientside
        public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request)
        {
            var result = await _client.PostAsJsonAsync("api/auth/change-password", request.Password);

            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        //Handles user login on clientside
        public async Task<ServiceResponse<string>> Login(UserLogin request)
        {
            var result = await _client.PostAsJsonAsync("api/auth/login", request);

            return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }


        //Handles user registration on clientside
        public async Task<ServiceResponse<int>> Register(UserRegister request)
        {
            var result = await _client.PostAsJsonAsync("api/auth/register", request);

            return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        }
    }
}
