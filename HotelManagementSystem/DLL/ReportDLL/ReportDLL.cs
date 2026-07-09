using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.Report;
using HotelManagementSystem.Models.Report;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace HotelManagementSystem.DLL.ReportDLL
{
    public class ReportDLL: IReportDLL
    {
        private readonly IDbConnectionFactory _dbConn;
        public ReportDLL(IDbConnectionFactory dbConn)
        {
           _dbConn = dbConn;
        }

        public async Task<DashboardReport> GetDashboardReportAsync()
        {
            using var conn = _dbConn.CreateConnection();
            string procedureName = "sp_GetDashboardReports";

            using var multi = await conn.QueryMultipleAsync(
                procedureName,
                commandType: CommandType.StoredProcedure
            );

            var report = new DashboardReport();

            report.MostOrderedItems = await multi.ReadAsync<TopItemReport>();
            report.RegularCustomers = await multi.ReadAsync<TopCustomerReport>();
            report.Summary = await multi.ReadSingleOrDefaultAsync<FinancialSummary>() ?? new FinancialSummary();

            return report;
        }

    }
}
