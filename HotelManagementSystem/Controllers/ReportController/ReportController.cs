using HotelManagementSystem.Interfaces.Report;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.ReportController
{
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
            byte[] fileBytes = await _reportService.ExportDashboardReportAsync();
            string filename = $"Dashboard_Performance_Report_{DateTime.UtcNow:yyyyMMdd}.xlsx";

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filename
            );
        }
    }
}
