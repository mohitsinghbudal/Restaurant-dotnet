using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.User;

namespace HotelManagementSystem.DLL.Users
{
    public class UserDLL : IUserDLL
    {
        private readonly IDbConnectionFactory _dbConnection;

        public UserDLL(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            using var connection = _dbConnection.CreateConnection();

            const string sql = @"
                SELECT *
                FROM Users;";

            return await connection.QueryAsync<UserModel>(sql);
        }

        public async Task<UserModel?> GetUserByEmailAsync(string email)
        {
            using var connection = _dbConnection.CreateConnection();

            const string sql = @"
                SELECT *
                FROM Users
                WHERE Email = @Email;";

            return await connection.QuerySingleOrDefaultAsync<UserModel>(
                sql,
                new { Email = email });
        }

        public async Task<int> SignUp(UserModel user)
        {
            using var connection = _dbConnection.CreateConnection();

            const string sql = @"
                INSERT INTO Users
(FirstName,MiddleName,LastName,PhoneNo,Email,PasswordHash,RoleId,IsActive,CreatedAt
)
VALUES
(@FirstName,@MiddleName,@LastName,@PhoneNo,@Email,@PasswordHash,@RoleId,@IsActive,@CreatedAt
)";

            return await connection.ExecuteAsync(sql, user);
        }

        public async Task<int> AssignWaiterAsync()
        {
            using var conn = _dbConnection.CreateConnection();

            var sql = @"
SELECT
    u.UserId,
    u.FirstName,
    COUNT(d.SessionId) AS ActiveSessions
FROM Users u
LEFT JOIN Tables t
    ON u.UserId = t.WaiterId
LEFT JOIN Dinning d
    ON d.TableId = t.TableId
    AND d.SessionStatus <> 'Completed'
    AND d.UpdatedAt > DATEADD(HOUR, -6, GETDATE())
WHERE u.RoleId = 3
  AND u.IsActive = 1
GROUP BY
    u.UserId,
    u.FirstName
ORDER BY
    ActiveSessions ASC,
    u.UserId ASC;";

            var waiters = await conn.QueryAsync(sql);

            var waiter = waiters.FirstOrDefault();

            if (waiter == null)
                throw new Exception("No active waiter found.");

            return waiter.UserId;
        }
    }
}