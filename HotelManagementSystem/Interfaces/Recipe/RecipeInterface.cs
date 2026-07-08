using HotelManagementSystem.Models.Recipe;

namespace HotelManagementSystem.Interfaces.RecipeInterface
{
    public interface IRecipeDLL
    {
        Task<Recipe> CreateRecipeAsync(Recipe recipe);
        Task<IEnumerable<Recipe>> GetRecipeByMenuIdAsync(int menuId);
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();
        Task<int> UpdateRecipeAsync(Recipe recipe);
        Task<int> DeleteRecipeAsync(int recipeId);
    }
    public interface IRecipeService
    {
        Task<Recipe> CreateRecipeAsync(Recipe recipe);

        Task<IEnumerable<Recipe>> GetRecipeByMenuIdAsync(int menuId);

        Task<IEnumerable<Recipe>> GetAllRecipesAsync();

        Task<int> UpdateRecipeAsync(Recipe recipe);

        Task<int> DeleteRecipeAsync(int recipeId);
    }
}
