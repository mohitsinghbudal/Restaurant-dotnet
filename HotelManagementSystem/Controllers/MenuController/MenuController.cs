using HotelManagementSystem.Interfaces.MenuInterface;
using HotelManagementSystem.Models.MenuItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.MenuController
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuServices _menuService;

        public MenuController(IMenuServices menuService)
        {
            _menuService = menuService;
        }


        // GET: api/Menu
        //[AllowAnonymous]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllMenuItems()
        {
            var menus = await _menuService.GetAllMenuItemsAsync();

            return Ok(new
            {
                message = "Success",
                items = menus
            });
        }


        // GET: api/Menu/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuItemById(int id)
        {
            var menu = await _menuService.GetMenuItemByIdAsync(id);

            if (menu == null)
            {
                return NotFound(new
                {
                    message = "Menu item not found"
                });
            }

            return Ok(new
            {
                message = "Success",
                item = menu
            });
        }


        // POST: api/Menu
        [HttpPost]
        public async Task<IActionResult> CreateMenuItem(
            [FromBody] CreateMenu menu)
        {
            var createdMenu = await _menuService.CreateMenuItemAsync(menu);

            return Ok(new
            {
                message = "Menu item created successfully",
                item = createdMenu
            });
        }


        // PUT: api/Menu/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(
            int id,
            [FromBody] UpdateMenu menu)
        {
            if (menu == null)
            {
                return BadRequest();
            }

            // Ensure the MenuId expected by the SQL is provided from the route
            menu.MenuId = id;

            var result = await _menuService.UpdateMenuAsync(menu);

            if (result == 0)
            {
                return NotFound(new
                {
                    message = "Menu item not found"
                });
            }

            return Ok(new
            {
                message = "Menu item updated successfully"
            });
        }
    }
}