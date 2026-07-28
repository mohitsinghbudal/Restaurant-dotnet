namespace HotelManagementSystem.Interfaces.EmailInterface
{
    public interface IEmailService
    {
        Task SendOtpAsync(string email, string otp);
        Task SendEmailAsync(string to, string subject, string html);
    }
}
