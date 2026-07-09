using HotelManagementSystem.Models;
using HotelManagementSystem.Models.Order;
namespace HotelManagementSystem.Interfaces.OrderInterface
{
    public interface IOrderDLL
    {
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrderBySessionId(int Id);
        Task<int> UpdateOrderAsync(Order order);
        Task<Order> CreateOrderAsync(Order order);

    }
    public interface IOrderService
    {
        Task<Order?> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrderBySessionId(int Id);
        Task<int> UpdateOrderAsync(int id, int wid);
        Task<Order?> CreateOrderAsync(CreateOrder order);

        Task<int> UpdateOrderAsync(Order order, int currentOrderedQuantity, int newOrderedQuantity, int menuId);
        

    }
}
