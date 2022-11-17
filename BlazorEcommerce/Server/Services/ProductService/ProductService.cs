﻿namespace BlazorEcommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext context;

        public ProductService(DataContext context)
        {
            this.context = context;
        }

        //returns a single product given the id
        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();

            var product = await context.Products.Include(x => x.Variants).ThenInclude(y => y.ProductType).FirstOrDefaultAsync( z => z.Id == productId);

            if (product == null)
            {
                response.Success = false;
                response.Message = "Sorry, but this product does not exist";
            }
            else
            {
                response.Data = product;
            }

            return response;
        }

        //returns a single product given the id
        public Product GetProductById(int productId)
        {
            //return context.Products.First(x=> x.Id == productId);
            return context.Products.Find(productId);
        }


        //returns a list of the products
        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await context.Products.Include(x => x.Variants).ToListAsync()
            };

            return response;
        }

        //returns a list of products based on category
        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await context.Products.Where( x => x.Category.Url.ToLower().Equals(categoryUrl.ToLower()))
                .Include(p => p.Variants).ToListAsync()
            };

            return response;
        }

        //returns a list of products based on given search strings
        public async Task<ServiceResponse<List<Product>>> SearchProducts(string searchText)
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await context.Products.Where(p => 
                p.Title.ToLower().Contains(searchText.ToLower()) || p.Description.ToLower().Contains(searchText.ToLower()))
                .Include(p => p.Variants).ToListAsync()
            };

            return response;
        }
    }
}
