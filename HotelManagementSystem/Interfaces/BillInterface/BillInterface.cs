using HotelManagementSystem.Models.Bill;
using HotelManagementSystem.Models.Payment;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Interfaces.BillInterface
{
    public interface IBillService
    {
        Task<Bill> PayBillCash(PayBill pay);
        Task<Bill> CalculateSessionTotalAsync(int sessionId, decimal discountPercentage);
        Task<Bill?> GenerateBillAsync(int sessionId, decimal subTotalAmount, decimal discountPercentage, string paymentMethod, int userId);
        Task<EsewaInitiateResponseDto> InitiateEsewaPaymentAsync(int sessionId);
        Task<bool> VerifyAndProcessEsewaCallbackAsync(string encodedData);
        //Task<IActionResult> PayBillEsewaSuccess(object obj);



        Task<Bill> ViewBillAsync(int sessionId);
    }
    public interface IBillDLL
    {
        Task<int> GetNextBillNoAsync();
        Task<Bill> CreateBillAsync(Bill bill);
        Task<Bill> PayBillAsync(bool pay, long bilno, string paymentmethod);
        Task<Bill?> GetBillByNoAsync(long billNo);
        Task<Bill> ViewBillBySessionId(int sessionId);
        Task<bool> MarkBillAsPaidAsync(int billid);
    }
}
