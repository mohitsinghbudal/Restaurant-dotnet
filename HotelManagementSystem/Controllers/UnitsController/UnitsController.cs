using HotelManagementSystem.Interfaces.Units;
using HotelManagementSystem.Models.Units;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.UnitsController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitServices _unitServices;

        public UnitsController(IUnitServices unitServices)
        {
            _unitServices = unitServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUnits()
        {
            if (!User.IsInRole("5"))
                throw new System.Exception("User not allowed");
            var units = await _unitServices.GetAllUnitsAsync();
            return Ok(units);
        }

        [HttpPost]
        public async Task<IActionResult> AddUnit([FromBody] Unit unit)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!User.IsInRole("5"))
                throw new System.Exception("User not allowed");

            var result = await _unitServices.AddUnitAsync(unit);

            if (result > 0)
                return Ok(new { message = "Unit added successfully." });

            return BadRequest(new { message = "Failed to add unit." });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUnit([FromBody] Unit unit)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!User.IsInRole("5"))
                throw new System.Exception("User not allowed");

            var result = await _unitServices.UpdateUnitAsync(unit);

            if (result > 0)
                return Ok(new { message = "Unit updated successfully." });

            return NotFound(new { message = "Unit not found." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            var result = await _unitServices.DeleteUnitAsync(id);

            if (result > 0)
                return Ok(new { message = "Unit deleted successfully." });

            return NotFound(new { message = "Unit not found." });
        }
    }
}