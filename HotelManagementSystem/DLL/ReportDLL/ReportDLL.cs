using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.Report;
using HotelManagementSystem.Models.MenuItems;
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

        public async Task<IEnumerable<TopItemReport>> GetMostOrderedItems(
            DateTime startDate,
            DateTime endDate)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
SELECT TOP 5
       ItemName,
       COUNT(*) AS TotalQuantity
FROM Orders
WHERE CreatedAt >= @StartDate
AND CreatedAt < DATEADD(day,1,@EndDate)
GROUP BY ItemName
ORDER BY TotalQuantity DESC;";

            return await conn.QueryAsync<TopItemReport>(
                sql,
                new
                {
                    StartDate = startDate,
                    EndDate = endDate
                });
        }

        public async Task<IEnumerable<TopCustomer>> TopCustomer(
            DateTime startDate,
            DateTime endDate)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
SELECT TOP 5

u.UserId,

u.FirstName + ' ' +
ISNULL(u.MiddleName + ' ','') +
u.LastName AS FullName,

SUM(b.TotalAmount) AS TotalSpent

FROM Bills b

INNER JOIN Users u
ON b.PaidBy = u.UserId

WHERE b.IsPaid = 1

AND b.PaidAt >= @StartDate
AND b.PaidAt < DATEADD(day,1,@EndDate)

GROUP BY

u.UserId,
u.FirstName,
u.MiddleName,
u.LastName

ORDER BY TotalSpent DESC;";

            return await conn.QueryAsync<TopCustomer>(
                sql,
                new
                {
                    StartDate = startDate,
                    EndDate = endDate
                });
        }

        public async Task<FinancialSummary> GetRevenueByOrder(
            DateTime startDate,
            DateTime endDate)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
SELECT

ISNULL(SUM(TotalAmount),0) AS TotalRevenue,

COUNT(*) AS TotalOrders

FROM Bills

WHERE IsPaid = 1

AND PaidAt >= @StartDate
AND PaidAt < DATEADD(day,1,@EndDate);";

            return await conn.QuerySingleAsync<FinancialSummary>(
                sql,
                new
                {
                    StartDate = startDate,
                    EndDate = endDate
                });
        }
    }
}