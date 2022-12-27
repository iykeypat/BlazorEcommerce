namespace BlazorEcommerce.Server.Services.AddressService
{
    public class AddressService : IAddressService
    {
        DataContext _context;
        IAuthService _authService;

        //ctor
        public AddressService(DataContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        //adds a new address for a user or updates the user's existing address
        public async Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address)
        {
            var response = new ServiceResponse<Address>();
            var dbAddress = (await GetAddress()).Data;

            if (dbAddress ==null)
            {
                address.UserId = _authService.GetUserId();
                _context.Addresses.Add(address);
                response.Data= address;
            }
            else
            {
                dbAddress.FirstName = address.FirstName;
                dbAddress.LastName = address.LastName;
                dbAddress.State= address.State;
                dbAddress.City = address.City;
                dbAddress.Country= address.Country;
                dbAddress.Zip = address.Zip;
                dbAddress.Street= address.Street;

            }

            await _context.SaveChangesAsync();

            return response;
        }


        //returns a user's address
        public async Task<ServiceResponse<Address>> GetAddress()
        {
            int userId = _authService.GetUserId();

            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);

            return new ServiceResponse<Address> { Data = address };
        }
    }
}
