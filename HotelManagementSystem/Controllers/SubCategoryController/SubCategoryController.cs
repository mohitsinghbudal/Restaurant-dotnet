using HotelManagementSystem.Interfaces.SubCategoryInterface;
using HotelManagementSystem.Models.Categories;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.SubCategoryController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryService _subCategoryService;

        public SubCategoryController(ISubCategoryService subCategoryService)
        {
            _subCategoryService = subCategoryService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateSubCategory([FromBody] SubCategory subCategory)
        {
            if (subCategory == null)
                return BadRequest(new { message = "Invalid data payload." });

            try
            {
                var id = await _subCategoryService.CreateSubCategoryAsync(subCategory);
                return Ok(new { subCategoryId = id, message = "SubCategory created successfully." });
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

        [HttpGet("All")]
        public async Task<IActionResult> GetAllSubCategories()
        {
            try
            {
                var list = await _subCategoryService.GetAllSubCategoriesAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error pulling subcategories.", details = ex.Message });
            }
        }

        [HttpGet("ByCategory/{categoryId}")]
        public async Task<IActionResult> GetByCategoryId(int categoryId)
        {
            if (categoryId <= 0)
                return BadRequest(new { message = "Valid parent Category ID required." });

            try
            {
                var list = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(categoryId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategoryById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid SubCategory ID." });

            try
            {
                var subCategory = await _subCategoryService.GetSubCategoryByIdAsync(id);
                return Ok(subCategory);
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
        public async Task<IActionResult> UpdateSubCategory([FromBody] SubCategory subCategory)
        {
            if (subCategory == null || subCategory.SubCategoryId <= 0)
                return BadRequest(new { message = "Invalid update structural details." });

            try
            {
                var result = await _subCategoryService.UpdateSubCategoryAsync(subCategory);
                if (result <= 0)
                    return BadRequest(new { message = "Failed to update record context." });

                return Ok(new { message = "SubCategory updated successfully." });
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
        public async Task<IActionResult> DeleteSubCategory(int id, int deletedBy)
        {
            if (id <= 0 || deletedBy <= 0)
                return BadRequest(new { message = "Invalid parameters passed." });

            try
            {
                var result = await _subCategoryService.DeleteSubCategoryAsync(id, deletedBy);
                if (result <= 0)
                    return BadRequest(new { message = "Deletion operation was unsuccessful." });

                return Ok(new { message = "SubCategory soft-deleted successfully." });
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
