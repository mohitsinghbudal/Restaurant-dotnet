using HotelManagementSystem.Interfaces.Roles;
using HotelManagementSystem.Models.Roles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace HotelManagementSystem.Controllers.RolesController
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService) {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {

            try
            {
                var result = await _roleService.GetRoleAsync();
                return Ok(new { result = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Role role) {
            try
            {
                var result = await _roleService.CreateRoleAsync(role);
                return Ok(new { result = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Role role)
        {
            try
            {
                var result = await _roleService.UpdateRoleAsync(role);
                return Ok(new { result = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] Role role) {
            try
            {
                var result = await _roleService.DeleteRoleAsync(role.RoleId);
                return Ok(new { result = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
    }
}
