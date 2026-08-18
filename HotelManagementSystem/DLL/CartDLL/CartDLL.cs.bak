using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HotelManagementSystem.Interfaces;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Models.Cart;
using System.Data;
using System.Data.Common;

namespace HotelManagementSystem.DLL.CartDLL
{
    public class CartDLL : ICartDLL
    {
        private readonly IDbConnectionFactory _dbconn;
        public CartDLL(IDbConnectionFactory dbconn)
        {
            _dbconn = dbconn;
        }

        public async Task<IEnumerable<Cart?>> GetMyCartAsync(int userID)
        {
            using var conn = _dbconn.CreateConnection();
            string sql = @"SELECT * FROM Cart WHERE UserId = @UserId;";

            // Use QueryFirstOrDefaultAsync to return a single Cart object (or null)
            return await conn.QueryAsync<Cart>(sql, new { UserId = userID });
        }
        public async Task<Cart?> GetCartAsync(int userID, int MenuId)
        {
            using var conn = _dbconn.CreateConnection();
            string sql = @"SELECT * FROM Cart WHERE UserId = @UserId AND MenuId = @MenuId;";

            // Use QueryFirstOrDefaultAsync to return a single Cart object (or null)
            return await conn.QueryFirstOrDefaultAsync<Cart>(sql, new { UserId = userID ,MenuId = MenuId});
        }
        public async Task<Cart?> GetMyCartByItemAsync(int cartId)
        {
            using var conn = _dbconn.CreateConnection();
            string sql = @"SELECT * FROM Cart WHERE CartId = @CartId;";

            // Use QueryFirstOrDefaultAsync to return a single Cart object (or null)
            return await conn.QueryFirstOrDefaultAsync<Cart>(sql, new {CartId = cartId });
        }

        public async Task<bool> CreateCartAsync(Cart cart)
        {
            using var conn = _dbconn.CreateConnection();
            string sql = @"INSERT INTO Cart (UserId, MenuId,Quantity, CreatedAt) 
                       VALUES (@UserId, @MenuId , @Quantity,@CreatedAt);";

            int rowsAffected = await conn.ExecuteAsync(sql, cart);
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateCartAsync(Cart cart)
        {
            using var conn = _dbconn.CreateConnection();
            string sql = @"
        UPDATE Cart 
        SET UpdatedAt = @UpdatedAt,
            Quantity = @Quantity
        WHERE CartId = @CartId;";

            int rowsAffected = await conn.ExecuteAsync(sql, cart);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCartAsync(int cartId)
        {
            using var conn = _dbconn.CreateConnection();
            string sql = @"DELETE FROM Cart WHERE CartId = @Id";

            int rowsAffected = await conn.ExecuteAsync(sql, new { Id = cartId });
            return rowsAffected > 0;
        }
    }
}
