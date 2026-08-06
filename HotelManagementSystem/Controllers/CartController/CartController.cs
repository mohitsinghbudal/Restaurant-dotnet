using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Interfaces;
using HotelManagementSystem.Models.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace HotelManagementSystem.Controllers.CartController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet]
        public async Task<IActionResult> GetMyCart([FromQuery] int userId)
        {
            try{
                var cart = await _cartService.GetMyCartAsync(userId);
                if (cart == null)
                {
                    return NotFound();
                }
                return Ok(cart);
            }catch(Exception ex){
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] Cart cart)
        {
            try{
                int userId = Convert.ToInt32(cart.UserId);

                
                var result = await _cartService.CreateCartAsync(cart, userId);
            if (!result)
            {
                return BadRequest();
            }
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCart([FromBody] Cart cart)
        {
            try{var result = await _cartService.UpdateCartAsync(cart);
            if (!result)
            {
                return BadRequest();
            }
                return Ok("success");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCart([FromQuery] int cartId)
        {
           try{

                if (!User.IsInRole("1"))
                {
                    throw new Exception("User not allowed");
                }
                var result = await _cartService.DeleteCartAsync(cartId);
            
                if (!result)
            {
                return BadRequest();
            }
                return Ok("success");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
