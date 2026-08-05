using HotelManagementSystem.Models.Report;

namespace HotelManagementSystem.Interfaces.Report
{
    public interface IReportService
    {
        Task<byte[]> ExportDashboardReportAsync();

        Task<IEnumerable<TopItemReport>> GetMostOrderedItems(
            DateTime startDate,
            DateTime endDate);

        Task<IEnumerable<TopCustomer>> TopCustomer(
            DateTime startDate,
            DateTime endDate);

        Task<FinancialSummary> GetRevenueByOrder(
            DateTime startDate,
            DateTime endDate);
    }
    public interface IReportDLL
    {
        Task<DashboardReport> GetDashboardReportAsync();

        Task<IEnumerable<TopItemReport>> GetMostOrderedItems(DateTime startDate, DateTime endDate);

        Task<IEnumerable<TopCustomer>> TopCustomer(DateTime startDate, DateTime endDate);

        Task<FinancialSummary> GetRevenueByOrder(DateTime startDate, DateTime endDate);
    }
}
