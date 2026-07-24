using DocumentFormat.OpenXml.Bibliography;
using HotelManagementSystem.Interfaces.BillInterface;
using HotelManagementSystem.Interfaces.OrderInterface;
using HotelManagementSystem.Interfaces.PaymentInterface;
using HotelManagementSystem.Models.Bill;
using HotelManagementSystem.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace HotelManagementSystem.Services.BillService
{
    public class BillService : IBillService
    {
        private readonly IBillDLL _billDLL;
        private readonly IOrderDLL _orderDLL;
        private readonly IConfiguration _config;
        private readonly IPaymentDLL _paymentDLL;
        private readonly HttpClient _httpClient;
        public BillService(IBillDLL billDLL, IOrderDLL orderDLL, IConfiguration config, IHttpClientFactory httpClientFactory, IPaymentDLL paymentDLL)
        {
            _billDLL = billDLL;
            _orderDLL = orderDLL;
            _config = config;
            // Create a HttpClient from the factory to avoid lifetime mismatches
            _httpClient = httpClientFactory.CreateClient();
            _paymentDLL = paymentDLL;
        }

        public async Task<Bill> ViewBillAsync(int sessionId)
        {
            return await _billDLL.ViewBillBySessionId(sessionId);
        }

        public async Task<Bill> CalculateSessionTotalAsync(int sessionId, decimal discountPercentage)
        {
   
            var orders = await _orderDLL.GetOrderBySessionId(sessionId);



            decimal grandTotal = orders.Sum(o => o.TotalAmount);
            
            decimal discount = grandTotal * (discountPercentage / 100M);

            decimal taxableAmount = grandTotal - discount;

            decimal tax = taxableAmount * 0.13M;

            decimal TotalAmount = taxableAmount + tax;

            var newBill = new Bill
            {
                BillNo = long.Parse(DateTime.UtcNow.ToString("yyMMddHHmmss")),
                SessionId = sessionId,
                GrandTotal = grandTotal, //final after all addition and deduction
                TotalAmount = TotalAmount, //food amount only
                TaxAmount = tax,
                DiscountAmount = discount,
                PaymentMethod = "any",
                IsPaid = false,
                CreatedDate = DateTime.UtcNow,
            };

            return await _billDLL.CreateBillAsync(newBill);
        }
        public async Task<Bill?> GenerateBillAsync(int sessionId, decimal subTotalAmount, decimal discountPercentage, string paymentMethod, int userId)
        {
            if (sessionId <= 0) throw new ArgumentException("Invalid Session ID.");
            if (subTotalAmount < 0) throw new ArgumentException("Subtotal cannot be negative.");

            // 1. Math Calculation Blocks
            decimal discountAmount = subTotalAmount * (discountPercentage / 100m);
            decimal taxableAmount = subTotalAmount - discountAmount;

            if (taxableAmount < 0) taxableAmount = 0;
            decimal taxAmount = taxableAmount * 0.13M;
            decimal totalAmount = taxableAmount + taxAmount;

            // 2. Fetch sequential auto-incrementing Bill number configuration
            int nextBillNo = await _billDLL.GetNextBillNoAsync();

            // 3. Assemble complete Model properties aligning with non-null table schemas
            var bill = new Bill
            {
                BillNo = nextBillNo,
                SessionId = sessionId,
                DiscountAmount = Math.Round(discountAmount, 2),
                TaxAmount = Math.Round(taxAmount, 2),
                TotalAmount = Math.Round(totalAmount, 2),
                PaymentMethod = string.IsNullOrWhiteSpace(paymentMethod) ? "Cash" : paymentMethod,
                IsPaid = false, // Set true assuming processing happens at physical point of sale settlement
                CreatedDate = DateTime.UtcNow,
                PaidAt = DateTime.UtcNow,
                PaidBy = userId
            };

            // 4. Save calculations directly downstream via DLL
            return await _billDLL.CreateBillAsync(bill);
        }
        public async Task<Bill> PayBillCash(PayBill pay)
        {
            
            var bill = await _billDLL.GetBillByNoAsync(pay.BillNo);


            bill.PaymentMethod = "Cash";
            bill.IsPaid = true;
            bill.PaidAt = DateTime.UtcNow;


            return await _billDLL.PayBillAsync(bill.IsPaid, pay.BillNo,bill.PaymentMethod);
        }

        public async Task<EsewaInitiateResponseDto> InitiateEsewaPaymentAsync(int sessionId)
        {
            var bill = await _billDLL.ViewBillBySessionId(sessionId);
            if (bill == null) throw new Exception("No bill found for this session.");
            if (bill.IsPaid) throw new Exception("This bill has already been paid.");

            string transactionUuid = Guid.NewGuid().ToString();
            string productCode = _config["Esewa:ProductCode"] ?? "EPAYTEST";
            string secretKey = _config["Esewa:SecretKey"] ?? "8gBm/:&EnhH.1/q";

            // Strictly format to 2 decimal places
            string formattedTotalAmount = bill.GrandTotal.ToString("0.00"); // Base + Tax
            string formattedTaxAmount = bill.TaxAmount.ToString("0.00");   // Tax portion
            string formattedAmount = bill.TotalAmount.ToString("0.00");     // Base portion

            // HMAC Message string strictly: total_amount,transaction_uuid,product_code
            string message = $"total_amount={formattedTotalAmount},transaction_uuid={transactionUuid},product_code={productCode}";
            string signature = GenerateSignature(message, secretKey);

            // Record initial pending payment
            var payment = new Payment
            {
                BillId = bill.BillId,
                TransactionUuid = transactionUuid,
                PaymentGateway = "eSewa",
                Amount = bill.GrandTotal,
                Status = "Pending",
                Signature = signature,
                CreatedDate = DateTime.UtcNow
            };
            await _paymentDLL.CreatePaymentAsync(payment);

            return new EsewaInitiateResponseDto
            {
                Amount = formattedAmount,                
                TaxAmount = formattedTaxAmount,          
                TotalAmount = formattedTotalAmount,      
                TransactionUuid = transactionUuid,
                ProductCode = productCode,
                Signature = signature,
                PaymentUrl = _config["Esewa:PaymentUrl"] ?? "https://rc-epay.esewa.com.np/api/epay/main/v2/form"
            };
        }

        // --- Step B: Verify Callback Response from eSewa ---
        public async Task<bool> VerifyAndProcessEsewaCallbackAsync(string encodedData)
        {
            // 1. Base64 Decode callback data payload
            byte[] base64Bytes = Convert.FromBase64String(encodedData);
            string decodedJson = Encoding.UTF8.GetString(base64Bytes);
            var callbackData = JsonSerializer.Deserialize<EsewaCallbackDecodedData>(decodedJson);

            if (callbackData == null || string.IsNullOrEmpty(callbackData.transaction_uuid))
                return false;

            // 2. Fetch locally saved pending payment record
            var payment = await _paymentDLL.GetPaymentByUuidAsync(callbackData.transaction_uuid);
            if (payment == null) return false;

            string productCode = _config["Esewa:ProductCode"] ?? "EPAYTEST";

            // 3. Double-check status with eSewa Status Verification API
            string statusApiUrl = $"https://rc-epay.esewa.com.np/api/epay/transaction/status/?product_code={productCode}&total_amount={callbackData.total_amount}&transaction_uuid={callbackData.transaction_uuid}";

            var statusResponse = await _httpClient.GetFromJsonAsync<EsewaStatusApiResponse>(statusApiUrl);

            // 4. Update Payment & Bill records if verification passes
            if (statusResponse != null && statusResponse.status == "COMPLETE")
            {
                payment.Status = "Completed";
                payment.GatewayTransactionId = callbackData.transaction_code;
                payment.ResponseData = decodedJson;

                await _paymentDLL.UpdatePaymentAsync(payment);

                // Update Bill table status to Paid
                await _billDLL.MarkBillAsPaidAsync(payment.BillId);
                return true;
            }
            else
            {
                payment.Status = "Failed";
                payment.ResponseData = decodedJson;
                await _paymentDLL.UpdatePaymentAsync(payment);
                return false;
            }
        }
        private string GenerateSignature(string message, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return Convert.ToBase64String(hash);
        }
    }
}
