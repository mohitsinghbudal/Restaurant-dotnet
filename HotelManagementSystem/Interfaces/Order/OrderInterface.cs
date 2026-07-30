using HotelManagementSystem.Models;
using HotelManagementSystem.Models.Order;
namespace HotelManagementSystem.Interfaces.OrderInterface
{
    public interface IOrderDLL
    {
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrderBySessionId(int Id);
        Task<bool> UpdateOrderAsync(Order order );
        Task<Order> CreateOrderAsync(Order order);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> UpdateOrderQuantityAsync(int quantity, int orderId);

    }
    public interface IOrderService
    {
        Task<Order?> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrderBySessionId(int Id);
        Task<bool> UpdateOrderAsync(int id, int wid);
        Task<Order?> CreateOrderAsync(CreateOrder order);
        Task<IEnumerable<Order>> PlaceOrder(CreateOrderItems req, int id);
        Task<bool> UpdateOrderAsync(Order order, int currentOrderedQuantity, int newOrderedQuantity, int menuId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> CancelOrderAsync(int orderId, int userId);
        Task<bool> UpdateOrderQuantityAsync(int quantity, int orderId, int menuId);

    }
}
