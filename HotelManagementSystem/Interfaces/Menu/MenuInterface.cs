using HotelManagementSystem.Models.MenuItems;

namespace HotelManagementSystem.Interfaces.MenuInterface
{
    public interface IMenuServices
    {
        Task<int> CreateMenuItemAsync(CreateMenu menu);
        Task<Menu?> GetMenuItemByIdAsync(int menuId);
        Task<IEnumerable<ShowMenu>> GetAllMenuItemsAsync();
        Task<int> UpdateMenuAsync(UpdateMenu menu, int userId);
    }

    public interface IMenuDLL
    {
        Task<int> CreateMenuItemAsync(CreateMenu menu); 
        Task<Menu?> GetMenuItemByIdAsync(int menuId);
        
        Task<int> UpdateMenuAsync(UpdateMenu menu); 
        Task<IEnumerable<ShowMenu>> GetAllMenuItemsAsync();
        Task<decimal> GetPriceById(int Id);
    }
}
