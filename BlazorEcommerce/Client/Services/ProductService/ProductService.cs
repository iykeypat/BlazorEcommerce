﻿

namespace BlazorEcommerce.Client.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient httpClient;

        public event Action ProductChanged;

        public List<Product> Products { get; set; } = new List<Product>();
        public string Message { get; set; } = "Loading products...";
        public int CurrentPage { get; set; } = 1;
        public int PageCount { get; set; } = 0;
        public string LastSearchText { get; set; } = string.Empty;

        //ctor
        public ProductService(HttpClient httpClient)  
        {
            this.httpClient = httpClient;
        }

        //returns a list of product either by category or not
        public async Task GetProducts(string? categoryUrl = null)
        {
            var result = categoryUrl == null ?
                await httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/products/featured") :
                await httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/products/category/{categoryUrl}");


            if (result != null && result.Data != null)
            {
                Products = result.Data;
            }
            CurrentPage = 1;
            PageCount = 0;

            if(Products.Count == 0)
            {
                Message = "No products found";

            }

            ProductChanged.Invoke();

        }


        //returns a particular product given an Id
        public async Task<ServiceResponse<Product>> GetProduct(int productId)
        {
            
            var result = await httpClient.GetFromJsonAsync<ServiceResponse<Product>>($"/api/Products/product/{productId}");
            if (result != null && result.Data != null)
            {
                               return result;
            }
            else
            {
                result = new ServiceResponse<Product>
                {
                    Success = false,
                    Message = "Sorry, but this product does not exist"
                };

                return result;
            }

           
                
   
        }


        //returns a list of products according to the given search parameters
        public async Task SearchProducts(string searchText, int page)
        {
            var result = await httpClient.GetFromJsonAsync<ServiceResponse<ProductSearchResult>>($"/api/products/search/{searchText}/{page}");

            if (result != null && result.Data != null)
            {
                Products = result.Data.Products;
                CurrentPage = result.Data.CurrentPage;
                PageCount = result.Data.Pages;
            }

            if (Products.Count == 0)
            {
                Message = "No products found";
            }

            ProductChanged?.Invoke();
        }

        public async Task<List<string>> GetProductSearchSuggestions(string searchText)
        {
            var result = await httpClient.GetFromJsonAsync<ServiceResponse<List<string>>>("api/products/searchsuggestions/" + searchText);

            return result.Data;
        }
    }
}
