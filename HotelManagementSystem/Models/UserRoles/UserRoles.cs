namespace HotelManagementSystem.Models.UserRoles
{
    public class UserRoles
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedAt { get; set; }
        public int AssignedBy { get; set; }
        public bool IsDelete { get; set; }
        public DateTime DeletedAt{ get; set; }
        public int DeletedBy { get; set; }
    }
}
