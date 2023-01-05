using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;
        IHttpContextAccessor _httpContextAccessor;

        //ctor
        public ProductService(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //method for admin to create a new product. it returns the newly created product inside the DTO
        public async Task<ServiceResponse<Product>> CreateProduct(Product product)
        {
            foreach (var variant in product.Variants)
            {
                variant.ProductType = null;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Product> { Data= product };
        }

        //this method allows admin to soft-delete a product
        public async Task<ServiceResponse<bool>> DeleteProduct(int productId)
        {
            var dbProduct = await _context.Products.FindAsync(productId);

            if (dbProduct == null) {
                return new ServiceResponse<bool> { Success= false, Message = "Product not found. ", Data = false };
            }

            dbProduct.Deleted= true;

            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }

        //returns all products for admin view whether visible or not
        public async Task<ServiceResponse<List<Product>>> GetAdminProducts()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products.Where(p => !p.Deleted).Include(x => x.Variants.Where(v => !v.Deleted))
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
                Data = await _context.Products.Where(p=> p.Featured && p.Visible && !p.Deleted).Include(p => p.Variants.Where(p => p.Visible && !p.Deleted)).ToListAsync()
            };

            return response;
        }

        //returns a single product given the id
        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();

            Product product = null;

            if (_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                product = await _context.Products.Include(x => x.Variants.Where(v => !v.Deleted))
                .ThenInclude(y => y.ProductType).FirstOrDefaultAsync(z => z.Id == productId && !z.Deleted);
            }
            else
            {
                product = await _context.Products.Include(x => x.Variants.Where(v => v.Visible && !v.Deleted))
                .ThenInclude(y => y.ProductType).FirstOrDefaultAsync(z => z.Id == productId && !z.Deleted && z.Visible);
            }

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
            //return _context.Products.First(x=> x.Id == productId);
            return _context.Products.Find(productId);
        }


        //returns a list of the products
        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products.Where(p => p.Visible && !p.Deleted).Include(x => x.Variants.Where(v => v.Visible && !v.Deleted)).ToListAsync()
            };

            return response;
        }

        //returns a list of products based on category
        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await _context.Products.Where( x => x.Category.Url.ToLower().Equals(categoryUrl.ToLower()) && x.Visible && !x.Deleted)
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

            var products = await _context.Products.Where(p =>
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

        public async Task<ServiceResponse<Product>> UpdateProduct(Product product)
        {
            var dbProduct = await _context.Products.FindAsync(product.Id);

            if (dbProduct == null)
            {
                return new ServiceResponse<Product> { Success = false, Message = "Product not found. " };
            }

            dbProduct.Title = product.Title;
            dbProduct.Description = product.Description;
            dbProduct.ImageUrl= product.ImageUrl;
            dbProduct.CategoryId= product.CategoryId;
            dbProduct.Visible= product.Visible;
            dbProduct.Featured = product.Featured;

            foreach (var variant in product.Variants)
            {
                var dbVariant = await _context.ProductVariants.SingleOrDefaultAsync(v => v.ProductId == variant.ProductId && v.ProductTypeId == variant.ProductTypeId);

                if (dbVariant == null)
                {
                    variant.ProductType = null;
                    _context.ProductVariants.Add(variant);
                }
                else
                {
                    dbVariant.ProductTypeId= variant.ProductTypeId;
                    dbVariant.Price= variant.Price;
                    dbVariant.OriginalPrice= variant.OriginalPrice;
                    dbVariant.Visible = variant.Visible;
                    dbVariant.Deleted= variant.Deleted;
                }
            }

            await _context.SaveChangesAsync();
            return new ServiceResponse<Product> { Data = product };
        }

        private async Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            return await _context.Products.Where(p =>
                            p.Title.ToLower().Contains(searchText.ToLower()) || p.Description.ToLower().Contains(searchText.ToLower()) && p.Visible && !p.Deleted)
                            .Include(p => p.Variants.Where(p => p.Visible && !p.Deleted)).ToListAsync();
        }
    }
}
