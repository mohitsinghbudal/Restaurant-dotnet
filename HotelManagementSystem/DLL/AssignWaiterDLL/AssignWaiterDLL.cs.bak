using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.User;
using HotelManagementSystem.Models.User;

namespace HotelManagementSystem.DLL.AssignWaiterDLL
{
    public class AssignWaiterDLL : IWaiterDLL
    {
        private readonly IDbConnectionFactory _dbconn;

        public AssignWaiterDLL(IDbConnectionFactory dbconn)
        {
            _dbconn = dbconn;
        }
        public async Task<int?> AssignWaiterAsync()
        {
            using var conn = _dbconn.CreateConnection();

            var sql = @"
        SELECT 
            u.UserId,
            u.FirstName,
            COUNT(d.SessionId) AS ActiveSessions
        FROM Users u
        LEFT JOIN Dinning d
            ON u.UserId = d.WaiterUserId
            AND d.SessionStatus <> 'Completed'
            AND d.UpdatedAt < DATEADD(HOUR, -6, GETDATE())
        WHERE u.RoleId = 3
          AND u.IsActive = 1
        GROUP BY u.UserId, u.FirstName
        ORDER BY ActiveSessions ASC, u.UserId ASC;";

            var waiters = (await conn.QueryAsync<WaiterAssignDTO>(sql)).ToList();

            if (!waiters.Any())
                return null;

            // Least busy waiter
            return waiters.First().UserId;
        }
        
    }
}
