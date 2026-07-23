using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Models.Dinning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HotelManagementSystem.Controllers.DinningController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DinningController : ControllerBase // Fixed 'Dinnning' spelling typo
    {
        private readonly IDinningService _dinningService;

        public DinningController(IDinningService dinningService)
        {
            _dinningService = dinningService;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetSessionByUserId()
        {
            int userId = ClaimHelper.GetUserId(User);
            int roleId = ClaimHelper.GetRoleId(User);

            if (roleId != 1)
            {
                return Unauthorized("Please login first");
            }
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "Invalid User ID provided." });
                }

                int sessionId = await _dinningService.GetDiningSession(userId);

                if (sessionId <= 0)
                {
                    return NotFound(new { message = $"No active dining session found for User ID {userId}." });
                }

                return Ok(new { sessionId = sessionId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the dining session.", details = ex.Message });
            }
        }

        [HttpPost("Create-Session")]
        public async Task<IActionResult> CreateDinningSession([FromBody] DinningDTO dto)
        {
            int userId = ClaimHelper.GetUserId(User);

            if (dto == null || dto.TableId <= 0)
            {
                return BadRequest(new { message = "Invalid data payload. A valid Table ID is required." });
            }

            try
            {
                var sessionId = await _dinningService.CreateDinningAsync(dto.TableId, userId);
                return Ok(new { sessionId = sessionId, message = "Dining session initiated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal server error occurred.", details = ex.Message });
            }
        }

        [HttpPost("End-Session/{sessionId}")]
        public async Task<IActionResult> EndDinningSession(int sessionId)
        {
            if (sessionId <= 0)
            {
                return BadRequest(new { message = "A valid Session ID must be provided." });
            }

            try
            {
                var rowsAffected = await _dinningService.EndDinningSessionAsync(sessionId);

                if (rowsAffected <= 0)
                {
                    return BadRequest(new { message = "Failed to close the session. It may already be closed or does not exist." });
                }

                return Ok(new { message = "Dining session closed successfully. Table has transitioned to cleaning status." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal server error occurred.", details = ex.Message });
            }
        }
    }
}