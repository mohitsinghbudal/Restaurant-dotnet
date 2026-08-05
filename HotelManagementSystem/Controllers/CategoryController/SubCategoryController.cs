using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.SubCategoryInterface;
using HotelManagementSystem.Models.Categories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelManagementSystem.Controllers.CategoryController
{
    public class SubCategoryController : ISubCategoryDLL
    {
        private readonly IDbConnectionFactory _dbConn;

        public SubCategoryController(IDbConnectionFactory dbConn)
        {
            _dbConn = dbConn;
        }

        public async Task<int> AddSubCategoryAsync(SubCategory subCategory)
        {
            // Authorization note: callers should verify role membership with User.IsInRole("5") instead of numeric RoleId checks.
            using var conn = _dbConn.CreateConnection();
            var sql = @"
INSERT INTO SubCategories (CategoryId, SubCategoryName, [Description], IsAvailable, IsActive, CreatedBy, CreatedOn, DisplayOrder)
OUTPUT INSERTED.SubCategoryId
VALUES (@CategoryId, @SubCategoryName, @Description, @IsAvailable, 1, @CreatedBy, GETUTCDATE(), @DisplayOrder);";
            return await conn.QuerySingleAsync<int>(sql, subCategory);
        }

        public async Task<IEnumerable<SubCategory>> GetAllSubCategoriesAsync()
        {
            using var conn = _dbConn.CreateConnection();
            var sql = @"SELECT * FROM SubCategories ORDER BY DisplayOrder ASC, SubCategoryName ASC;";
            return await conn.QueryAsync<SubCategory>(sql);
        }

        public async Task<IEnumerable<SubCategory>> GetSubCategoriesByCategoryIdAsync(int categoryId)
        {
            using var conn = _dbConn.CreateConnection();
            var sql = @"SELECT * FROM SubCategories WHERE CategoryId = @CategoryId ORDER BY DisplayOrder ASC, SubCategoryName ASC;";
            return await conn.QueryAsync<SubCategory>(sql, new { CategoryId = categoryId });
        }

        public async Task<SubCategory?> GetSubCategoryByIdAsync(int subCategoryId)
        {
            using var conn = _dbConn.CreateConnection();
            var sql = @"SELECT * FROM SubCategories WHERE SubCategoryId = @SubCategoryId";
            return await conn.QuerySingleOrDefaultAsync<SubCategory>(sql, new { SubCategoryId = subCategoryId });
        }

        public async Task<int> UpdateSubCategoryAsync(SubCategory subCategory)
        {
            // Authorization note: callers should verify role membership with User.IsInRole("5") instead of numeric RoleId checks.
            using var conn = _dbConn.CreateConnection();
            var sql = @"
UPDATE SubCategories
SET SubCategoryName = @SubCategoryName,
    [Description] = @Description,
    IsAvailable = @IsAvailable,
    IsActive = @IsActive,
    UpdatedBy = @UpdatedBy,
    UpdatedOn = GETUTCDATE(),
    DisplayOrder = @DisplayOrder
WHERE SubCategoryId = @SubCategoryId ";
            return await conn.ExecuteAsync(sql, subCategory);
        }

        public async Task<int> SoftDeleteSubCategoryAsync(int subCategoryId, int deletedBy)
        {
            // Authorization note: callers should verify role membership with User.IsInRole("5") instead of numeric RoleId checks.
            using var conn = _dbConn.CreateConnection();
            var sql = @"
UPDATE SubCategories
SET IsActive = 0,
    UpdatedBy = @UpdatedBy,
    UpdatedOn = GETUTCDATE()
WHERE SubCategoryId = @SubCategoryId;";
            return await conn.ExecuteAsync(sql, new { SubCategoryId = subCategoryId, UpdatedBy = deletedBy });
        }
    }
}
