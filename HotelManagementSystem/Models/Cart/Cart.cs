namespace HotelManagementSystem.Models.Cart
{
    public class Cart
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
