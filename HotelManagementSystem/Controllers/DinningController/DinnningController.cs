using HotelManagementSystem.Models.Dinning;
using HotelManagementSystem.Interfaces.DinningInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace HotelManagementSystem.Controllers.DinningController
{
    [Route("api/[controller]")]
    [ApiController]
    public class DinnningController : ControllerBase
    {
        private readonly IDinningService _dinnning;

        public DinnningController(IDinningService dinning)
        {
            _dinnning = dinning;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDinningSession([FromBody] DinningDTO Dinning)
        {
            if (Dinning.TableId <= 0)
            {
                return BadRequest("Dinning data is null.");
            }
            try
            {
                var sessionId = await _dinnning.CreateDinningAsync(Dinning.TableId);
                return Ok(new { SessionId = sessionId });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                return StatusCode(500,ex.Message);
            }
        }


    }

    
}
