using HotelManagementSystem.Interfaces.Inventory;
using HotelManagementSystem.Interfaces.MenuInterface;
using HotelManagementSystem.Interfaces.OrderInterface;
using HotelManagementSystem.Models.Order;

namespace HotelManagementSystem.Services.OrderService
{
    public class OrderService : IOrderService // Assuming you have an IOrderService interface
    {
        private readonly IOrderDLL _orderDLL;
        //private readonly IOrderItemService _orderItemService;
        private readonly IInventoryService _inventoryService;
        private readonly IMenuDLL _menuDLL;


        public OrderService(IOrderDLL orderDLL, IInventoryService inventoryService , IMenuDLL menuDLL)
        {
            _orderDLL = orderDLL;
            //_orderItemService = orderItemService;
            _inventoryService = inventoryService;
            _menuDLL = menuDLL;
        }

        //get order by id
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Order ID.");

            return await _orderDLL.GetOrderByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrderBySessionId(int Id)
        {
            if (Id <= 0)
                throw new ArgumentException("Invalid Dining Session ID.");
            return await _orderDLL.GetOrderBySessionId(Id);
        }


        

        public async Task<Order?> CreateOrderAsync(CreateOrder order)
        {

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if(order.MenuId <= 0)
                throw new ArgumentException("Invalid Menu ID for order creation.");

            if(order.Quantity <= 0)
                throw new ArgumentException("Ordered quantity must be greater than zero.");

            Console.WriteLine("reached the service");

            Console.WriteLine(order.MenuId);

            //// 1. First check and deduct kitchen stock based on the recipe requirements
            bool stockDeducted = await _inventoryService.DeductInventoryForOrderAsync(order.MenuId, order.Quantity);

            if (!stockDeducted)
            {
                throw new InvalidOperationException("Order placement failed: Insufficient kitchen inventory stock.");
            }

            var newOrder = new Order
            {
                MenuId = order.MenuId,
                ItemName = order.ItemName,
                DiningSessionId = order.DiningSessionId,
                Description = order.Description,
                CreatedBy = order.CreatedBy,
                Quantity = order.Quantity,
                CreatedAt = DateTime.UtcNow,
                OrderStatus = "Pending",
                IsActive = true,
                UnitPrice = await _menuDLL.GetPriceById(order.MenuId)
                
            };

         var createdOrder = await _orderDLL.CreateOrderAsync(newOrder);


            return createdOrder;

        }

        public async Task<IEnumerable<Order>> PlaceOrder(CreateOrderItems req, int createdBy)
        {
            if (req == null)
                throw new ArgumentNullException(nameof(req));

            if (req.Items == null || !req.Items.Any())
                throw new ArgumentException("No order items found.");

            List<Order> orders = new();

            foreach (var item in req.Items)
            {
                item.CreatedBy = createdBy;
                item.CreatedAt = DateTime.UtcNow;
                item.IsActive = true;

                var createdOrder = await CreateOrderAsync(item);

                if (createdOrder == null)
                    throw new Exception($"Failed to create order for MenuId {item.MenuId}");

                orders.Add(createdOrder);
            }

            return orders;
        }

        public async Task<int> UpdateOrderAsync(int id, int wid)
        {
            var order = await _orderDLL.GetOrderByIdAsync(id);

            if (order.OrderStatus == "Completed")
            {
                throw new Exception("already completed order");
            }

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderId <= 0)
                throw new ArgumentException("Invalid Order ID for update operation.");

            order.OrderStatus = "Completed";
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = wid;
            return await _orderDLL.UpdateOrderAsync(order);

        }
        public async Task<int> UpdateOrderAsync(Order order, int currentOrderedQuantity, int newOrderedQuantity, int menuId)
        {
            if (order == null || order.OrderId <= 0)
                throw new ArgumentException("Invalid order data.");

            // 1. Calculate the difference (Delta)
            // If positive: user added items (need to deduct more inventory)
            // If negative: user removed items (need to return inventory back to kitchen)
            int quantityDifference = newOrderedQuantity - currentOrderedQuantity;

            if (quantityDifference != 0)
            {
                // 2. Call inventory service with the difference
                // We will adjust our inventory service to handle negative numbers as "reversing/adding back" stock
                bool stockAdjusted = await _inventoryService.DeductInventoryForOrderAsync(menuId, quantityDifference);

                if (!stockAdjusted)
                {
                    throw new InvalidOperationException("Mofidication failed: Insufficient kitchen stock for the additional items.");
                }
            }

            // 3. Forward the updated metadata to the DLL
            return await _orderDLL.UpdateOrderAsync(order);
        }
    }
}