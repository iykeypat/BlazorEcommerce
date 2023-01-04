namespace BlazorEcommerce.Client.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        public List<ProductType> ProductTypes { get ; set ; } = new List<ProductType>();
        public event Action OnChange;
        HttpClient _httpClient;

        //ctor
        public ProductTypeService(HttpClient httpClient) {

            _httpClient = httpClient;
        }


        //returns product types for admin view
        public async Task GetProductTypes()
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<ProductType>>>("api/producttype");

            ProductTypes = result.Data;
        }

        //This method is called when admin wants to add a new product type
        public async Task AddProductType(ProductType productType)
        {
            var response = await _httpClient.PostAsJsonAsync("api/producttype", productType);

            ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;

            OnChange.Invoke(); 
        }

        //This method is called when admin wants to make changes to existing product type
        public async Task UpdateProductType(ProductType productType)
        {
            var response = await _httpClient.PutAsJsonAsync("api/producttype", productType);

            ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;

            OnChange.Invoke();
        }

        //This method is called when admin wants to create a new product type
        public ProductType CreateNewProductType()
        {
            var newProductType = new ProductType { IsNew = true, Editing = true};

            ProductTypes.Add(newProductType);
            OnChange.Invoke();

            return newProductType;
        }
    }
}
