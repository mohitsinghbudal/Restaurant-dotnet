namespace HotelManagementSystem.Models.Table
{
    public class TableModel
    {
        public int TableId { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
        public int TableNo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? WaiterId { get; set; }

        

    }
}
