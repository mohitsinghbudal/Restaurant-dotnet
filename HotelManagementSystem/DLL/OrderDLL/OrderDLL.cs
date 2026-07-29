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
            string sql = @"SELECT * FROM Orders WHERE DiningSessionId = @DiningSessionId;";
            return await conn.QueryAsync<Order>(sql, new { DiningSessionId = Id });
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
                UPDATE Orders
                SET
                    OrderStatus = @OrderStatus,
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
    }
}