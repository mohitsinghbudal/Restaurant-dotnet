using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.Table;

namespace HotelManagementSystem.DLL.Tables
{
    public class TableDLL : ITableDLL
    {
        private readonly IDbConnectionFactory _dbConnection;

        private readonly IUserDLL _userDLL;


        public TableDLL(IDbConnectionFactory dbConnection, IUserDLL userDLL)
        {
            _dbConnection = dbConnection;
            _userDLL = userDLL;
        }

        public async Task<TableModel> CreateTableAsync(TableModel table)
        {
            using var connection = _dbConnection.CreateConnection();
            {
                var sql = @"
    INSERT INTO Tables (TableNo, Capacity, Status, CreatedAt,CreatedBy, UpdatedAt) 
    OUTPUT INSERTED.*
    VALUES (@TableNo, @Capacity, @Status,@CreatedBy GETUTCDATE(), GETUTCDATE());";

                return await connection.QuerySingleOrDefaultAsync<TableModel>(sql, table);
            }
        }
        public async Task<TableModel> GetTableByIdAsync(int tableId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"SELECT * FROM Tables WHERE TableId = @TableId;";
            return await connection.QuerySingleOrDefaultAsync<TableModel>(sql, new { TableId = tableId });
        }

        public async Task<TableModel> GetTableByTableNoAsync(int tableNo)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"SELECT * FROM Tables WHERE TableNo = @TableNo;";
            return await connection.QuerySingleOrDefaultAsync<TableModel>(sql, new { TableNo = tableNo });
        }

        public async Task<int> UpdateTableAsync(UpdateTable table)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"UPDATE Tables 
                SET Status = @Status, 
                    UpdatedAt = @UpdatedAt, 
                    UpdatedBy = @UpdatedBy 
                WHERE TableNo = @TableNo;";
            return await connection.ExecuteAsync(sql,table);

        }
        public async Task<int> BookTableAsync(TableModel table)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"UPDATE Tables 
                SET Status = @Status, 
                    UpdatedAt = @UpdatedAt, 
                    CreatedBy = @CreatedBy, 
                    WaiterId = @WaiterId
                WHERE TableNo = @TableNo;";
            return await connection.ExecuteAsync(sql, table);
        }

    }
}
