using HotelManagementSystem.Interfaces.BillInterface;
using HotelManagementSystem.Interfaces.OrderInterface;
using HotelManagementSystem.Models.Bill;

namespace HotelManagementSystem.Services.BillService
{
    public class BillService : IBillSeervice
    {
        private readonly IBillDLL _billDLL;
        private const decimal TAX_RATE = 0.13m; // 13% standard tax rate
        private readonly IOrderDLL _orderDLL;
        public BillService(IBillDLL billDLL, IOrderDLL orderDLL)
        {
            _billDLL = billDLL;
            _orderDLL = orderDLL;
        }

        public async Task<Bill> CalculateSessionTotalAsync(int sessionId, decimal discountPercentage)
        {
            var orders = await _orderDLL.GetOrderBySessionId(sessionId);

            decimal grandTotal = orders.Sum(o => o.TotalAmount);
            decimal tax = grandTotal * 0.11M;
            decimal discount = grandTotal * (discountPercentage / 100M);

            var newBill = new Bill
            {
                BillNo = long.Parse(DateTime.UtcNow.ToString("yyMMddHHmmss")),
                SessionId = sessionId,
                TotalAmount = grandTotal,
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
            decimal taxAmount = taxableAmount * TAX_RATE;
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
        public async Task<Bill> PayBill(PayBill pay)
        {
            
            var bill = await _billDLL.GetBillByNoAsync(pay.BillNo);

            

            bill.IsPaid = true;
            bill.PaidAt = DateTime.UtcNow;


            return await _billDLL.PayBillAsync(bill.IsPaid, pay.BillNo);
        }
    }
}
