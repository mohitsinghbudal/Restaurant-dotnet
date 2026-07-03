using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Models.Table;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.TableController
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTableAsync([FromBody] TableModel table)
        {
            try
            {
                var createtable = await _tableService.CreateTableAsync(table);

                if (createtable == null)
                {
                    return BadRequest("Table creation failed.");
                }
                return Ok(new { table = createtable, message = "Table created successfully." });
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("book-table")]
        public async Task<IActionResult> BookTable([FromBody] UpdateTable table)
        {
            try
            {
                var update = await _tableService.BookTableAsync(table);

                if(update<=0)
                {
                    return BadRequest(new { message = "error booking the table" });
                }

                return Ok(new {update = update ,message = "table booked successfully !!!" });

            } catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("free-table")]
        public async Task<IActionResult> FreeTable([FromBody] UpdateTable table)
        {
            try
            {

                var update = await _tableService.FreeTableAsync(table);
                if (update <= 0)
                {
                    return BadRequest(new { message = "error booking the table" });
                }

                return Ok(new { update = update, message = "Table is Available now !!!" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("clean-table")]
        public async Task<IActionResult> CleanTable([FromBody] UpdateTable table)
        {
            try
            {
                var update = await _tableService.CleanTableAsync(table);
                if (update <= 0)
                {
                    return BadRequest(new { update=update, message = "error cleanning the table" });
                }

                return Ok(new { update = update, message = "Table is cleanning !!!" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("generate-qr/{tableNo}/{updatedById}")]
        public IActionResult GetTableQRCode(int tableNo, int updatedById)
        {
            try
            {
                var base64Qr = _tableService.GenerateTableQRCode(tableNo, updatedById);
                return Ok(new { qrCode = base64Qr, message = "QR generated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}