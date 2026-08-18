using HotelManagementSystem.Helper.ClaimHelper;
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

        
        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromBody] Recipe recipe)
        {
            if (!User.IsInRole("5")) 
            {
                throw new Exception("User not allowed");
            }
            var createdRecipe = await _recipeService.CreateRecipeAsync(recipe);

            return Ok(new
            {
                message = "Recipe created successfully.",
                item = createdRecipe
            });
        }

        
        [HttpPut]
        public async Task<IActionResult> UpdateRecipe([FromBody] Recipe recipe)
        {
            if (!User.IsInRole("5")) 
            {
                throw new Exception("User not allowed");
            }
            int userId = ClaimHelper.GetUserId(User);
            

            var result = await _recipeService.UpdateRecipeAsync(recipe , userId);

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

        
        [HttpDelete("{recipeId}")]
        public async Task<IActionResult> DeleteRecipe(int recipeId)
        {
            if (!User.IsInRole("5")) 
            {
                throw new Exception("User not allowed");
            }
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
