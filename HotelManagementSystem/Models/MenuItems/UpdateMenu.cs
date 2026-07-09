using HotelManagementSystem.Models.Categories;

namespace HotelManagementSystem.Models.MenuItems
{
    public class UpdateMenu
    {
        public int MenuId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemDescription { get; set; }
        public int CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public string? ItemImage { get; set; }
        public decimal ItemPrice { get; set; }
        public int UnitId { get; set; } 
        public bool IsAvailable { get; set; } = true;
        
        public int? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
     
    }
}
