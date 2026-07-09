using System.Data;
using HotelManagementSystem.Models;
using HotelManagementSystem.Models.Order;

namespace HotelManagementSystem.Interfaces.OrderItemInterface
{
    public interface IOrderItemDLL
    {
        Task<OrderItem> CreateOrderItemAsync(OrderItem orderItem);
        Task<bool> SaveOrderItemsAsync(IEnumerable<OrderItem> items);
        Task<IEnumerable<OrderItem>> GetItemsByOrderIdAsync(int orderId);
        Task<int> UpdateOrderItemStatusAsync(int orderItemId, string status, int userId);
    }

    public interface IOrderItemService
    {
        Task<Order?> GetOrderByIdAsync(int id);
        Task<int> CompleteOrderAsync(int id, int waiterId);
        Task<Order?> CreateOrderAsync(Order order, IEnumerable<OrderItemInputModel> inputItems);
        Task<int> UpdateOrderAsync(Order order, IEnumerable<OrderItem> updatedItems);
    }
}