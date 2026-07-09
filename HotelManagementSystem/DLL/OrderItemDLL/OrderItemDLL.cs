using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.OrderItemInterface;

//using HotelManagementSystem.Interfaces.OrderItemInterface;
using HotelManagementSystem.Models;
using System.Data;

namespace HotelManagementSystem.DLL.OrderItemDLL
{
    public class OrderItemDLL : IOrderItemDLL
    {
        private readonly IDbConnectionFactory _dbconn;
        public OrderItemDLL(IDbConnectionFactory dbconn)
        {
            _dbconn = dbconn;
        }

        public async Task<OrderItem> CreateOrderItemAsync(OrderItem orderItem)
        {
            using var connection = _dbconn.CreateConnection();
            string sql = @"
                INSERT INTO OrderItems 
                (
                    OrderId, MenuId, Quantity, UnitPrice, Remarks, 
                    ItemStatus, IsActive, CreatedBy, CreatedAt
                )
                VALUES 
                (
                    @OrderId, @MenuId, @Quantity, @UnitPrice, @Remarks, 
                    @ItemStatus, @IsActive, @CreatedBy, SYSUTCDATETIME()
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";
            int newId = await connection.QuerySingleAsync<int>(sql, orderItem);
            orderItem.OrderItemId = newId;
            return orderItem;
        }

        
        public async Task<bool> SaveOrderItemsAsync(IEnumerable<OrderItem> items)
        {
            using var connection = _dbconn.CreateConnection();
            string sql = @"
                INSERT INTO OrderItems 
                (
                    OrderId, MenuId, Quantity, UnitPrice, Remarks, 
                    ItemStatus, IsActive, CreatedBy, CreatedAt
                )
                VALUES 
                (
                    @OrderId, @MenuId, @Quantity, @UnitPrice, @Remarks, 
                    @ItemStatus, @IsActive, @CreatedBy, SYSUTCDATETIME()
                );";

            // Dapper automatically loops over the collection and executes efficiently in bulk
            int rowsAffected = await connection.ExecuteAsync(sql, items);
            return rowsAffected == items.Count();
        }

        public async Task<IEnumerable<OrderItem>> GetItemsByOrderIdAsync(int orderId)
        {
            using var conn = _dbconn.CreateConnection();
            string sql = @"SELECT * FROM OrderItems WHERE OrderId = @OrderId AND IsActive = 1;";
            return await conn.QueryAsync<OrderItem>(sql, new { OrderId = orderId });
        }


        public async Task<int> UpdateOrderItemStatusAsync(int orderItemId, string status, int userId)
        {
            using var conn = _dbconn.CreateConnection();

            string sql = @"
                UPDATE OrderItems
                SET 
                    ItemStatus = @ItemStatus,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = SYSUTCDATETIME()
                WHERE 
                    OrderItemId = @OrderItemId;";

            return await conn.ExecuteAsync(sql, new
            {
                ItemStatus = status,
                UpdatedBy = userId,
                OrderItemId = orderItemId
            });
        }
    }
}
    

