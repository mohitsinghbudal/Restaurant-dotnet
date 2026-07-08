using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.MenuInterface;
using HotelManagementSystem.Models.MenuItems;

namespace HotelManagementSystem.DLL.MenuDLL
{
    public class MenuDLL : IMenuDLL
    {
        private readonly IDbConnectionFactory _dbConn;

        public MenuDLL(IDbConnectionFactory dbConn)
        {
            _dbConn = dbConn;
        }

        public async Task<int> CreateMenuItemAsync(CreateMenu menu)
        {
            using var connection = _dbConn.CreateConnection();

            string sql = @"
                INSERT INTO Menus
                (
                    ItemName,
                    ItemDescription,
                    CategoryId,
                    SubCategoryId,
                    ItemImage,
                    ItemPrice,
                    UnitId,
                    IsAvailable,
                    IsActive,
                    CreatedBy,
                    CreatedOn
                )
                VALUES
                (
                    @ItemName,
                    @ItemDescription,
                    @CategoryId,
                    @SubCategroyId,
                    @ItemImage,
                    @ItemPrice,
                    @UnitId,
                    1,
                    1,
                    @CreatedBy,
                    @CreatedOn
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.ExecuteScalarAsync<int>(sql, menu);
        }

        public async Task<Menu?> GetMenuItemByIdAsync(int menuId)
        {
            using var connection = _dbConn.CreateConnection();

            string sql = @"
                SELECT *
                FROM Menus
                WHERE MenuId = @MenuId
                  AND IsActive = 1;";

            return await connection.QuerySingleOrDefaultAsync<Menu>(
                sql,
                new { MenuId = menuId });
        }

        public async Task<IEnumerable<ShowMenu>> GetAllMenuItemsAsync()
        {
            using var connection = _dbConn.CreateConnection();

            string sql = @"
                SELECT
                    MenuId,
                    ItemName,
                    ItemDescription,
                    CategoryId,
                    SubCategoryId,
                    ItemImage,
                    ItemPrice,
                    UnitId,
                    IsAvailable,
                    IsActive,
                    CreatedBy,
                    CreatedOn,
                    LastUpdatedBy,
                    LastUpdatedOn
                FROM Menus
                WHERE IsActive = 1;";

            return await connection.QueryAsync<ShowMenu>(sql);
        }

        public async Task<int> UpdateMenuAsync(Menu menu)
        {
            using var connection = _dbConn.CreateConnection();

            string sql = @"
                UPDATE Menus
                SET
                    ItemName = @ItemName,
                    ItemDescription = @ItemDescription,
                    CategoryId = @CategoryId,
                    SubCategoryId = @SubCategoryId,
                    ItemImage = @ItemImage,
                    ItemPrice = @ItemPrice,
                    UnitId = @UnitId,
                    IsAvailable = @IsAvailable,
                    LastUpdatedBy = @LastUpdatedBy,
                    LastUpdatedOn = SYSUTCDATETIME()
                WHERE MenuId = @MenuId
                  AND IsActive = 1;";

            return await connection.ExecuteAsync(sql, menu);
        }
    }
}