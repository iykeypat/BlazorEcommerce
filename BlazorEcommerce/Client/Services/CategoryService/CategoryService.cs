namespace BlazorEcommerce.Client.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        HttpClient httpClient { get; set; }

        public CategoryService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task GetCategories()
        {
            var response = await httpClient.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/category");

            if(response != null && response.Data !=null)
            {
                Categories = response.Data;
            }
 
        }
    }
}
