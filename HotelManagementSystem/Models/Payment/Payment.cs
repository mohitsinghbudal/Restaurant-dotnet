namespace HotelManagementSystem.Models.Payment
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int BillId { get; set; }

        public string TransactionUuid { get; set; } = Guid.NewGuid().ToString();

        public string? GatewayTransactionId { get; set; }

        public string PaymentGateway { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Status { get; set; } = "Pending";

        public string? Signature { get; set; }

        public string? ResponseData { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }
    }
}