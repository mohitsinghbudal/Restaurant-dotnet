using Dapper;
using HotelManagementSystem.Interfaces.BillInterface;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Models.Bill;
using System.Net.NetworkInformation;

namespace HotelManagementSystem.DLL.BillDLL
{
    
        public class BillDLL : IBillDLL
        {
            private readonly IDbConnectionFactory _dbConn;

            public BillDLL(IDbConnectionFactory dbConn)
            {
                _dbConn = dbConn;
            }

            public async Task<int> GetNextBillNoAsync()
            {
                using var conn = _dbConn.CreateConnection();
                // Fallback to 1 if it's the very first bill in the system
                string sql = "SELECT ISNULL(MAX(BillNo), 0) + 1 FROM Bills;";
                return await conn.ExecuteScalarAsync<int>(sql);
            }

            public async Task<Bill> CreateBillAsync(Bill bill)
            {
                using var conn = _dbConn.CreateConnection();
                string sql = @"
                INSERT INTO Bills 
                (
                    BillNo, SessionId, TotalAmount, TaxAmount, DiscountAmount, 
                    PaymentMethod, IsPaid, CreatedDate
                )
                OUTPUT INSERTED.*
                VALUES 
                (
                    @BillNo, @SessionId, @TotalAmount, @TaxAmount, @DiscountAmount, 
                    @PaymentMethod, @IsPaid, @CreatedDate
                );";

                return await conn.QueryFirstOrDefaultAsync<Bill>(sql, bill);
            }

            public async Task<Bill> PayBillAsync(bool pay, long bilno)
            {
            using var conn = _dbConn.CreateConnection();
            string sql = @"
        UPDATE Bills 
        SET IsPaid = @IsPaid, 
            PaidAt = GETUTCDATE()
        OUTPUT INSERTED.*
        WHERE BillNo = @BillNo;";
            return await conn.QueryFirstOrDefaultAsync<Bill>(sql, new { IsPaid = pay, BillNo = bilno });

            }
        public async Task<Bill?> GetBillByNoAsync(long billNo)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"SELECT * FROM Bills WHERE BillNo = @BillNo;";

            return await conn.QueryFirstOrDefaultAsync<Bill>(sql, new { BillNo = billNo });
        }

    }

}