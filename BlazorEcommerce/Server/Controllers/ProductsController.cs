using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
      
        //ctor
        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        //This endpoint returns a list of all visible products from DB
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts()
        {
            var result = await productService.GetProductsAsync();

            
            return Ok(result); 
        }

        //Returns a single product from DB given the id
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

        //Returns alist of products that belong to a given category
        [HttpGet("category/{categoryUrl}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductsByCategory(string categoryUrl)
        {
            var result = await productService.GetProductsByCategory(categoryUrl);


            return Ok(result);
        }

        //Returns all products that matches a given search string and paginates the search result
        [HttpGet("search/{searchText}/{page}")]
        public async Task<ActionResult<ServiceResponse<ProductSearchResult>>> SearchProducts(string searchText,int page=1) 
        {
            var result = await productService.SearchProducts(searchText,page);


            return Ok(result);
        }

        //Returns as suggestions, all products whose title or description contains the search string
        [HttpGet("searchsuggestions/{searchText}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductSearchSuggestions(string searchText)
        {
            var result = await productService.GetProductSearchSuggestions(searchText);


            return Ok(result);
        }

        //Returns a list of products with featured flag set to true
        [HttpGet("featured")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetFeaturedProducts()
        {
            var result = await productService.GetFeaturedProducts();


            return Ok(result);
        }
        
        //returns all products whether visible or not for admin view
        [HttpGet("admin"),Authorize(Roles ="Admin")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetAdminProducts()
        {
            var result = await productService.GetAdminProducts();


            return Ok(result);
        }

        //Creates a new product and returns the newly created product
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<Product>>> CreateProduct(Product product)
        {
            var result = await productService.CreateProduct(product);


            return Ok(result);
        }

        //Updates an existing product and returns it
        [HttpPut, Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<Product>>> UpdateProduct(Product product)
        {
            var result = await productService.UpdateProduct(product);


            return Ok(result);
        }

        //Soft Deletes a product given the product id
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteProduct(int id)
        {
            var result = await productService.DeleteProduct(id); 


            return Ok(result);
        }



    }
}
