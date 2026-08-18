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
            string sql = @"SELECT * FROM InventoryItems WHERE IsDeleted = 0";

            return await conn.QueryAsync<InventoryItem>(sql);
        }

        public async Task<IEnumerable<InventoryItem>> GetAllInventoryItemsAsync(int offset, int pageSize)
        {
           try{ using var conn = _dbConn.CreateConnection();



            string sql = @" SELECT * 
                            FROM InventoryItems
                            ORDER BY InventoryItemId 
                            OFFSET @Offset ROWS
                            FETCH NEXT @PageSize ROWS ONLY;";


                return await conn.QueryAsync<InventoryItem>(sql, new
                {
                    Offset = offset,
                    PageSize = pageSize
                });
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
                    IsDeleted,
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
                    0, -- Default IsDeleted to false
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
                    IsActive = @IsActive,
                    UpdatedBy = @UpdatedBy,
                    UpdatedOn = @UpdatedOn
                WHERE InventoryItemId = @InventoryItemId 
                  AND IsDeleted = 0;";

            return await conn.ExecuteAsync(sql, inventoryItem);
        }

        public async Task<InventoryItem?> GetInventoryItemById(int id)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
                SELECT *
                FROM InventoryItems
                WHERE InventoryItemId = @Id
                  AND IsDeleted = 0;";

            return await conn.QueryFirstOrDefaultAsync<InventoryItem>(sql, new { Id = id });
        }

        public async Task<bool> DeductRawStockAsync(IEnumerable<InventoryDeductionModel> itemsToDeduct)
        {
            using var connection = _dbConn.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var item in itemsToDeduct)
                {
                    string sql = @"
                        UPDATE InventoryItems 
                        SET CurrentQuantity = CurrentQuantity - @Deduction
                        WHERE InventoryItemId = @InventoryItemId 
                          AND IsDeleted = 0
                          AND CurrentQuantity >= @Deduction;";

                    int rowsAffected = await connection.ExecuteAsync(
                        sql,
                        new { Deduction = item.TotalDeduction, InventoryItemId = item.InventoryItemId },
                        transaction
                    );

                    // If stock is insufficient or item is deleted, abort entire transaction
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

        public async Task<bool> DeleteInventoryItem(int id, int deletedBy)
        {
            try
            {
                using var connection = _dbConn.CreateConnection();

                const string sql = @"
                    UPDATE InventoryItems 
                    SET 
                        IsDeleted = 1,
                        DeletedOn = GETUTCDATE(),
                        DeletedBy = @DeletedBy
                    WHERE InventoryItemId = @Id 
                      AND IsDeleted = 0;";

                int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, DeletedBy = deletedBy });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error while performing soft delete: " + ex.Message, ex);
            }
        }
    }
}