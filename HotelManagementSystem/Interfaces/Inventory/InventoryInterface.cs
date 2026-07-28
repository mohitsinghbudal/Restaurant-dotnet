using HotelManagementSystem.DLL.InventoryDLL;
using HotelManagementSystem.Models.Inventory;
using HotelManagementSystem.Models.InventoryItem;
using System.Data;

namespace HotelManagementSystem.Interfaces.Inventory
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItem>> GetInventoryItemsAsync();
        Task<InventoryItem?> GetInventoryItemById(int id);
        Task<InventoryItem> AddInventoryItem(InventoryItem inventoryItem, int userId);
        Task<int> UpdateInventoryItem(InventoryItem inventoryItem , int userId);
        Task<bool> DeductInventoryForOrderAsync(int menuId, int orderedQuantity);
        Task<bool> DeleteInventoryItem(int id, int deletedby);
        Task<IEnumerable<InventoryItem>> GetAllInventoryItemsAsync();
    }
    public interface IInventoryDLL
    {
        Task<IEnumerable<InventoryItem>> GetInventoryItemAsync();
        Task<InventoryItem> AddInventoryItem(InventoryItem inventoryItem);
        Task<int> UpdateInventoryItem(InventoryItem inventoryItem);
        Task<InventoryItem?> GetInventoryItemById(int id);
        Task<bool> DeductRawStockAsync(IEnumerable<InventoryDeductionModel> itemsToDeduct);
        Task<bool> DeleteInventoryItem(int id,int deletedBy);

        Task<IEnumerable<InventoryItem>> GetAllInventoryItemsAsync();
    }
}
