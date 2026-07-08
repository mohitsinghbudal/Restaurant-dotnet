using HotelManagementSystem.Interfaces.CategoryInterface;
using HotelManagementSystem.Models.Categories;
namespace HotelManagementSystem.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryDLL _categoryDLL;

        public CategoryService(ICategoryDLL categoryDLL)
        {
            _categoryDLL = categoryDLL;
        }

        public async Task<int> CreateCategoryAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            if (string.IsNullOrWhiteSpace(category.CategoryName)) throw new ArgumentException("Category Name is required.");

            return await _categoryDLL.AddCategoryAsync(category);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryDLL.GetAllCategoriesAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryDLL.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} does not exist.");
            }
            return category;
        }

        public async Task<int> UpdateCategoryAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            // Check if it exists before editing
            await GetCategoryByIdAsync(category.CategoryId);

            return await _categoryDLL.UpdateCategoryAsync(category);
        }

        public async Task<int> DeleteCategoryAsync(int categoryId, int deletedBy)
        {
            await GetCategoryByIdAsync(categoryId);
            return await _categoryDLL.SoftDeleteCategoryAsync(categoryId, deletedBy);
        }
    }
}
