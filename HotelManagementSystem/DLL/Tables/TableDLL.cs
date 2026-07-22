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
        
        public async Task<TableModel> GetMyBookings(int userId)
        {
            using var conn = _dbConnection.CreateConnection();
            string sql = @"SELECT * FROM  Tables WHERE UpdatedBy = @Id AND IsActive = 1";
            return await conn.QueryFirstOrDefaultAsync<TableModel>(sql, new {Id = userId});
        }

        public async Task<IEnumerable<TableModel>> GetMyAllBookings(int userId)
        {
            using var conn = _dbConnection.CreateConnection();

            string sql = @"
        SELECT *
        FROM Tables
        WHERE UpdatedBy = @Id
          AND IsActive = 1";

            return await conn.QueryAsync<TableModel>(sql, new { Id = userId });
        }

        public async Task<TableModel> GetTableByNo( int No)
        {
            using var conn = _dbConnection.CreateConnection();
            string sql = @"SELECT * FROM Tables WHERE TableNo = @TableNo;";
            return await conn.QueryFirstOrDefaultAsync<TableModel>(sql, new { TableNo = No });
        }

        public async Task<TableModel> CreateTableAsync(TableModel table)
        {
            using var connection = _dbConnection.CreateConnection();

            string sql = @"INSERT INTO Tables (TableNo, Capacity, [Status], CreatedBy, CreatedAt, UpdatedAt) 
                        OUTPUT INSERTED.*
                        VALUES (@TableNo, @Capacity, @Status, @CreatedBy, GETUTCDATE(), NULL);";

            return await connection.QuerySingleOrDefaultAsync<TableModel>(sql, table);
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

            // 1. Enclosed [Status] keyword in brackets
            // 2. Switched UpdatedAt to execute directly using SQL's GETUTCDATE()
            var sql = @"UPDATE Tables 
                        SET [Status] = @Status, 
                            UpdatedAt = GETUTCDATE(), 
                            UpdatedBy = @UpdatedBy 
                        WHERE TableNo = @TableNo;";

            return await connection.ExecuteAsync(sql, table);
        }

        public async Task<int> BookTableAsync(TableModel table)
        {
            // 1. Utilize your injected _userDLL dependency to dynamically 
            //    find and assign the waiter with the lowest active workload!
            int WaiterId = await _userDLL.AssignWaiterAsync();

            using var connection = _dbConnection.CreateConnection();

            // 2. Enclosed [Status] keyword in brackets
            // 3. We map the parameters explicitly to make sure Dapper knows 
            //    to read 'UpdatedBy' from your object and inject our dynamic waiter ID.
            var sql = @"UPDATE Tables 
                        SET [Status] = @Status, 
                            UpdatedAt = GETUTCDATE(), 
                            UpdatedBy = @UpdatedBy,
                            WaiterId = @WaiterId
                        WHERE TableNo = @TableNo AND IsActive = 1;";

            return await connection.ExecuteAsync(sql, new
            {
                Status = table.Status,
                UpdatedBy = table.UpdatedBy,
                WaiterId = WaiterId,
                TableNo = table.TableNo
            });
        }
        public async Task<IEnumerable<TableModel>> GetAllTable()
        {
            using var connection = _dbConnection.CreateConnection();
            string sql = @"SELECT * FROM Tables WHERE Status = 'Available' AND IsActive = 1"; 
            return await connection.QueryAsync<TableModel>(sql);
        }
    }
}