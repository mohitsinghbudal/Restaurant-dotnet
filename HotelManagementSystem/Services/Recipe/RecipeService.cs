using HotelManagementSystem.Interfaces.RecipeInterface;
using HotelManagementSystem.Models.Recipe;

namespace HotelManagementSystem.Services.RecipeService
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeDLL _recipeDLL;

        public RecipeService(IRecipeDLL recipeDLL)
        {
            _recipeDLL = recipeDLL;
        }

        public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
        {
            return await _recipeDLL.CreateRecipeAsync(recipe);
        }

        public async Task<IEnumerable<Recipe>> GetRecipeByMenuIdAsync(int menuId)
        {
            return await _recipeDLL.GetRecipeByMenuIdAsync(menuId);
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            return await _recipeDLL.GetAllRecipesAsync();
        }

        public async Task<int> UpdateRecipeAsync(Recipe recipe)
        {
            return await _recipeDLL.UpdateRecipeAsync(recipe);
        }

        public async Task<int> DeleteRecipeAsync(int recipeId)
        {
            return await _recipeDLL.DeleteRecipeAsync(recipeId);
        }
    }
}