using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        HttpClient httpClient { get; set; }
        public List<Category> AdminCategories { get; set; } = new List<Category>();

        //ctor
        public CategoryService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public event Action OnChange;

        //Get categories for client
        public async Task GetCategories()
        {
            var response = await httpClient.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/category");

            if(response != null && response.Data !=null)
            {
                Categories = response.Data;
            }
 
        }

        //Get categories for admin
        public async Task GetAdminCategories()
        {
            var response = await httpClient.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/category/admin");

            if (response != null && response.Data != null)
            {
                AdminCategories = response.Data;
            }
        }

        //adds a category to the db
        public async Task AddCategory(Category category)
        {
            var response = await httpClient.PostAsJsonAsync("api/category/add-category", category);
            AdminCategories = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>()).Data;

            await GetCategories();
            OnChange.Invoke();
        }


        //updates the properties of an existing category
        public async Task UpdateCategory(Category category)
        {
            var response = await httpClient.PutAsJsonAsync("api/category/update-category", category);

            AdminCategories = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>()).Data;

            await GetCategories();
            OnChange.Invoke();
        }


        //Deletes a category from the db
        public async Task DeleteCategory(int categoryId)
        {
            var response = await httpClient.DeleteAsync("api/category/add-category/delete-category/"+ categoryId);
            AdminCategories = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>()).Data;

            await GetCategories();
            OnChange.Invoke();
        }

        public Category CreateNewCategory()
        {
            var newCategory = new Category { IsNew = true, Editing = true};
            AdminCategories.Add(newCategory);
            OnChange.Invoke();
            return newCategory;
        }
    }
}
