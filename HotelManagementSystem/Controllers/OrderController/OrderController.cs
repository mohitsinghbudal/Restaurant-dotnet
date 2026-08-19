using DocumentFormat.OpenXml.Spreadsheet;
using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Hubs;
using HotelManagementSystem.Interfaces.OrderInterface;
using HotelManagementSystem.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HotelManagementSystem.Controllers.OrderController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderController(IOrderService orderService, IHubContext<OrderHub> hubContext)
        {
            _orderService = orderService;
            _hubContext = hubContext;
        }
        private async Task BroadcastUpdatedOrdersAsync()
        {
            var latestOrders = await _orderService.GetAllOrdersAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveAllOrders", latestOrders);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrdersAsync()
        {
            
            try
            {
                if (!User.IsInRole("5") && !User.IsInRole("4"))
                {
                    throw new Exception("User not allowed");
                }
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(new { message = "success", orders = orders });
            }
            catch (Exception ex)
            {
               return BadRequest(new { message = "server error  " +ex.Message });
            }
        }
        [HttpGet("my-orders")]
        public async Task<IActionResult> MyOrdersAsync(int SessionId)
        {
            var roles = User.FindAll(ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
            if (!User.IsInRole("1")&&!User.IsInRole("2"))
            {
                throw new Exception("User not allowed");
            }
            try
            {
                var result = await _orderService.GetOrderBySessionId(SessionId);
                return Ok(new { message = result });
            }
            catch (Exception ex) {
                return BadRequest(new { message = "server error" + ex.Message });
            }
        }

        [HttpGet("orderbyid/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Order ID provided.");

            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound($"Order with ID {id} was not found.");

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

       [HttpGet("sessionId/{id}")]
       public async Task<IActionResult> GetOrderBySessionId(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Order ID provided.");

            }
            try
            {
                var order = await _orderService.GetOrderBySessionId(id);
                if (order == null)
                    return NotFound($"Order with ID {id} was not found.");

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


        }
        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody ] CreateOrder newOrder)
        {
            try
            {
                
                var createOrder = await _orderService.CreateOrderAsync(newOrder);
                if (createOrder == null)
                    throw new Exception("Server Error");

                //broad cast
                await BroadcastUpdatedOrdersAsync();

                return Ok(new { NewOrder = createOrder });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderItems req)
        {
            int userId = ClaimHelper.GetUserId(User);
            if (!User.IsInRole("1")&&!User.IsInRole("5") &&!User.IsInRole("2"))
                
            {
                return Unauthorized("Please login first");
            }

            try
            {
                var order = await _orderService.PlaceOrder(req, userId);
                //broad cast
                await BroadcastUpdatedOrdersAsync();
                return Ok(order);
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        

            [HttpPut("cancel")]
            public async Task<IActionResult> CancelOrderAsync([FromQuery] int OrderId )
            {

                int userId = ClaimHelper.GetUserId(User);
            
                if (!User.IsInRole("5")&&!User.IsInRole("1")&&!User.IsInRole("4"))
                    throw new Exception("User not allowed");

                try
                {   
                    if (OrderId <= 0) throw new Exception("Please enter the order values");

                    bool cancelorder = await _orderService.CancelOrderAsync(OrderId, userId);
                    
                //broad cast
                await BroadcastUpdatedOrdersAsync();
                
                return Ok(new { message = "sucessfull" });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "server error" + ex});
                }
            }

        [HttpPut("updateQuantity")]
        public async Task<IActionResult> UpdateOrderQuantity([FromQuery] int itemQuantity, [FromQuery] int orderId ,  [FromQuery] int menuId)
        {
            int userId = ClaimHelper.GetUserId(User);
            if (!User.IsInRole("5")&& !User.IsInRole("2"))
                throw new Exception("User not allowed");

            try
            {
                bool updateOrder = await _orderService.UpdateOrderQuantityAsync(itemQuantity,orderId, menuId);

                //broad cast
                await BroadcastUpdatedOrdersAsync();

                return Ok(new { message = "sucessfull" });
            }
            catch (Exception ex)
            {
                return BadRequest("server error" + ex );
            }
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] string status, [FromQuery] int OrderId)
        {
            try
            {
                if (!User.IsInRole("4"))
                    throw new Exception("user is not allowed");

                var result = await _orderService.UpdateStatus(status , OrderId);

                //broad cast
                await BroadcastUpdatedOrdersAsync();

                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        

        
        

        
        

        
        
        

        

        
        

        
        
        
        
        
        
        
        
        
        
        
        
        

        
        
        



        
        
        
        
        

        
        
        
        
        
        
        

        
        
        
        
        
        
        
        
        
        

        
        
        

        
        

        
        
        
        
        
        
        
        
        
        
        
    }
}

