using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Interfaces.MenuInterface;
using HotelManagementSystem.Models.MenuItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.MenuController
{

    [Authorize]
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
            
            try    

            {
                var menus = await _menuService.GetAllMenuItemsAsync();

                return Ok(new
                {
                    message = "Success",
                    items = menus
                });
            }catch(Exception ex)
            {
                return BadRequest(ex);
            }
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
            try{var createdMenu = await _menuService.CreateMenuItemAsync(menu);

                return Ok(new
                {
                    message = "Menu item created successfully",
                    item = createdMenu
                });
            }catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }


        // PUT: api/Menu/5
        [HttpPut]
        public async Task<IActionResult> UpdateMenuItem(
            [FromBody] UpdateMenu menu)
        {

            int userId = ClaimHelper.GetUserId(User);

            if (!User.IsInRole("5")) 
            {
                throw new Exception("User not allowed");
            }
            
            if (menu == null)
            {
                return BadRequest("Enter the valid menu Item");
            }

            // Ensure the MenuId expected by the SQL is provided from the route

           try{
                var result = await _menuService.UpdateMenuAsync(menu , userId);

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
            catch(Exception ex)
            {
                return BadRequest("server errro");
            }

            
        }
    }
}