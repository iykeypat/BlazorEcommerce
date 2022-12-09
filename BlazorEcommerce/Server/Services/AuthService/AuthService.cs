using System.Security.Cryptography;

namespace BlazorEcommerce.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        DataContext _context;
        public AuthService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<string>> Login(string email, string password)
        {
            var response =  new ServiceResponse<string>()
            {
                Data = "Token"
            };

            return response;
        }

        //for user registration
        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            if (await UserExists(user.Email))
            {
                return new ServiceResponse<int> { Success = false, Message = "User already exists." };
            }

            CreatePasswordHash(password,out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new ServiceResponse<int>()
            {
                Data = user.Id,
                Message = "Registration successful!"
            };
        }


       //Checks if the user already exist
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

        //Create password hash
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
