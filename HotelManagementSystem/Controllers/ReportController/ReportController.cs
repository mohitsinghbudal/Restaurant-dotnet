using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Interfaces.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.ReportController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("dashboard/excel")]
        public async Task<IActionResult> DownloadDashboardExcel()
        {
            if (!User.IsInRole("5")) 
            {
                throw new Exception("User not allowed");
            }

            byte[] fileBytes = await _reportService.ExportDashboardReportAsync();
            string filename = $"Dashboard_Performance_Report_{DateTime.UtcNow:yyyyMMdd}.xlsx";

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filename
            );
        }

        [HttpGet("top-selling-item")]
        public async Task<IActionResult> GetTopSellingItem(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (!User.IsInRole("5"))
                {
                    throw new Exception("User not allowed");
                }

                var result = await _reportService.GetMostOrderedItems(startDate, endDate);

                return Ok(new
                {
                    response = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("top-customer")]
        public async Task<IActionResult> GetTopCustomer(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (!User.IsInRole("5"))
                {
                    throw new Exception("User not allowed");
                }
                var result = await _reportService.TopCustomer(startDate, endDate);

                return Ok(new
                {
                    response = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("get-total-financial-order")]
        public async Task<IActionResult> GetFinancialReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (!User.IsInRole("5"))
                {
                    throw new Exception("User not allowed");
                }

                var result = await _reportService.GetRevenueByOrder(startDate, endDate);

                return Ok(new
                {
                    response = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
    