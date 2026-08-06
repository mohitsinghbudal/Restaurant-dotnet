using HotelManagementSystem.Models.Cart;

namespace HotelManagementSystem.Interfaces
{
    public interface ICartDLL
    {
        Task<IEnumerable<Cart?>> GetMyCartAsync(int userID);
        Task<Cart?> GetMyCartByItemAsync(int CartId);
        Task<Cart?> GetCartAsync(int userID, int MenuId);
        Task<bool> CreateCartAsync(Cart cart);
        Task<bool> UpdateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(int cartId);

    }
    public interface ICartService
    {
        Task<IEnumerable<Cart?>> GetMyCartAsync(int userID);
        Task<Cart?> GetMyCartByItemAsync( int userID);
        Task<bool> CreateCartAsync(Cart cart, int userID);
        Task<bool> UpdateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(int cartId);
    }
}
