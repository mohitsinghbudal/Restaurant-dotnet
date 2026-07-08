using HotelManagementSystem.Models.Categories;

namespace HotelManagementSystem.Models.MenuItems
{
    public class Menu
    {
        public int MenuId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemDescription { get; set; }
        public int CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public string? ItemImage { get; set; }
        public decimal ItemPrice { get; set; }
        public int UnitId { get; set; } // Matches DB UnitId
        public bool IsAvailable { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public int? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }

        // --- Navigation Properties for Dapper Mapping ---
        public Category? Category { get; set; }
        public SubCategory? SubCategory { get; set; }
    }
}