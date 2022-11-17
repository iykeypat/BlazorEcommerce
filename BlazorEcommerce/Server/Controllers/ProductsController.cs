using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
      

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts()
        {
            var result = await productService.GetProductsAsync();

            
            return Ok(result); 
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<ServiceResponse<Product>>> GetProductAsync(int productId)
        {
            var result = await productService.GetProductAsync(productId);


            return Ok(result);
        }

        [HttpGet("GetProductById/{id}")]
        public IActionResult GetProductById(int id)
        {
            var result =  productService.GetProductById(id);


            return Ok(result);
        }

        [HttpGet("category/{categoryUrl}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductsByCategory(string categoryUrl)
        {
            var result = await productService.GetProductsByCategory(categoryUrl);


            return Ok(result);
        }

        [HttpGet("search/{searchText}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> SearchProducts(string searchText)
        {
            var result = await productService.SearchProducts(searchText);


            return Ok(result);
        }

    }
}
