using HotelManagementSystem.Interfaces.RecipeInterface;
using HotelManagementSystem.Models.Recipe;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.RecipeController
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        // GET: api/Recipe
        [HttpGet]
        public async Task<IActionResult> GetAllRecipes()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();

            return Ok(new
            {
                message = "Success",
                items = recipes
            });
        }

        // GET: api/Recipe/menu/1
        [HttpGet("menu/{menuId}")]
        public async Task<IActionResult> GetRecipeByMenuId(int menuId)
        {
            var recipes = await _recipeService.GetRecipeByMenuIdAsync(menuId);

            return Ok(new
            {
                message = "Success",
                items = recipes
            });
        }

        // POST: api/Recipe
        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromBody] Recipe recipe)
        {
            var createdRecipe = await _recipeService.CreateRecipeAsync(recipe);

            return Ok(new
            {
                message = "Recipe created successfully.",
                item = createdRecipe
            });
        }

        // PUT: api/Recipe
        [HttpPut]
        public async Task<IActionResult> UpdateRecipe([FromBody] Recipe recipe)
        {
            var result = await _recipeService.UpdateRecipeAsync(recipe);

            if (result == 0)
            {
                return NotFound(new
                {
                    message = "Recipe not found."
                });
            }

            return Ok(new
            {
                message = "Recipe updated successfully."
            });
        }

        // DELETE: api/Recipe/5
        [HttpDelete("{recipeId}")]
        public async Task<IActionResult> DeleteRecipe(int recipeId)
        {
            var result = await _recipeService.DeleteRecipeAsync(recipeId);

            if (result == 0)
            {
                return NotFound(new
                {
                    message = "Recipe not found."
                });
            }

            return Ok(new
            {
                message = "Recipe deleted successfully."
            });
        }
    }
}