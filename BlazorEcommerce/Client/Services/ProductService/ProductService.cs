

namespace BlazorEcommerce.Client.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient httpClient;

        public event Action ProductChanged;

        public List<Product> Products { get; set; } = new List<Product>();

        //public ServiceResponse<Product> serviceResponse { get; set; } = new ServiceResponse<Product>();

        public string Message { get; set; } = "Loading products...";


        //ctor
        public ProductService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        //returns a list of product either by category or not
        public async Task GetProducts(string? categoryUrl = null)
        {
            var result = categoryUrl == null ?
                await httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/products/featured") :
                await httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/products/category/{categoryUrl}");


            if (result != null && result.Data != null)
            {
                Products = result.Data;
            }

            ProductChanged.Invoke();

        }


        //returns a particular product given an Id
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

        public async Task SearchProducts(string searchText)
        {
            var result = await httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("/api/products/search/"+searchText);

            if (result != null && result.Data != null)
            {
                Products = result.Data;
            }

            if (Products.Count == 0)
            {
                Message = "No products found";
            }

            ProductChanged?.Invoke();
        }

        public async Task<List<string>> GetProductSearchSuggestions(string searchText)
        {
            var result = await httpClient.GetFromJsonAsync<ServiceResponse<List<string>>>("api/products/searchsuggestions/" + searchText);

            return result.Data;
        }
    }
}
