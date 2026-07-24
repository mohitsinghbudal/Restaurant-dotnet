namespace HotelManagementSystem.Models.Bill
{
    public class Bill
    {
        public int BillId { get; set; }

        public long BillNo { get; set; }

        public int SessionId { get; set; }

        public decimal GrandTotal { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public bool IsPaid { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public DateTime? PaidAt { get; set; }

        public int? PaidBy { get; set; }
    }

}
