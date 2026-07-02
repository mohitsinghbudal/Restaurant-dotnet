namespace HotelManagementSystem.Models.User
{
    public class UserModel
    {
        public string FirstName { get; set; }=string.Empty;
        public int UserId { get; set; }
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string PhoneNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
