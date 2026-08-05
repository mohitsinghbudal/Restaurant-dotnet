using DocumentFormat.OpenXml.Spreadsheet;
using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models;
using HotelManagementSystem.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using System.Data;
using System.Security.Claims;

namespace HotelManagementSystem.Controllers.usercontroller
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetUserAsync()
        {
            // Enforce Admin role
            


            try
            {
                if (!User.IsInRole("5")) // Checks if "5" exists in ANY of the user's role claims
                {
                    throw new Exception("User not allowed");
                }

                var users = await _userService.GetUsersAsync();
                if (!users.Any())
                {
                    return NotFound("No users found.");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> updateuserprofile([FromBody] UserModel user)
        {
            if (!User.IsInRole("5")) // Checks if "5" exists in ANY of the user's role claims
            {
                throw new Exception("User not allowed");
            }

            try
            {
                var res = await _userService.UpdateUser(user);
                return Ok(new { message = "success", res = res });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [AllowAnonymous]
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
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO user)
        {
            try
            {
                var loginRes = await _userService.Login(user);

                
                if (loginRes == null)
                {
                    return BadRequest("Error Occured");
                }
                return Ok(new{ Login_token = loginRes.token, message = "login successfull", Roles = loginRes.roles, UserId = loginRes.userId});
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] verifyotp req)
        {


            try
            {

                var verify = await _userService.VerifyOTP(req);
                return Ok(new {message = "successfully verified email" });

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("update-roles")]
        public async Task<IActionResult> UpdateRoles([FromBody] AssignRolesDto model)
        {
            if (!User.IsInRole("5")) // Checks if "5" exists in ANY of the user's role claims
            {
                throw new Exception("User not allowed");
            }

            if (model == null || model.UserId <= 0 || model.RoleIds == null)
            {
                return BadRequest(new { message = "Invalid request payload." });
            }

            int adminUserId = ClaimHelper.GetUserId(User);

            try
            {
                bool isUpdated = await _userService.AssignRolesAsync(model.UserId, model.RoleIds, adminUserId);

                if (isUpdated)
                {
                    return Ok(new { message = "User roles updated successfully." });
                }

                return BadRequest(new { message = "Failed to update user roles." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // TODO: Add logging here (e.g., _logger.LogError(ex, "..."))
                return StatusCode(500, new { message = "An error occurred while updating roles.", error = ex.Message });
            }
        }


    }
    public class verifyotp
    {
        public string Email { get; set; }
        public string otp { get; set; }
    }
}
