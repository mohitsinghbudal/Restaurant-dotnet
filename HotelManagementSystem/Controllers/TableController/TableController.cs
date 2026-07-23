using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Models.Table;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace HotelManagementSystem.Controllers.TableController
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;
        


        public TableController(ITableService tableService)
        {
            _tableService = tableService;

        }


        [HttpGet("get-all-table")]
        public async Task<IActionResult> GetAllTable()
        {

            var tables = _tableService.GetAllTable();
            return Ok(new { message = "this is tables info", alltables = tables });
        }


        [HttpGet("my-active-bookings")]
        public async Task<IActionResult> getmybookings()
        {
            int userId = ClaimHelper.GetUserId(User);
            int roleId = ClaimHelper.GetRoleId(User);
            if (roleId != 1)
            {
                return Unauthorized("Please login first");
            }
            try
            {
                var items = await _tableService.GetMyBookings(userId);
                return Ok(new { bookings = items });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-all-bookings")]
        public async Task<IActionResult> getallbookings()
        {
            int userId = ClaimHelper.GetUserId(User);
            int roleId = ClaimHelper.GetRoleId(User);

            if (roleId != 1)
            {
                return Unauthorized("Please login first");
            }

            try
            {
                var items = await _tableService.GetMyAllBookings(userId);
                return Ok(new { bookings = items });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-single-table-info")]
        public async Task<IActionResult> SeeTableInfo([FromQuery] int tableNo)
        {
            var table = await _tableService.SeeTableInfo(tableNo);

            if (table == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Table not found."
                });
            }

            return Ok(new
            {
                success = true,
                tableData = table
            });
        }

        [HttpPost("CreateTable")]
        public async Task<IActionResult> CreateTableAsync([FromBody] CreateTable table)
        {
            if (table == null) return BadRequest(new { message = "Invalid table data payload." });

            try
            {
                var createdTable = await _tableService.CreateTableAsync(table);

                if (createdTable == null)
                {
                    return BadRequest(new { message = "Table creation failed. Table number may already exist." });
                }
                return Ok(new { table = createdTable, message = "Table created successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("book-table")]
        public async Task<IActionResult> BookTable([FromBody] int tableNo)
        {

            int userId = ClaimHelper.GetUserId(User);
            int roleId = ClaimHelper.GetRoleId(User);

            if (roleId != 1)
                    return Unauthorized("user allowed is not an customer");


            if (tableNo <=0) return BadRequest(new { message = "Invalid update payload." });


            try
            {
                var result = await _tableService.BookTableAsync(tableNo , userId);

                if (result <= 0)
                {
                    return BadRequest(new { message = "Error encountered while attempting to book the table." });
                }

                return Ok(new { sessionid = result, message = "Table successfully booked and waiter assigned!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("clean-table")]
        public async Task<IActionResult> CleanTable([FromBody] CleanTable table)
        {

            if (table == null) return BadRequest(new { message = "Invalid update payload." });

            //int userId = ClaimHelper.GetUserId(User);
            //int roleId = ClaimHelper.GetRoleId(User);

            

            //if (roleId != 2)
            //    return Unauthorized("user not allowed is not an waiter")


            try
            {
                var result = await _tableService.CleanTableAsync(table);
                if (result <= 0)
                {
                    return BadRequest(new { message = "Error encountered while attempting to clear the cleaning status." });
                }

                return Ok(new { affectedRows = result, message = "Table cleaning complete. Table is now available!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("free-table")]
        public async Task<IActionResult> FreeTable([FromBody] UpdateTable table)
        {
            if (table == null) return BadRequest(new { message = "Invalid update payload." });

            try
            {
                var result = await _tableService.FreeTableAsync(table);
                if (result <= 0)
                {
                    return BadRequest(new { message = "Error encountered while attempting to free the table." });
                }

                // Fixed success messaging path to cleanly indicate transition to Cleaning status
                return Ok(new { affectedRows = result, message = "Table status set to Available!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        

        [HttpGet("qrcode/{tableNo}")]
        public IActionResult GetQRCode(int tableNo)
        {
            

            byte[] imageBytes = _tableService.GenerateTableQRCode(tableNo);

            return File(imageBytes, "image/png");
        }
    }
}