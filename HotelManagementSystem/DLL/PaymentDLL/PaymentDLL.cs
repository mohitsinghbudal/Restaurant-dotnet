using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.PaymentInterface;
using HotelManagementSystem.Models.Payment;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.DLL.PaymentDLL
{
    public class PaymentDLL: IPaymentDLL
    {
        private readonly IDbConnectionFactory _dbConn;

        public PaymentDLL(IDbConnectionFactory dbConn)
        {
            _dbConn = dbConn;
        }
        public async Task<int> CreatePaymentAsync(Payment pay)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
                INSERT INTO Payment (
                    BillId, 
                    TransactionUuid, 
                    GatewayTransactionId, 
                    PaymentGateway, 
                    Amount, 
                    Status, 
                    Signature, 
                    ResponseData, 
                    CreatedDate
                )
                VALUES (
                    @BillId, 
                    @TransactionUuid, 
                    @GatewayTransactionId, 
                    @PaymentGateway, 
                    @Amount, 
                    @Status, 
                    @Signature, 
                    @ResponseData, 
                    @CreatedDate
                );";

            return await conn.ExecuteAsync(sql, pay);
        }
        public async Task<bool> UpdatePaymentAsync(Payment pay)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
                UPDATE Payment
                SET 
                    GatewayTransactionId = @GatewayTransactionId,
                    Status = @Status,
                    Signature = @Signature,
                    ResponseData = @ResponseData,
                    UpdatedDate = @UpdatedDate
                WHERE TransactionUuid = @TransactionUuid;";

            pay.UpdatedDate = DateTime.UtcNow;

            int rowsAffected = await conn.ExecuteAsync(sql, pay);
            return rowsAffected > 0;
        }

        public async Task<Payment?> GetPaymentByUuidAsync(string transactionUuid)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
                SELECT * FROM Payment 
                WHERE TransactionUuid = @TransactionUuid;";

            return await conn.QueryFirstOrDefaultAsync<Payment>(sql, new { TransactionUuid = transactionUuid });
        }
    }
}
