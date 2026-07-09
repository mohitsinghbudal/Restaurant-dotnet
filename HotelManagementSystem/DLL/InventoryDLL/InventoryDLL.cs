using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.Inventory;
using HotelManagementSystem.Models.Inventory;
using HotelManagementSystem.Models.InventoryItem;
using System.Data;

namespace HotelManagementSystem.DLL.InventoryDLL
{
    public class InventoryDLL : IInventoryDLL
    {
        private readonly IDbConnectionFactory _dbConn;
        public InventoryDLL(IDbConnectionFactory dbConn)
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

        public async Task<bool> DeductRawStockAsync(IEnumerable<InventoryDeductionModel> itemsToDeduct)
        {
            using var connection = _dbConn.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Console.WriteLine("reached the dll invetory");
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var item in itemsToDeduct)
                {
                    string sql = @"
                        UPDATE InventoryItems 
                        SET CurrentQuantity = CurrentQuantity - @Deduction
                        WHERE InventoryItemId = @InventoryItemId 
                          AND CurrentQuantity >= @Deduction;";

                    int rowsAffected = await connection.ExecuteAsync(sql,
                        new { Deduction = item.TotalDeduction, InventoryItemId = item.InventoryItemId },
                        transaction);

                    // If any single ingredient fails validation or has insufficient stock, abort the whole order
                    if (rowsAffected == 0)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


    }

}
