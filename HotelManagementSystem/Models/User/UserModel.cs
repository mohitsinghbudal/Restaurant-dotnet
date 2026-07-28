using DocumentFormat.OpenXml.Bibliography;

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
        public string PhoneNo { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public string EmailOtp { get; set; } = string.Empty;
        public DateTime OtpExpiry { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}
