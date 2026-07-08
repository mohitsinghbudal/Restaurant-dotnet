namespace HotelManagementSystem.Models.Recipe
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public int MenuId { get; set; }
        public int InventoryItemId { get; set; }
        public decimal QantityRequired { get; set; }
        public int UnitId { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

    }
}
