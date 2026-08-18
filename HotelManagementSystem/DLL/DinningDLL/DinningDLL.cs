using Dapper;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Models.Dinning;
using System;
using System.Threading.Tasks;

namespace HotelManagementSystem.DLL.DinningDLL
{
    public class DinningDLL : IDinningDLL
    {
        private readonly IDbConnectionFactory _dbconn;

        public DinningDLL(IDbConnectionFactory dbconn)
        {
            _dbconn = dbconn;
        }

        public async Task<IEnumerable<DinningModel>> GetAllDinningSessions()
        {
            using var connection = _dbconn.CreateConnection();

            var sql = "SELECT * FROM DinningSessions";

            return await connection.QueryAsync<DinningModel>(sql);
        }

        public async Task<int> GetDiningSession(int userId)
        {
            using var connection = _dbconn.CreateConnection();

            var sql = @"
        SELECT sessionid from DinningSessions WHERE CreatedBy = @UserId AND SessionStatus = 'Active';";

            var sessionId = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { UserId = userId });

            return sessionId ?? 0;
        }
        public async Task<int> CreateDinningAsync(DinningModel dinning)
        {
            using var connection = _dbconn.CreateConnection();

            
            
            var sql = @"
                INSERT INTO DinningSessions (TableId, StartedAt, SessionStatus, UpdatedAt,CreatedBy)
                OUTPUT INSERTED.SessionId
                VALUES (@TableId, GETUTCDATE(), @SessionStatus, NULL,@CreatedBy);";

            
            var sessionId = await connection.QuerySingleAsync<int>(sql, dinning);
            Console.WriteLine(sessionId);

            return sessionId;
        }

        public async Task<DinningModel> GetDinningByIdAsync(int sessionId)
        {
            using var connection = _dbconn.CreateConnection();

            var sql = "SELECT * FROM DinningSessions WHERE SessionId = @SessionId;";
            var dinning = await connection.QuerySingleOrDefaultAsync<DinningModel>(sql, new { SessionId = sessionId });

            if (dinning == null)
            {
                throw new KeyNotFoundException($"Dining session with ID {sessionId} was not found.");
            }
            return dinning;
        }

        public async Task<int> EndDinningSessionAsync(DinningModel dinning)
        {
            using var connection = _dbconn.CreateConnection();

            
            var sql = @"
                UPDATE DinningSessions
                SET EndAt = GETUTCDATE(),
                    EndedBy =@EndedBy,
                    SessionStatus = @SessionStatus,
                    UpdatedAt = GETUTCDATE()

                WHERE SessionId = @SessionId;";

            
            return await connection.ExecuteAsync(sql, dinning);
        }

        public async Task<DinningModel> GetDinningBySessionId(int id)
        {
            using var connection = _dbconn.CreateConnection();

            string sql = @"SELECT * FROM DinningSessions WHERE SessionId = @SessionId;";

            return await connection.QuerySingleOrDefaultAsync<DinningModel>(sql, new { SessionId = id });
        }

        
        
        

        

        
        

        public async Task<int?> GetMySessionId(int userId)
            {
            using var connection = _dbconn.CreateConnection();
            string sql = @"SELECT SessionId 
                   FROM DinningSessions 
                   WHERE CreatedBy = @UserId AND SessionStatus = 'Active';";

            return await connection.QueryFirstOrDefaultAsync<int?>(sql, new { UserId = userId });
        }
        public async Task<int> GetCustomerId(int tableId)
        {
            using var connection = _dbconn.CreateConnection();

            string sql = @"
        SELECT SessionId
        FROM DinningSessions
        WHERE TableId = @TableId
          AND SessionStatus = 'Active';";

            return await connection.ExecuteScalarAsync<int>(sql, new { TableId = tableId });
        }
    }
}
