using HotelManagementSystem.Interfaces.CategoryInterface;
using HotelManagementSystem.Interfaces.SubCategoryInterface;
using HotelManagementSystem.Models.Categories;

namespace HotelManagementSystem.Services.Categories
{
    public class SubCategoryServices : ISubCategoryService
    {
        private readonly ISubCategoryDLL _subCategoryDLL;
        private readonly ICategoryDLL _categoryDLL; 

        public SubCategoryServices(ISubCategoryDLL subCategoryDLL, ICategoryDLL categoryDLL)
        {
            _subCategoryDLL = subCategoryDLL;
            _categoryDLL = categoryDLL;
        }

        public async Task<int> CreateSubCategoryAsync(SubCategory subCategory)
        {
            if (subCategory == null) throw new ArgumentNullException(nameof(subCategory));
            if (string.IsNullOrWhiteSpace(subCategory.SubCategoryName)) throw new ArgumentException("SubCategory Name is required.");

            
            var parentCategory = await _categoryDLL.GetCategoryByIdAsync(subCategory.CategoryId);
            if (parentCategory == null)
            {
                throw new KeyNotFoundException($"Cannot create SubCategory. Parent Category with ID {subCategory.CategoryId} does not exist.");
            }

            return await _subCategoryDLL.AddSubCategoryAsync(subCategory);
        }

        public async Task<IEnumerable<SubCategory>> GetAllSubCategoriesAsync()
        {
            return await _subCategoryDLL.GetAllSubCategoriesAsync();
        }

        public async Task<IEnumerable<SubCategory>> GetSubCategoriesByCategoryIdAsync(int categoryId)
        {
            if (categoryId <= 0) throw new ArgumentException("Invalid Category ID.");
            return await _subCategoryDLL.GetSubCategoriesByCategoryIdAsync(categoryId);
        }

        public async Task<SubCategory> GetSubCategoryByIdAsync(int subCategoryId)
        {
            var subCategory = await _subCategoryDLL.GetSubCategoryByIdAsync(subCategoryId);
            if (subCategory == null)
            {
                throw new KeyNotFoundException($"SubCategory with ID {subCategoryId} was not found.");
            }
            return subCategory;
        }

        public async Task<int> UpdateSubCategoryAsync(SubCategory subCategory)
        {
            if (subCategory == null) throw new ArgumentNullException(nameof(subCategory));

            
            await GetSubCategoryByIdAsync(subCategory.SubCategoryId);

            
            var parentCategory = await _categoryDLL.GetCategoryByIdAsync(subCategory.CategoryId);

            if (parentCategory == null)
            {
                throw new KeyNotFoundException($"Cannot update SubCategory. Target Category with ID {subCategory.CategoryId} does not exist.");
            }

            return await _subCategoryDLL.UpdateSubCategoryAsync(subCategory);
        }

        public async Task<int> DeleteSubCategoryAsync(int subCategoryId, int deletedBy)
        {
            
            await GetSubCategoryByIdAsync(subCategoryId);
            return await _subCategoryDLL.SoftDeleteSubCategoryAsync(subCategoryId, deletedBy);
        }
    }
}

