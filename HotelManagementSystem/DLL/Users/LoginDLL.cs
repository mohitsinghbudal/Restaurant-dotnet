using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Models.User;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace HotelManagementSystem.DLL.Users
{
    public class LoginDLL
    {
        private readonly IDbConnectionFactory _dbconn;
        public LoginDLL(IDbConnectionFactory dbconn)  
        {
            _dbconn = dbconn;
        }
        public async Task<int> SignUp(UserModel user)
        {
            var conn = _dbconn.CreateConnection();
            string sql = @"
                INSERT INTO Users
                (
                    Name,PhoneNo,Email, PasswordHash,RoleId,IsActive,CreatedAt,UpdatedAt
                )
                VALUES
                (
                    @Name,@PhoneNo,@Email, @PasswordHash,@RoleId,@IsActive,@CreatedAt,@UpdatedAt
                )";
            return await conn.ExecuteAsync(sql,user);
        }

    }
}
