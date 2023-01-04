

namespace BlazorEcommerce.Client.Services.ProductService
{
    public interface IProductService
    {
        List<Product> Products { get; set; }
        List<Product> AdminProducts { get; set; }

        event Action ProductChanged;

        string Message { get; set; }
        int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public string LastSearchText { get; set; }

        Task GetProducts(string? categoryUrl = null);

        Task GetAdminProducts();

        Task SearchProducts(string searchText, int page);

        Task<List<string>> GetProductSearchSuggestions(string searchText);
  

       Task<ServiceResponse<Product>> GetProduct(int productId);
    }
}
