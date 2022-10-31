

namespace BlazorEcommerce.Client.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient httpClient;

        public List<Product> Products { get; set; } = new List<Product>();

        public ProductService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task GetProducts()
        {
            var result = await httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/products");
            if (result != null && result.Data != null)
            {
                Products = result.Data;
            }

        }
    }
}
