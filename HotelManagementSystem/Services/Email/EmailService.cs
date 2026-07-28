using HotelManagementSystem.Interfaces.EmailInterface;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        // C#
public async Task SendEmailAsync(string to, string subject, string html)
        {
            var fromEmail = _configuration["EmailSettings:SenderEmail"]?.Trim();
            var fromName = _configuration["EmailSettings:SenderName"]?.Trim();
            var host = _configuration["EmailSettings:Host"]?.Trim();
            var port = int.Parse(_configuration["EmailSettings:Port"]?.Trim() ?? "587");
            var appPassword = _configuration["EmailSettings:AppPassword"]?.Trim();

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(fromName, fromEmail));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = html };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromEmail, appPassword);
                await client.SendAsync(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", to);
                throw;
            }
            finally
            {
                try
                {
                    if (client.IsConnected)
                    {
                        await client.DisconnectAsync(true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to cleanly disconnect SMTP client");
                }
            }
        }
        public async Task SendOtpAsync(string email, string otp)
            {
                var fromEmail = _configuration["EmailSettings:SenderEmail"]?.Trim();
                var fromName = _configuration["EmailSettings:SenderName"]?.Trim();
                var host = _configuration["EmailSettings:Host"]?.Trim();
                var port = int.Parse(_configuration["EmailSettings:Port"]?.Trim() ?? "587");
                var appPassword = _configuration["EmailSettings:AppPassword"]?.Trim();

                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(fromName, fromEmail));

                message.To.Add(MailboxAddress.Parse(email));

                message.Subject = "Verify Your Email";

                message.Body = new TextPart("html")
                {
                    Text = $@"
                    <h2>Email Verification</h2>
                    <p>Your OTP is:</p>
                    <h1>{otp}</h1>
                    <p>This OTP expires in 10 minutes.</p>"
                };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                try
                {
                    await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(fromEmail, appPassword);
                    await smtp.SendAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send OTP to {Recipient}", email);
                    throw;
                }
                finally
                {
                    try
                    {
                        if (smtp.IsConnected)
                        {
                            await smtp.DisconnectAsync(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to cleanly disconnect SMTP client after OTP send");
                    }
                }
            }
    }
}
