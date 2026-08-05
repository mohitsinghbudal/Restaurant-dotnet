using DocumentFormat.OpenXml.Spreadsheet;
using HotelManagementSystem.Interfaces;
using HotelManagementSystem.Models.Cart;

namespace HotelManagementSystem.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ICartDLL _cartDLL;
        public CartService(ICartDLL cartDLL)
        {
            _cartDLL = cartDLL;
        }
        public async Task<IEnumerable<Cart?>> GetMyCartAsync(int userID)
        {
            return await _cartDLL.GetMyCartAsync(userID);
        }
        public async Task<Cart?> GetMyCartByItemAsync( int userID)
        {
            return await _cartDLL.GetMyCartByItemAsync(userID);
        }
        public async Task<bool> CreateCartAsync(Cart cart)
        {
            var item = await _cartDLL.GetMyCartByItemAsync(cart.UserId );
            if (item!=null)
            {
                item.Quantity +=cart.Quantity;
                item.UpdatedAt = DateTime.UtcNow;
                return await _cartDLL.UpdateCartAsync(item);
            }

            var newitem = new Cart
            {
                UserId = cart.UserId,
                MenuId = cart.MenuId,
                Quantity = cart.Quantity,
                CreatedAt = DateTime.UtcNow,
                
            };

            
            return await _cartDLL.CreateCartAsync(newitem);
        }
        public async Task<bool> UpdateCartAsync(Cart cart)
        {
            var item = await _cartDLL.GetMyCartByItemAsync(cart.UserId);
            if (item != null)
            {
                throw new Exception("Item doesnot exists");
                return false;
            }
                return await _cartDLL.UpdateCartAsync(cart);
        }
        public async Task<bool> DeleteCartAsync(int cartId,int userId)
        {
            var item = await _cartDLL.GetMyCartByItemAsync(userId);
            if (item == null)
            {
                throw new Exception("Item doesnot exists");
                return false;
            }
            return await _cartDLL.DeleteCartAsync(cartId, userId);
        }
    }
}
