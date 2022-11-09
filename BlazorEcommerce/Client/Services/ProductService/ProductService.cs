

namespace BlazorEcommerce.Client.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient httpClient;

        public List<Product> Products { get; set; } = new List<Product>();

        public ServiceResponse<Product> serviceResponse { get; set; } = new ServiceResponse<Product>();



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



        public async Task<ServiceResponse<Product>> GetProduct(int productId)
        {
            
            var result = await httpClient.GetFromJsonAsync<ServiceResponse<Product>>($"/api/Products/product/{productId}");

            if (result != null && result.Data != null)
            {
                               return result;
            }
            else
            {
                result = new ServiceResponse<Product>
                {
                    Success = false,
                    Message = "Sorry, but this product does not exist"
                };

                return result;
            }

           
                
   
        }

        public Product GetProductById(int id)
        {
            return  httpClient.GetFromJsonAsync<Product>("api/product/GetProductById/" + id).Result;
        }
    }
}
