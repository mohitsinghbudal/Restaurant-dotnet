using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.Inventory;
using HotelManagementSystem.Models.Inventory;
using System.Data;

namespace HotelManagementSystem.DLL.InventoryDLL
{
    public class InventoryDLL : IInventoryDLL
    {
        private readonly IDbConnectionFactory _dbConn;
        public InventoryDLL (IDbConnectionFactory dbConn)
        {
            _dbConn = dbConn;
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryItemAsync()
        {
            using var conn = _dbConn.CreateConnection();
            string sql = @"SELECT * FROM InventoryItems";

            var result = await conn.QueryAsync<InventoryItem>(sql);
            return result;

        }
        public async Task<InventoryItem> AddInventoryItem(InventoryItem inventoryItem)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
        INSERT INTO InventoryItems
        (
            ItemName,
            UnitId,
            CurrentQuantity,
            MinimumQuantity,
            CostPrice,
            IsActive,
            CreatedBy,
            CreatedOn
        )
        OUTPUT INSERTED.*
        VALUES
        (
            @ItemName,
            @UnitId,
            @CurrentQuantity,
            @MinimumQuantity,
            @CostPrice,
            @IsActive,
            @CreatedBy,
            @CreatedOn
        );";

            return await conn.QuerySingleAsync<InventoryItem>(sql, inventoryItem);
        }
        public async Task<int> UpdateInventoryItem(InventoryItem inventoryItem)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
        UPDATE InventoryItems
        SET
            ItemName = @ItemName,
            UnitId = @UnitId,
            CurrentQuantity = @CurrentQuantity,
            MinimumQuantity = @MinimumQuantity,
            CostPrice = @CostPrice,
            UpdatedBy = @UpdatedBy,
            UpdatedOn = @UpdatedOn
        WHERE InventoryItemId = @InventoryItemId;";

            return await conn.ExecuteAsync(sql, inventoryItem);
        }
        public async Task<InventoryItem?> GetInventoryItemById(int id)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
        SELECT *
        FROM InventoryItems
        WHERE InventoryItemId = @Id
          AND IsActive = 1;";

            return await conn.QueryFirstOrDefaultAsync<InventoryItem>(
                sql,
                new { Id = id });
        }
    }
}
