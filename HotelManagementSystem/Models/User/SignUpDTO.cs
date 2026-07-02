namespace HotelManagementSystem.Models.User
{
    public class SignUpDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string PhoneNo { get; set; }
        public bool IsActive { get; set; }


    }
}
