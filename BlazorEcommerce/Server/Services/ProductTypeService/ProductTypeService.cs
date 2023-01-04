using BlazorEcommerce.Shared;

namespace BlazorEcommerce.Server.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        DataContext _context;

        //ctor
        public ProductTypeService(DataContext context)
        {
            _context = context;
        }

        //Adds new product type to the DB
        public async Task<ServiceResponse<List<ProductType>>> AddProductType(ProductType productType)
        {
            productType.Editing = productType.IsNew = false;
            _context.ProductTypes.Add(productType);
            await _context.SaveChangesAsync();

            return await GetProductTypes();
        }


        //Gets all product types from the DB
        public async Task<ServiceResponse<List<ProductType>>> GetProductTypes()
        {
            var productTypes = await _context.ProductTypes.ToListAsync();

            return new ServiceResponse<List<ProductType>> { Data= productTypes };
        }


        //Updates an existing product type details
        public async Task<ServiceResponse<List<ProductType>>> UpdateProductType(ProductType productType)
        {
            var dbProductType = await _context.ProductTypes.FindAsync(productType.Id);

            if (dbProductType == null)
            {
                return new ServiceResponse<List<ProductType>>
                {
                    Success = false,
                    Message = "Product type not found."
                };
            }

            dbProductType.Name = productType.Name;

            await _context.SaveChangesAsync();

            return await GetProductTypes();
        }
    }
}
