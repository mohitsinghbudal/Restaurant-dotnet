using HotelManagementSystem.Interfaces.CategoryInterface;
using HotelManagementSystem.Models.Categories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.CategoryController
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            if (!User.IsInRole("5"))
                throw new Exception("User not allowed");
            if (category == null)
                return BadRequest(new { message = "Invalid category payload." });

            try
            {
                var id = await _categoryService.CreateCategoryAsync(category);
                return Ok(new { categoryId = id, message = "Category created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error fetching categories.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid Category ID." });

            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            if (!User.IsInRole("5"))
                throw new Exception("User not allowed");
            if (category == null || category.CategoryId <= 0)
                return BadRequest(new { message = "Invalid update payload details." });

            try
            {
                var result = await _categoryService.UpdateCategoryAsync(category);
                if (result <= 0)
                    return BadRequest(new { message = "Failed to update category record." });

                return Ok(new { message = "Category updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("Delete/{id}/{deletedBy}")]
        public async Task<IActionResult> DeleteCategory(int id, int deletedBy)
        {
            if (!User.IsInRole("5"))
                throw new Exception("User not allowed");
            if (id <= 0 || deletedBy <= 0)
                return BadRequest(new { message = "Invalid deletion parameters provided." });

            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id, deletedBy);
                if (result <= 0)
                    return BadRequest(new { message = "Failed to execute soft delete operations." });

                return Ok(new { message = "Category soft-deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
