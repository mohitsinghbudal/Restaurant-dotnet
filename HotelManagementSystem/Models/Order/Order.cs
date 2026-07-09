namespace HotelManagementSystem.Models.Order
{
    public class Order
    {
        public int MenuId { get; set; }
        public int OrderId { get; set; }
        public string ItemName { get; set; }
        public int DiningSessionId { get; set; }

        public string OrderStatus { get; set; } = "Pending";

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }


    }
}
