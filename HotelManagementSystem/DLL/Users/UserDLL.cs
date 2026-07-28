using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.User;
using static QRCoder.PayloadGenerator;

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
        SELECT TOP 1
            u.UserId,
            u.FirstName,
            COUNT(d.SessionId) AS ActiveSessions
        FROM Users u
        LEFT JOIN Tables t 
            ON u.UserId = t.WaiterId
        LEFT JOIN DinningSessions d 
            ON d.TableId = t.TableId
            AND d.SessionStatus <> 'Completed'
            AND d.UpdatedAt > DATEADD(HOUR, -6, GETUTCDATE())
        WHERE u.RoleId = 2  
          AND u.IsActive = 1
        GROUP BY 
            u.UserId, 
            u.FirstName
        ORDER BY 
            ActiveSessions ASC, 
            u.UserId ASC;";

            // Use <dynamic> so Dapper can implicitly map the properties without needing a new class
            var waiter = await conn.QueryFirstOrDefaultAsync<dynamic>(sql);

            if (waiter == null)
                throw new InvalidOperationException("No active waiters found in the system.");

            return (int)waiter.UserId;
        }

        public async Task<bool> VerifyOtpUpdate(string email)
        {
            try{
                
                using var conn = _dbConnection.CreateConnection();

                string sql = @"
                    UPDATE Users
                    SET 
                        IsEmailVerified = 1,
                        EmailOtp = NULL,
                        OtpExpiry = GETUTCDATE(),
                        UpdatedAt = GETUTCDATE()
                    WHERE Email = @Email;";

                int rowsAffected = await conn.ExecuteAsync(sql, new { Email = email });
                return rowsAffected > 0;

                
            }catch(Exception ex)
            {
                return false;
            }
        }
    }
}