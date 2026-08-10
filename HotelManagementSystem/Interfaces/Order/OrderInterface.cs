using HotelManagementSystem.Models;
using HotelManagementSystem.Models.Order;
namespace HotelManagementSystem.Interfaces.OrderInterface
{
    public interface IOrderDLL
    {
        Task<Order> GetOrderByIdAsync(int id);
        //Task<IEnumerable<Order?>> MyOrderAsync(int UserId);
        Task<IEnumerable<Order>> GetOrderBySessionId(int Id);
        Task<bool> UpdateOrderAsync(Order order );
        Task<Order> CreateOrderAsync(Order order);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> UpdateOrderQuantityAsync(int quantity, int orderId);
        Task<bool> UpdateStatus(string status, int OrderId);

        Task<bool> UpdateStatusBySessionId(string status, int sessionId);
    }
    public interface IOrderService
    {
        Task<Order?> GetOrderByIdAsync(int id);
        //Task<IEnumerable<Order?>> MyOrderAsync(int UserId );
        Task<IEnumerable<Order>> GetOrderBySessionId(int Id);
        Task<bool> UpdateOrderAsync(int id, int wid);
        Task<Order?> CreateOrderAsync(CreateOrder order);
        Task<IEnumerable<Order>> PlaceOrder(CreateOrderItems req, int id);
        Task<bool> UpdateOrderAsync(Order order, int currentOrderedQuantity, int newOrderedQuantity, int menuId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> CancelOrderAsync(int orderId, int userId);
        Task<bool> UpdateOrderQuantityAsync(int quantity, int orderId, int menuId);
        Task<bool> UpdateStatus(string status, int OrderId);
        Task<bool> UpdateStatusBySession(string status, int sessionId);

    }
}
