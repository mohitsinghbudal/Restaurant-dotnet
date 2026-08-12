using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.OrderInterface;
using HotelManagementSystem.Models.Order;


namespace HotelManagementSystem.DLL.OrderDLL
{
    public class OrderDLL : IOrderDLL
    {
        private readonly IDbConnectionFactory _dbConn;

        public OrderDLL(IDbConnectionFactory dbConn)
        {
            _dbConn = dbConn;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            using var conn = _dbConn.CreateConnection();
            string sql = @"SELECT * FROM Orders;";

            return await conn.QueryAsync<Order>(sql);
        }
        

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            using var conn = _dbConn.CreateConnection();
            string sql = @"SELECT * FROM Orders WHERE OrderId = @OrderId;";

            return conn.QueryFirstOrDefault<Order>(sql, new { OrderId = id });
        }

        public async Task<IEnumerable<Order>> GetOrderBySessionId(int Id)
        {
            using var conn = _dbConn.CreateConnection();
            string sql = @"SELECT o.* 
FROM Orders o
INNER JOIN DinningSessions ds ON o.DiningSessionId = ds.SessionId
WHERE o.DiningSessionId = @DiningSessionId
  AND ds.SessionStatus = 'Active';";
            return await conn.QueryAsync<Order>(sql, new { DiningSessionId = Id });
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
                UPDATE Orders
                SET
                    OrderStatus = @OrderStatus,
                    IsActive = @IsActive,
                    Description = @Description,
                    UpdatedAt = GETUTCDATE(),
                    UpdatedBy = @UpdatedBy
                WHERE OrderId = @OrderId
                AND IsActive = 1;";

            var result = await conn.ExecuteAsync(sql, order);

            return result > 0;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
        INSERT INTO Orders
        (
            DiningSessionId,
            OrderStatus,
            Description,
            CreatedAt,
            UpdatedAt,
            CompletedAt,
            CreatedBy,
            UpdatedBy,
            IsActive,MenuId,Quantity,UnitPrice,ItemName
        )
        OUTPUT INSERTED.*
        VALUES
        (
            @DiningSessionId,
            @OrderStatus,
            @Description,
            @CreatedAt,
            @UpdatedAt,
            @CompletedAt,
            @CreatedBy,
            @UpdatedBy,
            @IsActive,@MenuId,@Quantity,@UnitPrice,@ItemName
        );";

            return await conn.QueryFirstOrDefaultAsync<Order>(sql, order);
        }

        public async Task<bool> UpdateOrderQuantityAsync(int quantity, int orderId)
        {

            using var conn = _dbConn.CreateConnection();

            string sql = @" UPDATE Orders
                SET
                Quantity = @Quantity
                WHERE OrderId = @OrderId
                AND IsActive = 1; ";
            var order = await conn.ExecuteAsync(sql, new { Quantity = quantity, OrderId = orderId });

            return order > 0;

        }
        public async Task<bool> UpdateStatus(string status, int OrderId)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @" UPDATE Orders
                SET
                OrderStatus = @Status
                WHERE OrderId = @OrderId
                AND IsActive = 1; ";
            var order = await conn.ExecuteAsync(sql, new {Status = status, OrderId = OrderId });

            return order > 0;
        }
        public async Task<bool> UpdateStatusBySessionId(string status,int sessionId)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @" UPDATE Orders
SET
    OrderStatus = 'Completed',
    IsActive = 0,
    UpdatedAt = GETUTCDATE()
WHERE DiningSessionId = @OrderId
  AND IsActive = 1;";
            var order = await conn.ExecuteAsync(sql, new { Status = status, OrderId = sessionId });

            return order > 0;
        }
    }
}