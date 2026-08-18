using HotelManagementSystem.Interfaces.Inventory;
using HotelManagementSystem.Interfaces.MenuInterface;
using HotelManagementSystem.Interfaces.OrderInterface;
using HotelManagementSystem.Models.MenuItems;
using HotelManagementSystem.Models.Order;

namespace HotelManagementSystem.Services.OrderService
{
    public class OrderService : IOrderService 
    {
        private readonly IOrderDLL _orderDLL;
        
        private readonly IInventoryService _inventoryService;
        private readonly IMenuDLL _menuDLL;
        private readonly IMenuServices _menuServices;


        public OrderService(IOrderDLL orderDLL, IInventoryService inventoryService , IMenuDLL menuDLL, IMenuServices menuServices)
        {
            _orderDLL = orderDLL;
            
            _inventoryService = inventoryService;
            _menuDLL = menuDLL;
            _menuServices = menuServices;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderDLL.GetAllOrdersAsync();
        }

        
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

        public async Task<bool> UpdateOrderAsync(int id, int wid)
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
        public async Task<bool> UpdateOrderAsync(Order order, int currentOrderedQuantity, int newOrderedQuantity, int menuId)
        {
            if (order == null || order.OrderId <= 0)
                throw new ArgumentException("Invalid order data.");

            
            
            
            int quantityDifference = newOrderedQuantity - currentOrderedQuantity;

            if (quantityDifference != 0)
            {
                
                
                bool stockAdjusted = await _inventoryService.DeductInventoryForOrderAsync(menuId, quantityDifference);

                if (!stockAdjusted)
                {
                    throw new InvalidOperationException("Mofidication failed: Insufficient kitchen stock for the additional items.");
                }
            }

            
            return await _orderDLL.UpdateOrderAsync(order);
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            try
            {
                var existingOrder = await _orderDLL.GetOrderByIdAsync(orderId);

                if (existingOrder == null) throw new Exception("invalid order");

                if(DateTime.UtcNow - existingOrder.CreatedAt > TimeSpan.FromMinutes(5) && existingOrder.OrderStatus =="Preparing") throw new Exception("can't be deleted its already preparing");

                existingOrder.OrderStatus = "Cancelled";
                existingOrder.UpdatedBy = userId;
                existingOrder.UpdatedAt = DateTime.UtcNow;
                existingOrder.IsActive = false;

                return await _orderDLL.UpdateOrderAsync(existingOrder);

            }catch(Exception ex)
            {
                throw new Exception("please enter valid order");
            }
            
        }
        public async Task<bool> UpdateOrderQuantityAsync(int quantity, int orderId, int menuId)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            var menuitem = await _menuServices.GetMenuItemByIdAsync(menuId);

            var order = await _orderDLL.GetOrderByIdAsync(orderId);

            if (order.OrderStatus == "Ready" || order.OrderStatus == "Completed"|| order.OrderStatus == "Cancelled") throw new Exception("order is already prepared create new order");

            if (menuitem == null)
                throw new Exception("Menu item not found.");

            if (!menuitem.IsAvailable)
                throw new Exception("Menu item is not available.");

            var menu = (await _menuServices.GetAllMenuItemsAsync())
                    .FirstOrDefault(m => m.MenuId == menuId);

            Console.WriteLine(menu);

            if (menu == null)
                throw new Exception("Menu item not found.");

            if (!menu.IsAvailable)
                throw new Exception("Menu item is not available.");

            if (menu.AvailablePortions < quantity)
                throw new Exception($"Only {menu.AvailablePortions} portion(s) are available.");




            return await _orderDLL.UpdateOrderQuantityAsync(quantity, orderId);
        }

        public async Task<bool> UpdateStatus(string status, int orderId)
        {
            var order = await _orderDLL.GetOrderByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order does not exist.");

            if (order.OrderStatus == "Completed" || order.OrderStatus == "Cancelled")
                throw new InvalidOperationException("Cannot update status for an order that is already completed or cancelled.");

            return await _orderDLL.UpdateStatus(status, orderId);
        }
        public async Task<bool> UpdateStatusBySession(string status, int sessionId)
        {
            var order = await _orderDLL.GetOrderBySessionId(sessionId);

            foreach(var or in order){

         

                if (or.OrderStatus != "Completed" && or.OrderStatus != "Cancelled")
                   { 
                    
                    await _orderDLL.UpdateStatus(status, sessionId);

                }


            }
            return true;


        }
    }
}
