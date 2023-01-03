namespace BlazorEcommerce.Server.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        DataContext _context;
        public CategoryService(DataContext context)
        {
            _context = context;

        }

        //Add a new category as suppliesd via the parameter and returns the current list of categories
        public async Task<ServiceResponse<List<Category>>> AddCategory(Category category)
        {
            category.Editing = category.IsNew = false;
            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return await GetAdminCategories();
        }

        //deletes a given category using the provided category id and returns the current list of categories
        public async Task<ServiceResponse<List<Category>>> DeleteCategory(int id)
        {
            Category category = await GetCategoryById(id);

            if (category == null)
            {
                return new ServiceResponse<List<Category>> { Success = false,Message ="Category not found" };
            }
            else
            {
                category.Deleted = true;
                await _context.SaveChangesAsync();
            }

            return await GetAdminCategories();
        }


        //helper method to get a particular category given the id
        private async Task<Category> GetCategoryById(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
   
        }


        //Returns a list carrying all categories
        public async Task<ServiceResponse<List<Category>>> GetAdminCategories()
        {
            var categories = await _context.Categories.Where(c => !c.Deleted).ToListAsync();

            return new ServiceResponse<List<Category>>()
            {
                Data = categories
            };
        }

        //returns a list carrying all categories that are visible
        public async Task<ServiceResponse<List<Category>>> GetCategories()
        {
            var categories = await _context.Categories.Where(c => !c.Deleted && c.Visible).ToListAsync();

            return new ServiceResponse<List<Category>>()
            {
                Data = categories
            };
        }


        //method to update the content of a given category
        public async Task<ServiceResponse<List<Category>>> UpdateCategory(Category category)
        {
            var dbCategory = await GetCategoryById(category.Id);

            if (dbCategory == null)
            {
                return new ServiceResponse<List<Category>> { Success = false, Message = "Category not found" };
            }

            dbCategory.Name = category.Name;
            dbCategory.Url = category.Url;
            dbCategory.Visible = category.Visible;

            await _context.SaveChangesAsync();

            return await GetAdminCategories();
        }
    }
}
