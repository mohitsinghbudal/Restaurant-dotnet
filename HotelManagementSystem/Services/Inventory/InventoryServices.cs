using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using HotelManagementSystem.Interfaces.Inventory;
using HotelManagementSystem.Interfaces.RecipeInterface;
using HotelManagementSystem.Models.Inventory;
using HotelManagementSystem.Models.InventoryItem;
using System.Data;

namespace HotelManagementSystem.Services.Inventory
{
    public class InventoryServices : IInventoryService
    {
        private readonly IInventoryDLL _inventoryDLL;
        private readonly IRecipeDLL _recipeDLL;

        public InventoryServices(IInventoryDLL inventoryDLL, IRecipeDLL recipeDLL)
        {
            _inventoryDLL = inventoryDLL;
            _recipeDLL = recipeDLL;
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryItemsAsync()
        {
            return await _inventoryDLL.GetInventoryItemAsync();
        }

        public async Task<InventoryItem?> GetInventoryItemById(int id)
        {
            return await _inventoryDLL.GetInventoryItemById(id);
        }

        public async Task<InventoryItem> AddInventoryItem(InventoryItem inventoryItem ,int userId)
        {
           try{ 
                inventoryItem.CreatedBy = userId;
                inventoryItem.CreatedOn = DateTime.UtcNow;
                inventoryItem.IsActive = true;

                return await _inventoryDLL.AddInventoryItem(inventoryItem);
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateInventoryItem(InventoryItem incomingItem , int userId)
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

            existingItem.CurrentQuantity = incomingItem.CurrentQuantity;
            existingItem.MinimumQuantity = incomingItem.MinimumQuantity;
            existingItem.IsActive = incomingItem.IsActive;


            // Track who modified it and when
            existingItem.UpdatedBy = incomingItem.UpdatedBy;
            existingItem.UpdatedOn = DateTime.UtcNow;

            // 3. Pass the fully merged record back to the DLL to save safely
            return await _inventoryDLL.UpdateInventoryItem(existingItem);
        }

        public async Task<bool> DeductInventoryForOrderAsync(int menuId, int orderedQuantity)
        {
            Console.WriteLine("Reached the inventory service transaction loop.");

            // 1. Business Logic: Extract recipe requirements using the shared connection context
            var recipes = (await _recipeDLL.GetRecipeByMenuIdAsync(menuId)).ToList();

            // 2. Business Logic: Direct retail items (like Coke cans) don't have ingredients to deduct
            if (!recipes.Any())
                return true;

            // 3. Business Logic: Loop and process calculations into direct payloads
            var deductionPayloads = new List<InventoryDeductionModel>();
            foreach (var recipe in recipes)
            {
                deductionPayloads.Add(new InventoryDeductionModel
                {
                    InventoryItemId = recipe.InventoryItemId,
                    // 💡 Supports both positive deductions (ordering) and negative additions (updates/returns)
                    TotalDeduction = recipe.QuantityRequired * orderedQuantity
                });
            }

            // 4. Send the sanitized parameters down to the DLL under the exact same transaction context
            return await _inventoryDLL.DeductRawStockAsync(deductionPayloads);
        }
        public async Task<bool> DeleteInventoryItem(int id, int deletedby)
        {
            try
            {

               var item =  await _inventoryDLL.GetInventoryItemById(id);
                if (item==null)
                {
                    throw new Exception("sorry no item found" );
                }
                return await _inventoryDLL.DeleteInventoryItem(id,deletedby);
            }catch(Exception ex)
            {
                throw new Exception("Server Error");
            }
        }
        public async Task<IEnumerable<InventoryItem>> GetAllInventoryItemsAsync(int page = 1)
        {
            try
            {
                int pageSize = 10;

                int offset = (page - 1) * pageSize;

                return await _inventoryDLL.GetAllInventoryItemsAsync(offset, pageSize);
            }catch(Exception ex)
            {
                throw new Exception("Server Error"+ex);
            }
        }
    }
}