using HotelManagementSystem.Models;
using HotelManagementSystem.Models.Order;

namespace HotelManagementSystem.Models.Order
{
    public class OrderItemInputModel
    {
        public int MenuId { get; set; }
        public int Quantity { get; set; }
        public string? Remarks { get; set; }
    }

    public class CreateOrderCompositeRequest
    {
        public Order Order { get; set; } = null!;
        public IEnumerable<OrderItemInputModel> Items { get; set; } = new List<OrderItemInputModel>();
    }

    public class UpdateOrderCompositeRequest
    {
        public Order Order { get; set; } = null!;
        public IEnumerable<OrderItem> UpdatedItems { get; set; } = new List<OrderItem>();
    }
}