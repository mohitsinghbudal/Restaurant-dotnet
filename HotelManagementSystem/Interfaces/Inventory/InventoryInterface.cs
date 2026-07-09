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
        Task<InventoryItem> AddInventoryItem(InventoryItem inventoryItem);
        Task<int> UpdateInventoryItem(InventoryItem inventoryItem);
        Task<bool> DeductInventoryForOrderAsync(int menuId, int orderedQuantity);
    }
    public interface IInventoryDLL
    {
        Task<IEnumerable<InventoryItem>> GetInventoryItemAsync();
        Task<InventoryItem> AddInventoryItem(InventoryItem inventoryItem);
        Task<int> UpdateInventoryItem(InventoryItem inventoryItem);
        Task<InventoryItem?> GetInventoryItemById(int id);
        Task<bool> DeductRawStockAsync(IEnumerable<InventoryDeductionModel> itemsToDeduct);
    }
}
