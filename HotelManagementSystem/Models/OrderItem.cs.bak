namespace HotelManagementSystem.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Calculated automatically by the database
        public decimal LineTotal { get; }

        public string? Remarks { get; set; }
        public string ItemStatus { get; set; } = "Pending";
        public bool IsActive { get; set; } = true;

        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
