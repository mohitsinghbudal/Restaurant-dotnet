namespace HotelManagementSystem.Models.Bill
{
    public class PayBill
    {
        public long BillNo { get; set; }
        public decimal BillAmount { get; set; }
        public int SessionId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime PaidAt { get; set; }
        public int PaidBy { get; set; }
    }
}
