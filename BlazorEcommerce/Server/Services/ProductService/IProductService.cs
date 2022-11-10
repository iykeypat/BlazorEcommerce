namespace BlazorEcommerce.Server.Services.ProductService
{
    public interface IProductService
    {
        Task<ServiceResponse<List<Product>>> GetProductsAsync();

        Task<ServiceResponse<Product>> GetProductAsync(int productId);

        Product GetProductById(int productId);

        Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl);
    }
}
