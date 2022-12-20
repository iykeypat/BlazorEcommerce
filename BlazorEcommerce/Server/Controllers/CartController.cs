using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        //ctor
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        //gets cartItems from the DB
        [HttpPost("products")]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetCartProducts(List<CartItem> cartItems)
        {
            var result = await _cartService.GetCartProducts(cartItems);

            return Ok(result);
        }
        
        //stores cartItems in the DB
        [HttpPost("store-products")]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> StoreCartItems(List<CartItem> cartItems)
        {
            var result = await _cartService.StoreCartItems(cartItems);

            return Ok(result);
        }

        //Gets the total number of cartitems from the DB
        [HttpGet("count")]
        public async Task<ActionResult<ServiceResponse<int>>> GetCartItemsCount()
        {
            return await _cartService.GetCartItemsCount();
        }

        //Gets cart item products from the DB
        [HttpGet("products")]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetDbCartProducts()
        {
            var result = await _cartService.GetDbCartProducts();

            return Ok(result);
        }

        //Adds product to the cartItems the DB
        [HttpPost("addproducts")]
        public async Task<ActionResult<ServiceResponse<bool>>> AddToCart(CartItem cartItem)
            {
            var result = await _cartService.AddToCart(cartItem);

            return Ok(result);
        }


        //Adds product to the cartItems the DB
        [HttpPut("update-quantity")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateQuantity(CartItem cartItem)
        {
            var result = await _cartService.UpdateQuantity(cartItem);

            return Ok(result);
        }        
        
        
        //removes product from the cartItems the DB
        [HttpDelete("delete/{productId}/{productTypeId}")]
        public async Task<ActionResult<ServiceResponse<bool>>> RemoveItemsFromCart(int productId, int productTypeId)
        {
            var result = await _cartService.RemoveItemFromCart(productId,productTypeId);

            return Ok(result);
        }
    }
}
