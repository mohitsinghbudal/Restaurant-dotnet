using System.Diagnostics.Eventing.Reader;

namespace HotelManagementSystem.Models.MenuItems
{
    public class CreateMenu
    {
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public int CategoryId { get; set; }
        public int SubCategroyId { get; set; }
        public string? ItemImage { get; set; }
        public decimal ItemPrice{ get; set; }
        public int UnitId { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn{ get; set; }
        public int CreatedBy { get; set; }
        
    }
}
