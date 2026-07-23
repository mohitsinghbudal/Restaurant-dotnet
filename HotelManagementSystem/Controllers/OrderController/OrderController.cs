using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Interfaces.OrderInterface;
using HotelManagementSystem.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.OrderController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("/orderbyid/{id}")]
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
            int roleId = ClaimHelper.GetRoleId(User);
            if (roleId != 1)
            {
                return Unauthorized("Please login first");
            }

            try
            {
                var order = await _orderService.PlaceOrder(req, userId);
                return Ok(order);
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //    public async Task<IActionResult> CreateOrder([FromQuery] int menuId, [FromQuery] int quantity, [FromBody] Order order)
        //    {

        //        if (menuId <= 0 || quantity <= 0)
        //            return BadRequest("Invalid Menu ID or quantity parameters.");

        //        if (order == null)
        //            return BadRequest("Order payload cannot be null.");

        //        try
        //        {
        //            Console.WriteLine("reached the controller");

        //            var createdOrder = await _orderService.CreateOrderAsync(order, menuId, quantity);

        //            if (createdOrder == null)
        //                return BadRequest("Failed to process order.");

        //            // Returns a 201 Created header pointing to the GetOrderById route
        //            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderId }, createdOrder);
        //        }
        //        catch (InvalidOperationException ex)
        //        {
        //            // This catches the specific "Insufficient kitchen inventory stock" exception thrown by your service layer
        //            return BadRequest(new { message = ex.Message });
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(500, $"Internal server error: {ex.Message}");
        //        }
        //    }

        //    [HttpPut("/api/CompleteOrder")]
        //    public async Task<IActionResult> UpdateOrder([FromQuery] int orderid ,[FromQuery] int waiterId)
        //    {



        //        try
        //        {   
        //            int rowsAffected = await _orderService.UpdateOrderAsync(orderid , waiterId);
        //            if (rowsAffected == 0)
        //                return NotFound($"Active order with ID {orderid} was not found or could not be updated.");

        //            return Ok(new { message = "Order updated successfully.", updatedOrderId = orderid });
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(500, $"Internal server error: {ex.Message}");
        //        }
        //    }

        //    [HttpPut("{id}")]
        //    public async Task<IActionResult> UpdateOrder(
        //int id,
        //[FromQuery] int currentQuantity,
        //[FromQuery] int newQuantity,
        //[FromQuery] int menuId,
        //[FromBody] Order order)
        //    {
        //        if (order == null || id != order.OrderId)
        //            return BadRequest("Order ID mismatch or invalid payload.");

        //        try
        //        {
        //            int rowsAffected = await _orderService.UpdateOrderAsync(order, currentQuantity, newQuantity, menuId);

        //            if (rowsAffected == 0)
        //                return NotFound($"Active order with ID {id} was not found.");

        //            return Ok(new { message = "Order and kitchen stock updated successfully." });
        //        }
        //        catch (InvalidOperationException ex)
        //        {
        //            return BadRequest(new { message = ex.Message });
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(500, $"Internal server error: {ex.Message}");
        //        }
        //    }
    }
}
