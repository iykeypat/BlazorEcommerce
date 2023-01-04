using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext context;

        //ctor
        public ProductService(DataContext context)
        {
            this.context = context;
        }

        //returns all products for admin view whether visible or not
        public async Task<ServiceResponse<List<Product>>> GetAdminProducts()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await context.Products.Where(p => !p.Deleted).Include(x => x.Variants.Where(v => !v.Deleted))
                .ThenInclude(v => v.ProductType)
                .ToListAsync()
            };

            return response;
        }

        //returns the featured products
        public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await context.Products.Where(p=> p.Featured && p.Visible && !p.Deleted).Include(p => p.Variants.Where(p => p.Visible && !p.Deleted)).ToListAsync()
            };

            return response;
        }

        //returns a single product given the id
        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();

            var product = await context.Products.Include(x => x.Variants.Where(v => v.Visible && !v.Deleted))
                .ThenInclude(y => y.ProductType).FirstOrDefaultAsync( z => z.Id == productId && !z.Deleted && z.Visible);

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
                Data = await context.Products.Where(p => p.Visible && !p.Deleted).Include(x => x.Variants.Where(v => v.Visible && !v.Deleted)).ToListAsync()
            };

            return response;
        }

        //returns a list of products based on category
        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await context.Products.Where( x => x.Category.Url.ToLower().Equals(categoryUrl.ToLower()) && x.Visible && !x.Deleted)
                .Include(p => p.Variants.Where(v => v.Visible && !v.Deleted)).ToListAsync()
            };

            return response;
        }

        //returns a list of suggested words related to the searchText
        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
        {
            var products = await FindProductsBySearchText(searchText);

            List<string> result = new List<string>();

            foreach (var product in products)
            {
                if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Title);
                }

                if (product.Description != null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();
                    var words = product.Description.Split().Select(x => x.Trim(punctuation)).ToArray();

                    foreach (var word in words)
                    {
                        if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase) && !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    }
                }
            }
            return new ServiceResponse<List<string>> { Data = result };
        }

        //returns a list of products based on given search strings
        public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText, int page)
        {
            var pageResult = 2f;
            var pageCount = Math.Ceiling((await FindProductsBySearchText(searchText)).Count / pageResult);

            var products = await context.Products.Where(p =>
                            p.Title.ToLower().Contains(searchText.ToLower()) || p.Description.ToLower().Contains(searchText.ToLower()) && p.Visible && !p.Deleted)
                            .Include(p => p.Variants).Skip((page - 1) * (int)pageResult).Take((int)pageResult).ToListAsync();

            var response = new ServiceResponse<ProductSearchResult>
            {
                Data = new ProductSearchResult
                {
                    Products = products,
                    CurrentPage = page,
                    Pages = (int)pageCount
                }
            };

            return response;
        }

        private async Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            return await context.Products.Where(p =>
                            p.Title.ToLower().Contains(searchText.ToLower()) || p.Description.ToLower().Contains(searchText.ToLower()) && p.Visible && !p.Deleted)
                            .Include(p => p.Variants.Where(p => p.Visible && !p.Deleted)).ToListAsync();
        }
    }
}
