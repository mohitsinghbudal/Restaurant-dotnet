namespace HotelManagementSystem.Models.User
{
    public class LoginResDTO
    {
        public string token { get; set; } = string.Empty;
        public int userId { get; set; }
        public int roleId { get; set; }
    }
}
