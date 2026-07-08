using HotelManagementSystem.Interfaces.Inventory;
using HotelManagementSystem.Models.Inventory;

namespace HotelManagementSystem.Services.Inventory
{
    public class InventoryServices : IInventoryService
    {
        private readonly IInventoryDLL _inventoryDLL;

        public InventoryServices(IInventoryDLL inventoryDLL)
        {
            _inventoryDLL = inventoryDLL;
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryItemsAsync()
        {
            return await _inventoryDLL.GetInventoryItemAsync();
        }

        public async Task<InventoryItem?> GetInventoryItemById(int id)
        {
            return await _inventoryDLL.GetInventoryItemById(id);
        }

        public async Task<InventoryItem> AddInventoryItem(InventoryItem inventoryItem)
        {
            return await _inventoryDLL.AddInventoryItem(inventoryItem);
        }

        public async Task<int> UpdateInventoryItem(InventoryItem incomingItem)
        {
            // 1. Fetch the current, real state of the item from the DB
            var existingItem = await _inventoryDLL.GetInventoryItemById(incomingItem.InventoryItemId);

            if (existingItem == null)
            {
                throw new KeyNotFoundException($"Inventory item with ID {incomingItem.InventoryItemId} not found.");
            }

            // 2. Apply business logic / Patch only what changed
            // If incoming CostPrice is greater than 0, update it. Otherwise, keep the old one.
            if (incomingItem.CostPrice > 0 && incomingItem.CostPrice != existingItem.CostPrice)
            {
                existingItem.CostPrice = incomingItem.CostPrice;
            }

            // Do the same for other fields if they are sent over
            if (!string.IsNullOrWhiteSpace(incomingItem.ItemName))
            {
                existingItem.ItemName = incomingItem.ItemName;
            }

            if (incomingItem.UnitId > 0)
            {
                existingItem.UnitId = incomingItem.UnitId;
            }

            // Track who modified it and when
            existingItem.UpdatedBy = incomingItem.UpdatedBy;
            existingItem.UpdatedOn = DateTime.UtcNow;

            // 3. Pass the fully merged record back to the DLL to save safely
            return await _inventoryDLL.UpdateInventoryItem(existingItem);
        }
    }
}