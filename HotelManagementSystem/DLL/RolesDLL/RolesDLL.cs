using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.Roles;
using HotelManagementSystem.Models.Roles;

namespace HotelManagementSystem.DLL.RolesDLL
{
    public class RolesDLL : IRoleDLL
    {
        private readonly IDbConnectionFactory _Idb;
        public RolesDLL(IDbConnectionFactory db)
        {
            _Idb = db;
        }
        public async Task<IEnumerable<Role>> GetRoleAsync()
        {
            using var conn = _Idb.CreateConnection();
            string sql = @"SELECT * FROM Roles";

            return await conn.QueryAsync<Role>(sql);
        }
        public async Task<int> CreateRoleAsync(Role role)
        {
            using var conn = _Idb.CreateConnection();
            string sql = @"INSERT INTO Roles (RoleName,IsActive, Description) 
                           VALUES (@Name,1, @Description);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

            return await conn.ExecuteScalarAsync<int>(sql, role);
        }
        public async Task<bool> UpdateRoleAsync(Role role)
        {
            using var conn = _Idb.CreateConnection();
            string sql = @"UPDATE Roles 
                           SET Name = @Name, 
                               Description = @Description 
                           WHERE RoleId = @RoleId";

            int rowsAffected = await conn.ExecuteAsync(sql, role);
            return rowsAffected > 0;
        }
        public async Task<bool> DeleteRoleAsync(int id)
        {
            using var conn = _Idb.CreateConnection();
            string sql = @"UPDATE Roles SET IsActive = 0 WHERE RoleId = @RoleId";

            int rowsAffected = await conn.ExecuteAsync(sql, new { RoleId = id });
            return rowsAffected > 0;
        }
    }
}
