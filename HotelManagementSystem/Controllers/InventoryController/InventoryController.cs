using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Interfaces.Inventory;
using HotelManagementSystem.Models.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.InventoryController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        //hotel staff
        // GET: api/Inventory
        [HttpGet]
        public async Task<IActionResult> GetInventoryItems()
        {
            try{var inventoryItems = await _inventoryService.GetInventoryItemsAsync();

                return Ok(new
                {
                    message = "Success",
                    items = inventoryItems
                });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAllInventoryItems([FromQuery] int page)
        {
            
            int roleId = ClaimHelper.GetRoleId(User);

            if (roleId != 5)
                return Unauthorized("user allowed is not allowed");

            try
            {
                if (page < 0) throw new Exception("page can't be less than zero");
                var inventoryItems = await _inventoryService.GetAllInventoryItemsAsync(page);

                return Ok(new
                {
                    message = "Success",
                    items = inventoryItems
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Inventory/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryItemById(int id)
        {
            var inventoryItem = await _inventoryService.GetInventoryItemById(id);

            if (inventoryItem == null)
            {
                return NotFound(new
                {
                    message = "Inventory item not found."
                });
            }

            return Ok(new
            {
                message = "Success",
                item = inventoryItem
            });
        }

        // POST: api/Inventory
        [HttpPost]
        public async Task<IActionResult> AddInventoryItem([FromBody] InventoryItem inventoryItem)
        {
            int userId = ClaimHelper.GetUserId(User);
            int roleId = ClaimHelper.GetRoleId(User);

            if (roleId != 5)
                return Unauthorized("user allowed is not an customer");
            try
            {
                
                var addedItem = await _inventoryService.AddInventoryItem(inventoryItem, userId);

                return Ok(new
                {
                    message = "Inventory item added successfully.",
                    item = addedItem
                });

            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex });
            }
            
        }

        // PUT: api/Inventory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventoryItem( [FromBody] InventoryItem inventoryItem)
        {
            int userId = ClaimHelper.GetUserId(User);   
            int roleId = ClaimHelper.GetRoleId(User);

            if (roleId != 5)
                return Unauthorized("user allowed is not an customer");
            try
            {

                var rowsAffected = await _inventoryService.UpdateInventoryItem(inventoryItem,  userId);

                if (rowsAffected == 0)
                {
                    return NotFound(new
                    {
                        message = "Inventory item not found."
                    });
                }

                return Ok(new
                {
                    message = "Inventory item updated successfully."
                });
            }catch(Exception ex)
            {
                return BadRequest(new {message = ex});
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            try
            {
                int userId = ClaimHelper.GetUserId(User);
                int roleId = ClaimHelper.GetRoleId(User);

                if (roleId != 5)
                    return Unauthorized("user allowed is not an customer");

                await _inventoryService.DeleteInventoryItem(id, userId);
                return Ok(new
                {
                    message = "Inventory item deleted successfully."
                });
            }catch(Exception ex)
            {
                return BadRequest(new { message = ex });
            }

            
        }
    }
}