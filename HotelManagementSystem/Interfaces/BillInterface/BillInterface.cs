using HotelManagementSystem.Models.Bill;

namespace HotelManagementSystem.Interfaces.BillInterface
{
    public interface IBillSeervice
    {
        Task<Bill> PayBill(PayBill pay);
        Task<Bill> CalculateSessionTotalAsync(int sessionId, decimal discountPercentage);
    }
    public interface IBillDLL
    {
        Task<int> GetNextBillNoAsync();
        Task<Bill> CreateBillAsync(Bill bill);
        Task<Bill> PayBillAsync(bool pay, long bilno);
        Task<Bill?> GetBillByNoAsync(long billNo);
    }
}
