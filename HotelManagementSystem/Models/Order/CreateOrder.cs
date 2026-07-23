namespace HotelManagementSystem.Models.Order
{
    public class CreateOrderItems
    {
        public List<CreateOrder> Items { get; set; }
    }
    public class CreateOrder
    {
        public int MenuId { get; set; }
        public string ItemName { get; set; }
        public int DiningSessionId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } 
        

    }
}
