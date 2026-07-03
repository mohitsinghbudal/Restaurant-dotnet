namespace HotelManagementSystem.Models.Dinning
{
    public class DinningModel
    {
        
        public int TableId { get; set; }
        public int CustomerUserId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public String SessionStatus { get; set; }
        //public int WaiterUserId { get; set; }
    }

}
