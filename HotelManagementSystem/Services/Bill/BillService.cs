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
using HotelManagementSystem.Interfaces.DinningInterface;

namespace HotelManagementSystem.Services.BillService
{
    public class BillService : IBillService
    {
        private readonly IBillDLL _billDLL;
        private readonly IOrderDLL _orderDLL;
        private readonly IConfiguration _config;
        private readonly IPaymentDLL _paymentDLL;
        private readonly HttpClient _httpClient;
        private readonly IDinningService _dinningService;
        public BillService(IBillDLL billDLL, IOrderDLL orderDLL, IConfiguration config, IHttpClientFactory httpClientFactory, IPaymentDLL paymentDLL, IDinningService dinningService)
        {
            _billDLL = billDLL;
            _orderDLL = orderDLL;
            _config = config;
            // Create a HttpClient from the factory to avoid lifetime mismatches
            _httpClient = httpClientFactory.CreateClient();
            _paymentDLL = paymentDLL;
            _dinningService = dinningService;
        }

        public async Task<Bill> ViewBillAsync(int sessionId)
        {
            return await _billDLL.ViewBillBySessionId(sessionId);
        }

        public async Task<Bill> CalculateSessionTotalAsync(int sessionId, decimal discountPercentage)
        {
            var orders = await _orderDLL.GetOrderBySessionId(sessionId);

            // 1. Block bill generation if any order is still in progress (e.g., Placed, Preparing)
            
            bool hasIncompleteOrders = orders.Any(order =>
                !order.OrderStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) &&
                !order.OrderStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)
            );

            if (hasIncompleteOrders)
            {
                throw new Exception("Current dining session orders are not completed");
            }

            // 2. Calculate grandTotal by including ONLY Completed orders (ignoring Cancelled)
            decimal grandTotal = orders
                .Where(order => order.OrderStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                .Sum(o => o.TotalAmount);

            // 3. Financial calculations
            decimal discount = grandTotal * (discountPercentage / 100M);
            decimal taxableAmount = grandTotal - discount;
            decimal tax = taxableAmount * 0.13M;
            decimal totalAmount = taxableAmount + tax;

            var newBill = new Bill
            {
                BillNo = long.Parse(DateTime.UtcNow.ToString("yyMMddHHmmss")),
                SessionId = sessionId,
                GrandTotal = grandTotal,
                TotalAmount = totalAmount,
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

            await _dinningService.EndDinningSessionAsync(sessionId);
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

            //always constant

            string transactionUuid = Guid.NewGuid().ToString(); /////
            string productCode = _config["Esewa:ProductCode"] ?? "EPAYTEST"; 
            string secretKey = _config["Esewa:SecretKey"] ?? "8gBm/:&EnhH.1/q";

            // eSewa configuration URLs

            string successUrl = _config["Esewa:SuccessUrl"] ?? "https://developer.esewa.com.np/success";
            string failureUrl = _config["Esewa:FailureUrl"] ?? "https://developer.esewa.com.np/failure";
            string signedFieldNames = "total_amount,transaction_uuid,product_code";

            //Strictly format to 2 decimal places
            string TaxAmount = bill.TaxAmount.ToString("0.00");   // Tax portion
            string Amount = bill.GrandTotal.ToString("0.00");     // Base portion
            string TotalAmount = bill.TotalAmount.ToString("0.00"); // Base + Tax

            Console.WriteLine(TotalAmount);
            Console.WriteLine(TotalAmount);
            Console.WriteLine(TotalAmount);

            Console.WriteLine(transactionUuid);


            string ServiceCharge = "0.00";
            string DeliveryCharge = "0.00";

            // HMAC Message string: total_amount,transaction_uuid,product_code
            string message = $"total_amount={TotalAmount},transaction_uuid={transactionUuid},product_code={productCode}";
            string signature = GenerateSignature(message, secretKey);


            // Record initial pending payment
            var payment = new Payment
            {
                BillId = bill.BillId,
                TransactionUuid = transactionUuid,
                PaymentGateway = "eSewa",
                Amount = bill.TotalAmount,
                Status = "Pending",
                Signature = signature,
                CreatedDate = DateTime.UtcNow
            };
            await _paymentDLL.CreatePaymentAsync(payment);



            return new EsewaInitiateResponseDto
            {
                Amount = Amount,
                TaxAmount = TaxAmount,
                TotalAmount = TotalAmount,
                ProductDeliveryCharge = DeliveryCharge,
                ProductServiceCharge = ServiceCharge,
                TransactionUuid = transactionUuid,
                ProductCode = productCode,
                Signature = signature,
                SignedFieldNames = signedFieldNames,
                SuccessUrl = successUrl,
                FailureUrl = failureUrl,
                PaymentUrl = _config["Esewa:PaymentUrl"] ?? "https://rc-epay.esewa.com.np/api/epay/main/v2/form"
            };
        }

        // --- Step B: Verify Callback Response from eSewa ---
        public async Task<bool> VerifyAndProcessEsewaCallbackAsync(string encodedData)
        {
            try
            {

                Console.WriteLine("reached the verification process");

                if (string.IsNullOrWhiteSpace(encodedData)) return false;

                // 1. Base64 Decode callback data payload
                byte[] base64Bytes = Convert.FromBase64String(encodedData);
                string decodedJson = Encoding.UTF8.GetString(base64Bytes);
                var callbackData = JsonSerializer.Deserialize<EsewaCallbackDecodedData>(decodedJson);

                if (callbackData == null || string.IsNullOrEmpty(callbackData.transaction_uuid))
                    return false;

                // 2. Fetch locally saved pending payment record
                var payment = await _paymentDLL.GetPaymentByUuidAsync(callbackData.transaction_uuid);
                if (payment == null) return false;

                // Idempotency Guard: prevent duplicate processing if already completed
                if (payment.Status == "Completed") return true;

                string productCode = _config["Esewa:ProductCode"] ?? "EPAYTEST";

                // Sanitize total_amount: prefer the callback numeric value if present, format with two decimals
                string totalAmountStr = callbackData.total_amount.HasValue
                    ? callbackData.total_amount.Value.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)
                    : payment.Amount.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);

                // Ensure two decimal places format if string comes back as "113.0"


                Console.WriteLine(productCode);
                Console.WriteLine(totalAmountStr);
                Console.WriteLine(callbackData.transaction_uuid);

                // 3. Double-check status with eSewa Status Verification API
                // CORRECT API PATH: /api/epay/transaction/status/
                string statusApiUrl = $"https://rc.esewa.com.np/api/epay/transaction/status/?product_code={productCode}&total_amount={totalAmountStr}&transaction_uuid={callbackData.transaction_uuid}";



                var request = new HttpRequestMessage(HttpMethod.Get, statusApiUrl);
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                var response = await _httpClient.SendAsync(request);

                Console.WriteLine("checking out the verififcation process");


                if (!response.IsSuccessStatusCode)
                {
                    payment.Status = "Failed";
                    payment.ResponseData = $"eSewa API returned HTTP {response.StatusCode}";
                    await _paymentDLL.UpdatePaymentAsync(payment);
                    return false;
                }

                var statusResponse = await response.Content.ReadFromJsonAsync<EsewaStatusApiResponse>();

                // 4. Update Payment & Bill records if verification passes
                if (statusResponse != null && statusResponse.status == "COMPLETE")
                {
                    payment.Status = "Completed";
                    payment.GatewayTransactionId = callbackData.transaction_code;
                    payment.ResponseData = decodedJson;

                    await _paymentDLL.UpdatePaymentAsync(payment);
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

            catch (Exception ex)
            {
                // Unhandled exception fallback
                return false;
            }
        }

       

        private string GenerateSignature(string message, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return Convert.ToBase64String(hash);
        }

        public async Task<IEnumerable<Bill>> GetBillAsync()
        {
            return await _billDLL.GetBillAsync();
        }
    }
}
