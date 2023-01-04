namespace BlazorEcommerce.Client.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        public List<ProductType> ProductTypes { get ; set ; } = new List<ProductType>();
        public event Action OnChange;
        HttpClient _httpClient;

        //ctor
        public ProductTypeService(HttpClient httpClient) {

            _httpClient = httpClient;
        }


        //returns product types for admin view
        public async Task GetProductTypes()
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<ProductType>>>("api/producttype");

            ProductTypes = result.Data;
        }
    }
}
