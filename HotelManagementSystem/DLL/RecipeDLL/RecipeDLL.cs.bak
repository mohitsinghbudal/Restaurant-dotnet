using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.RecipeInterface;
using HotelManagementSystem.Models.Recipe;
using Dapper;

namespace HotelManagementSystem.DLL.RecipeDLL
{
    public class RecipeDLL : IRecipeDLL
    {
        private readonly IDbConnectionFactory _dbConn;

        public RecipeDLL(IDbConnectionFactory dbConn)
        {
            _dbConn = dbConn;
        }


        public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
                INSERT INTO Recipe
                (
                    MenuId,
                    InventoryItemId,
                    QuantityRequired,
                    UnitId,
                    IsActive,
                    CreatedBy,
                    CreatedOn
                )
                OUTPUT INSERTED.*
                VALUES
                (
                    @MenuId,
                    @InventoryItemId,
                    @QuantityRequired,
                    @UnitId,
                    @IsActive,
                    @CreatedBy,
                    @CreatedOn
                );";

            return await conn.QuerySingleAsync<Recipe>(sql, recipe);
        }


        public async Task<IEnumerable<Recipe>> GetRecipeByMenuIdAsync(int menuId)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
                SELECT *
                FROM Recipe
                WHERE MenuId = @MenuId
                AND IsActive = 1;";

            return await conn.QueryAsync<Recipe>(
                sql,
                new { MenuId = menuId });
        }


        //public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        //{
        //    using var conn = _dbConn.CreateConnection();

        //    string sql = @"
        //        SELECT *
        //        FROM Recipe
        //        WHERE IsActive = 1;";

        //    return await conn.QueryAsync<Recipe>(sql);
        //}
        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            using var conn = _dbConn.CreateConnection();

            // Explicitly select and alias the columns to guarantee Dapper matches them
            string sql = @"
        SELECT 
            RecipeId,
            MenuId,
            InventoryItemId,
            QuantityRequired, 
            UnitId,
            IsActive,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn
        FROM Recipe;";

            return await conn.QueryAsync<Recipe>(sql);
        }


        public async Task<int> UpdateRecipeAsync(Recipe recipe)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
                UPDATE Recipe
                SET
                    MenuId = @MenuId,
                    InventoryItemId = @InventoryItemId,
                    QuantityRequired = @QuantityRequired,
                    UpdatedBy = @UpdatedBy,
                    UpdatedOn = GETUTCDATE()
                WHERE RecipeId = @RecipeId;";

            return await conn.ExecuteAsync(sql, recipe);
        }


        public async Task<int> DeleteRecipeAsync(int recipeId)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
                UPDATE Recipe
                SET
                    IsActive = 0,
                    UpdatedOn = GETUTCDATE()
                WHERE RecipeId = @RecipeId;";

            return await conn.ExecuteAsync(
                sql,
                new { RecipeId = recipeId });
        }
    }
}