namespace BlazorEcommerce.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        DataContext _context;
        public AuthService(DataContext context)
        {
            _context = context;
        }

        //for user registration
        public Task<ServiceResponse<int>> Register(User user, string password)
        {
            throw new NotImplementedException();
        }


       //Checks if the uder already exist
        public async Task<bool> UserExists(string email)
        {
            if (await _context.Users.AnyAsync( user => user.Email.ToLower().Equals(email.ToLower()) ))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
