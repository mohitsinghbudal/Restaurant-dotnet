using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.Inventory;
using HotelManagementSystem.Interfaces.MenuInterface;
using HotelManagementSystem.Interfaces.RecipeInterface;
using HotelManagementSystem.Models.MenuItems;
using Dapper;

namespace HotelManagementSystem.Services.MenuService
{
    public class MenuService : IMenuServices
    {
        private readonly IMenuDLL _menuDLL;
        private readonly IRecipeDLL _recipeDLL;
        private readonly IInventoryDLL _inventoryDLL;
        private readonly IDbConnectionFactory _dbConn;

        public MenuService(
            IMenuDLL menuDLL,
            IRecipeDLL recipeDLL,
            IInventoryDLL inventoryDLL,
            IDbConnectionFactory dbConn)
        {
            _menuDLL = menuDLL;
            _recipeDLL = recipeDLL;
            _inventoryDLL = inventoryDLL;
            _dbConn = dbConn;
        }

        public async Task<int> CreateMenuItemAsync(CreateMenu menu)
        {
            // Business validation rule
            if (menu.ItemPrice <= 0)
                throw new ArgumentException("Item price must be greater than zero.");

            return await _menuDLL.CreateMenuItemAsync(menu);
        }

        public async Task<Menu?> GetMenuItemByIdAsync(int menuId)
        {
            return await _menuDLL.GetMenuItemByIdAsync(menuId);
        }

        public async Task<int> UpdateMenuAsync(UpdateMenu menu)
        {
            return await _menuDLL.UpdateMenuAsync(menu);
        }

        public async Task<IEnumerable<ShowMenu>> GetAllMenuItemsAsync()
        {
            var menus = (await _menuDLL.GetAllMenuItemsAsync()).ToList();
            var recipes = (await _recipeDLL.GetAllRecipesAsync()).ToList();
            var inventoryItems = (await _inventoryDLL.GetInventoryItemAsync()).ToList();

            if (recipes.Any())
            {
                var firstRecipe = recipes.First();
                Console.WriteLine($"[DEBUG] Raw Recipe ID: {firstRecipe.RecipeId}, MenuId: {firstRecipe.MenuId}, Qty: {firstRecipe.QuantityRequired}");
            }

            foreach (var menu in menus)
            {
                var menuRecipes = recipes
                    .Where(r => r.MenuId == menu.MenuId)
                    .ToList();

                if (!menuRecipes.Any())
                {
                    menu.AvailablePortions = 0;
                    menu.IsAvailable = false;
                    continue;
                }

                int availablePortions = int.MaxValue;

                foreach (var recipe in menuRecipes)
                {
                    var inventory = inventoryItems.FirstOrDefault(i =>
                        i.InventoryItemId == recipe.InventoryItemId);

                    if (inventory == null)
                    {
                        availablePortions = 0;
                        break;
                    }

                    Console.WriteLine(inventory.CurrentQuantity);
                    Console.WriteLine(recipe.QuantityRequired);

                    if (recipe.QuantityRequired <= 0)
                    {
                        continue;
                    }
                    int possible = (int)(inventory.CurrentQuantity / recipe.QuantityRequired);
                    availablePortions = Math.Min(availablePortions, possible);
                }

                menu.AvailablePortions = availablePortions == int.MaxValue ? 0 : availablePortions;

                // Respect manual availability flag alongside real stock levels
                menu.IsAvailable = menu.IsAvailable && menu.AvailablePortions > 0;
            }

            return menus;
        }

        public async Task<int> GetAvailablePortionsAsync(int menuId)
        {
            var recipes = (await _recipeDLL.GetRecipeByMenuIdAsync(menuId)).ToList();

            if (!recipes.Any())
                return 0;

            // PERFORMANCE OPTIMIZATION: Pull all inventory records into memory in 1 query 
            // instead of running an individual DB connection trip inside the foreach loop.
            var inventoryItems = (await _inventoryDLL.GetInventoryItemAsync()).ToList();

            int available = int.MaxValue;

            foreach (var recipe in recipes)
            {
                var inventory = inventoryItems.FirstOrDefault(i => i.InventoryItemId == recipe.InventoryItemId);

                if (inventory == null)
                    return 0;

                int possible = (int)(inventory.CurrentQuantity / recipe.QuantityRequired);
                available = Math.Min(available, possible);
            }

            return available == int.MaxValue ? 0 : available;
        }

        public async Task<bool> IsMenuAvailableAsync(int menuId)
        {
            return await GetAvailablePortionsAsync(menuId) > 0;
        }

        public async Task<bool> DeductInventoryForOrderAsync(int menuId, int orderedQuantity)
        {
            // 1. Fetch recipe items required for this dish
            var recipes = await _recipeDLL.GetRecipeByMenuIdAsync(menuId);
            if (!recipes.Any()) return true; // Direct retail item without raw recipes (e.g. Coke, Mineral Water)

            using var connection = _dbConn.CreateConnection();

            // Explicitly open connection before invoking a local transaction block
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            // Use a transaction so if one ingredient fails to deduct, everything rolls back safely
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var recipe in recipes)
                {
                    decimal totalDeduction = recipe.QuantityRequired * orderedQuantity;

                    string sql = @"
                        UPDATE Inventory 
                        SET CurrentQuantity = CurrentQuantity - @Deduction
                        WHERE InventoryItemId = @InventoryItemId 
                          AND CurrentQuantity >= @Deduction;";

                    int rowsAffected = await connection.ExecuteAsync(sql,
                        new { Deduction = totalDeduction, InventoryItemId = recipe.InventoryItemId },
                        transaction);

                    if (rowsAffected == 0)
                    {
                        // Operational failure: Insufficient stock for this component!
                        transaction.Rollback();
                        return false;
                    }
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}