using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.User;
using System.Data;
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

        public async Task<IEnumerable<UserModel>> GetAllUserAsync()
        {
            using var connection = _dbConnection.CreateConnection();

            const string sql = @"
                SELECT
    UserId,
    FirstName,
    MiddleName,
    LastName,
    Email,
    PhoneNo,
    IsActive,
    CreatedAt,
    UpdatedAt,
    EmailOtp,
    OtpExpiry,
    IsEmailVerified    
FROM Users";

            return await connection.QueryAsync<UserModel>(
                sql);
        }
        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            using var connection = _dbConnection.CreateConnection();

            const string sql = @"
                SELECT
    *
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
(FirstName,MiddleName,LastName,PhoneNo,Email,PasswordHash,IsActive,CreatedAt
)
VALUES
(@FirstName,@MiddleName,@LastName,@PhoneNo,@Email,@PasswordHash,@IsActive,@CreatedAt
)";

            return await connection.ExecuteAsync(sql, user);
        }

        public async Task<int> AssignWaiterAsync()
        {
            using var conn = _dbConnection.CreateConnection();

            var sql = @"
        SELECT TOP 1
            u.UserId
        FROM Users u
        INNER JOIN UserRoles ur
            ON u.UserId = ur.UserId
        LEFT JOIN Tables t
            ON u.UserId = t.WaiterId
        LEFT JOIN DinningSessions d
            ON d.TableId = t.TableId
            AND d.SessionStatus <> 'Completed'
            AND d.UpdatedAt > DATEADD(HOUR, -6, GETUTCDATE())
        WHERE
            ur.RoleId = @RoleId
            AND ur.IsDeleted = 0
            AND u.IsActive = 1
        GROUP BY
            u.UserId
        ORDER BY
            COUNT(d.SessionId) ASC,
            u.UserId ASC;";

            const int WaiterRoleId = 2;

            int? waiterId = await conn.QueryFirstOrDefaultAsync<int?>(
                sql,
                new { RoleId = WaiterRoleId });

            if (!waiterId.HasValue)
                throw new InvalidOperationException("No active waiters found in the system.");

            return waiterId.Value;
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

        public async Task<List<int>> GetUserRoleIdsAsync(int userId)
        {
            using var conn = _dbConnection.CreateConnection();

            // Fixed SQL string formatting with proper spacing
            string sql = @"
        SELECT RoleId 
        FROM UserRoles 
        WHERE UserId = @UserId 
          AND IsDeleted = 0;";

            var roles = await conn.QueryAsync<int>(sql, new { UserId = userId });

            return roles.ToList();
        }

        public async Task AddOrReactivateRolesAsync(
    int userId,
    IEnumerable<int> roleIds,
    int assignedBy)
        {
            using var conn = _dbConnection.CreateConnection();

            foreach (var roleId in roleIds.Distinct())
            {
                string sql = @"
            IF EXISTS
            (
                SELECT 1
                FROM UserRoles
                WHERE UserId = @UserId
                  AND RoleId = @RoleId
            )
            BEGIN

                UPDATE UserRoles
                SET
                    IsDeleted = 0,
                    AssignedAt = @AssignedAt,
                    AssignedBy = @AssignedBy
                WHERE UserId = @UserId
                  AND RoleId = @RoleId;

            END
            ELSE
            BEGIN

                INSERT INTO UserRoles
                (
                    UserId,
                    RoleId,
                    AssignedAt,
                    AssignedBy,
                    IsDeleted
                )
                VALUES
                (
                    @UserId,
                    @RoleId,
                    @AssignedAt,
                    @AssignedBy,
                    0
                );

            END";

                await conn.ExecuteAsync(sql, new
                {
                    UserId = userId,
                    RoleId = roleId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = assignedBy
                });
            }
        }

        public async Task SoftDeleteRolesAsync(int userId, IEnumerable<int> roleIds, int deletedBy)
        {
            using var conn = _dbConnection.CreateConnection();

            string sql = @"
        UPDATE UserRoles
        SET
            IsDeleted = 1,
            DeletedAt = @DeletedAt,
            DeletedBy = @DeletedBy
        WHERE UserId = @UserId
          AND RoleId IN @RoleIds
          AND IsDeleted = 0;";

            await conn.ExecuteAsync(sql, new
            {
                UserId = userId,
                RoleIds = roleIds,
                DeletedAt = DateTime.UtcNow,
                DeletedBy = deletedBy
            });
        }
        
        public async Task AddRolesAsync(
    int userId,
    IEnumerable<int> roleIds,
    int assignedBy)
{
    using var conn = _dbConnection.CreateConnection();

    string sql = @"
        INSERT INTO UserRoles
        (
            UserId,
            RoleId,
            AssignedAt,
            AssignedBy,
            IsDeleted
        )
        VALUES
        (
            @UserId,
            @RoleId,
            @AssignedAt,
            @AssignedBy,
            0
        );";

    var parameters = roleIds.Select(roleId => new
    {
        UserId = userId,
        RoleId = roleId,
        AssignedAt = DateTime.UtcNow,
        AssignedBy = assignedBy
    });

    await conn.ExecuteAsync(sql, parameters);
}
        public async Task<bool> UpdateUser(UserModel user)
        {
            using var conn = _dbConnection.CreateConnection();

            string sql = @"
        UPDATE [Users]
        SET 
            FirstName = @FirstName,
            MiddleName = @MiddleName,
            LastName = @LastName,
            Email = @Email,
            PhoneNo = @PhoneNo,
            IsActive = @IsActive,
            IsEmailVerified = @IsEmailVerified,
            UpdatedAt = GETUTCDATE()
        WHERE UserId = @UserId;";

            int rowsAffected = await conn.ExecuteAsync(sql, new
            {
                user.UserId,
                user.FirstName,
                user.MiddleName,
                user.LastName,
                user.Email,
                user.PhoneNo,
                user.IsActive,
                user.IsEmailVerified
            });

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds, int assignedBy)
        {
            using var conn = _dbConnection.CreateConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            using var transaction = conn.BeginTransaction();

            try
            {
                // 1. Delete current role mappings
                string deleteSql = "DELETE FROM UserRoles WHERE UserId = @UserId;";
                await conn.ExecuteAsync(deleteSql, new { UserId = userId }, transaction);

                // 2. Insert new role mappings
                if (roleIds != null && roleIds.Any())
                {
                    string insertSql = @"
                    INSERT INTO UserRoles (UserId, RoleId, AssignedAt, AssignedBy)
                    VALUES (@UserId, @RoleId, GETDATE(), @AssignedBy);";

                    var roleParameters = roleIds.Distinct().Select(roleId => new
                    {
                        UserId = userId,
                        RoleId = roleId,
                        AssignedBy = assignedBy
                    });

                    await conn.ExecuteAsync(insertSql, roleParameters, transaction);
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