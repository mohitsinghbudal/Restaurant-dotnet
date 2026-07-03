using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace HotelManagementSystem.Controllers.usercontroller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> GetUserAsync()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                if (!users.Any()) { 
                    return NotFound("No users found.");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO user)
        {
            try
            {
             
                var success = await _userService.SignUp(user);

                if (success <= 0)
                {
                    return BadRequest("failed to create user");
                }
                return Ok("user created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO user)
        {
            try
            {
                string token = await _userService.Login(user);
                if (token==null)
                {
                    return BadRequest("Error Occured");
                }
                return Ok(new{ Login_token = token, message = "login successfull"});
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
