namespace HotelManagementSystem.Models.Dinning
{
    public class DinningModel
    {
        public int SessionId { get; set; }

        public int TableId { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? EndAt { get; set; }

        public string SessionStatus { get; set; } = string.Empty;

        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }

    }
}