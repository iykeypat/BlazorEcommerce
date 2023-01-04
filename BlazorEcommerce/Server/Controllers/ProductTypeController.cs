using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class ProductTypeController : ControllerBase
    {
        IProductTypeService _productTypeService;

        public ProductTypeController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        //Returns all product types for admin's view
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<ProductType>>>> GetProductTypes()
        {
            var response = await _productTypeService.GetProductTypes();

            return Ok(response);
        }

        //This API endpoint is called when admin wants to add a new product type
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<ProductType>>>> AddProductType(ProductType productType)
        {
            var response = await _productTypeService.AddProductType(productType);

            return Ok(response);
        }


        //This API endpoint is called when admin wants to update an existing product type
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<ProductType>>>> UpdateProductType(ProductType productType)
        {
            var response = await _productTypeService.UpdateProductType(productType);

            return Ok(response);
        }
    }
}
