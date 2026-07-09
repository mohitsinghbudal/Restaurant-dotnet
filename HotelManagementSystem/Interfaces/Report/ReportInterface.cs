using HotelManagementSystem.Models.Report;

namespace HotelManagementSystem.Interfaces.Report
{
    public interface IReportService
    {
        Task<byte[]> ExportDashboardReportAsync();
    }
    public interface IReportDLL
    {
        Task<DashboardReport> GetDashboardReportAsync();
    }
}
