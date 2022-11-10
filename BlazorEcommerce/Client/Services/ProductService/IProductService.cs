

namespace BlazorEcommerce.Client.Services.ProductService
{
    public interface IProductService
    {
        List<Product> Products { get; set; }

        event Action ProductChanged;
        
        Task GetProducts(string? categoryUrl = null);

        Product GetProductById(int id);   

       Task<ServiceResponse<Product>> GetProduct(int productId);
    }
}
