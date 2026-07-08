using Dapper;
using HotelManagementSystem.Interfaces.CategoryInterface;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Models.Categories;

namespace HotelManagementSystem.DLL.CategoryDLL
{
    public class CategoryDLL : ICategoryDLL
    {
        private readonly IDbConnectionFactory _dbConn;

        public CategoryDLL(IDbConnectionFactory dbConn)
        {
            _dbConn = dbConn;
        }

        public async Task<int> AddCategoryAsync(Category category)
        {
            using var conn = _dbConn.CreateConnection();

            var sql = @"
                INSERT INTO Categories 
                (CategoryName, [Description], IsAvailable, IsActive, CreatedBy, CreatedOn, DisplayOrder)
                OUTPUT INSERTED.CategoryId
                VALUES 
                (@CategoryName, @Description, @IsAvailable, 1, @CreatedBy, GETUTCDATE(), @DisplayOrder);";

            return await conn.QuerySingleAsync<int>(sql, category);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            using var conn = _dbConn.CreateConnection();

            // Sorts automatically by your display configuration order
            string sql = @"SELECT * FROM Categories 
                           WHERE IsActive = 1 
                           ORDER BY DisplayOrder ASC, CategoryName ASC;";

            return await conn.QueryAsync<Category>(sql);
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            using var conn = _dbConn.CreateConnection();
            string sql = @"SELECT * FROM Categories WHERE CategoryId = @CategoryId AND IsActive = 1;";
            return await conn.QueryFirstOrDefaultAsync<Category>(sql, new { CategoryId = categoryId });
        }

        public async Task<int> UpdateCategoryAsync(Category category)
        {
            using var conn = _dbConn.CreateConnection();

            // Maps exactly to UpdatedBy and UpdatedOn property schemas
            var sql = @"
                UPDATE Categories 
                SET CategoryName = @CategoryName,
                    [Description] = @Description,
                    IsAvailable = @IsAvailable,
                    UpdatedBy = @UpdatedBy,
                    UpdatedOn = GETUTCDATE(),
                    DisplayOrder = @DisplayOrder
                WHERE CategoryId = @CategoryId AND IsActive = 1;";

            return await conn.ExecuteAsync(sql, category);
        }

        public async Task<int> SoftDeleteCategoryAsync(int categoryId, int deletedBy)
        {
            using var conn = _dbConn.CreateConnection();

            var sql = @"
                UPDATE Categories 
                SET IsActive = 0,
                    UpdatedBy = @UpdatedBy,
                    UpdatedOn = GETUTCDATE()
                WHERE CategoryId = @CategoryId;";

            return await conn.ExecuteAsync(sql, new { CategoryId = categoryId, UpdatedBy = deletedBy });
        }
    }
}
