namespace HotelManagementSystem.Models.MenuItems
{
    public class ShowMenu
    {
        public int MenuId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string? ItemDescription { get; set; }

        public int CategoryId { get; set; }

        public int SubCategoryId { get; set; }

        public string? ItemImage { get; set; }

        public decimal ItemPrice { get; set; }

        public int UnitId { get; set; }

        
        public bool IsAvailable { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedOn { get; set; }

        
        public int AvailablePortions { get; set; }
    }
}
