namespace HotelManagementSystem.Models.Inventory
 {   public class InventoryItem
    {
        public int InventoryItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public int UnitId { get; set; }

        public decimal CurrentQuantity { get; set; }

        public decimal MinimumQuantity { get; set; }

        public decimal CostPrice { get; set; }

        public bool IsActive { get; set; } = true;

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}