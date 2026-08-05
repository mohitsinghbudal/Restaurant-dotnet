using HotelManagementSystem.Models.Cart;

namespace HotelManagementSystem.Interfaces
{
    public interface ICartDLL
    {
        Task<IEnumerable<Cart?>> GetMyCartAsync(int userID);
        Task<Cart?> GetMyCartByItemAsync(int userID);
        Task<bool> CreateCartAsync(Cart cart);
        Task<bool> UpdateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(int cartId, int userId);

    }
    public interface ICartService
    {
        Task<IEnumerable<Cart?>> GetMyCartAsync(int userID);
        Task<Cart?> GetMyCartByItemAsync( int userID);
        Task<bool> CreateCartAsync(Cart cart);
        Task<bool> UpdateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(int cartId, int userId);
    }
}
