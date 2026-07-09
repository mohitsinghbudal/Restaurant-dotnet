using HotelManagementSystem.Models.MenuItems;

namespace HotelManagementSystem.Interfaces.MenuInterface
{
    public interface IMenuServices
    {
        Task<int> CreateMenuItemAsync(CreateMenu menu);
        Task<Menu?> GetMenuItemByIdAsync(int menuId);
        Task<IEnumerable<ShowMenu>> GetAllMenuItemsAsync();
        Task<int> UpdateMenuAsync(UpdateMenu menu);
    }

    public interface IMenuDLL
    {
        Task<int> CreateMenuItemAsync(CreateMenu menu); // Returns the new identity ID
        Task<Menu?> GetMenuItemByIdAsync(int menuId);
        //Task<IEnumerable<Menu>> GetAllMenuItemsAsync(); // Added missing method
        Task<int> UpdateMenuAsync(UpdateMenu menu); // Standardized signature
        Task<IEnumerable<ShowMenu>> GetAllMenuItemsAsync();
        Task<decimal> GetPriceById(int Id);
    }
}