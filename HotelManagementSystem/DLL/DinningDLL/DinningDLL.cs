using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Models.Dinning;

namespace HotelManagementSystem.DLL.DinningDLL
{
    public class DinningDLL : IDinningDLL
    {
        private readonly IDbConnectionFactory _dbconn;
        public DinningDLL(IDbConnectionFactory dbconn)
        {
            _dbconn = dbconn;
        }
        public async Task<int> CreateDinningAsync(DinningModel dinning)
        {
            using var connection = _dbconn.CreateConnection();
            var sql = @"
            INSERT INTO Dinning
            (TableId,CustomerUserId,StartedAt,UpdatedAt,SessionStatus
            )
            VALUES
            (@TableId,@CustomerUserId,@StartedAt,@UpdatedAt,@SessionStatus
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT); ";
            var sessionId = await connection.ExecuteScalarAsync<int>(sql, dinning);
            return sessionId;
        }

        public async Task<DinningModel> GetDinningByIdAsync(int sessionId)
        {
            using var connection = _dbconn.CreateConnection();
            var sql = "SELECT * FROM Dinning WHERE SessionId = @SessionId";
            var dinning = await connection.QuerySingleOrDefaultAsync<DinningModel>(sql, new { SessionId = sessionId });
            if (dinning == null)
            {
                throw new Exception("Error occured here please solve here");
            }
            return dinning;
        }


        public async Task<DinningModel> EndDinningSessionAsync(DinningModel sessionId)
        {
            //update the dinning table set the endat date/time + set the status to end 

            //first update the table status to cleaning
            //
            using var connection = _dbconn.CreateConnection();

            var sql = @"
            INSERT INTO Dinning
            (EndAt,UpdatedAt,SessionStatus
            )
            VALUES
            (@EndAt,@UpdatedAt,@SessionStatus
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT); ";

            var update = await connection.QuerySingleOrDefaultAsync<DinningModel>(sql, sessionId);
            if (update == null)
            {
                throw new Exception("Error in ending the dinning session");
                
            }
            return update;

        }
        public async Task<DinningModel> GetDinningBySessionId(int id)
        {
            using var connection = _dbconn.CreateConnection();

            string sql = @"select * from Dinning where SessionId = @id";

            return await connection.QueryFirstOrDefault<DinningModel>(sql, new { id = id });

        }
    }
}
