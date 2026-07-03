namespace HotelManagementSystem.Models.Table
{
    public class UpdateTable
    {
        public string Status { get; set; }
        public int TableNo { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? WaiterId { get; set; }
    }
}
