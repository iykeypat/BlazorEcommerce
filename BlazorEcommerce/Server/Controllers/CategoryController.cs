using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        ICategoryService _categoryService;

        //ctor
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;

        }


        //default get method
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Category>>>> GetCategories()
        {
            var categories = await _categoryService.GetCategories();
            return Ok(categories);
        }

        //method to get categories for admin view
        [HttpGet("admin"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<List<Category>>>> GetAdminCategories()
        {
            var categories = await _categoryService.GetAdminCategories();
            return Ok(categories);
        }

        //method to delete a category given the Id
        [HttpDelete("delete-category"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<List<Category>>>> DeleteCategory(int id)
        {
            var categories = await _categoryService.DeleteCategory(id);
            return Ok(categories);
        }

        //method to add a category given the category object
        [HttpPost("add-category"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<List<Category>>>> AddCategory(Category category)
        {
            var categories = await _categoryService.AddCategory(category);
            return Ok(categories);
        }

        //method to update a given category
        [HttpPut("update-category"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<List<Category>>>> UpdateCategory(Category category)
        {
            var categories = await _categoryService.UpdateCategory(category);
            return Ok(categories);
        }


    }
}
