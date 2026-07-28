using DocumentFormat.OpenXml.Spreadsheet;
using HotelManagementSystem.Controllers.usercontroller;
using HotelManagementSystem.Helper.JWT;
using HotelManagementSystem.Interfaces.EmailInterface;
using HotelManagementSystem.Interfaces.JWTInterface;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.User;

namespace HotelManagementSystem.Services.User
{
    public class UserServices : IUserService
    {
        private readonly IUserDLL _userDLL;
        private readonly IJWT _jwt;
        private readonly IEmailService _emailService;

        public UserServices(IUserDLL userDLL, IJWT jwt, IEmailService emailService)
        {
            _userDLL = userDLL;
            _jwt = jwt;
            _emailService = emailService;
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            return await _userDLL.GetUsersAsync();
        }

        public async Task<int> SignUp(SignUpDTO user)
        {
            var existingUser = await _userDLL.GetUserByEmailAsync(user.Email);
            

            if (existingUser != null)
            {
                return 0;
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            var otp = new Random().Next(100000, 999999).ToString();

            string subject = "Verify Your Account - OTP Code";
            string htmlContent = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
            <h2>Welcome to Gourmet Haven!</h2>
            <p>Thank you for signing up. Please use the following One-Time Password (OTP) to verify your email address:</p>
            <h1 style='color: #2b6cb0; letter-spacing: 2px;'>{otp}</h1>
            <p>This code is valid for <strong>10 minutes</strong>.</p>
            <p>If you did not request this, please ignore this email.</p>
        </div>";

            await _emailService.SendEmailAsync(user.Email, subject, htmlContent);

            var newUser = new UserModel
            {
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNo = user.PhoneNo,
                PasswordHash = passwordHash,
                //RoleId = user.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailOtp = otp,
                OtpExpiry = DateTime.UtcNow.AddMinutes(10),
                IsEmailVerified = false

            };

            var User = await _userDLL.SignUp(newUser);
            
            return User;
        }
        
        public async Task<LoginResDTO> Login(LoginDTO user)
        {
            var existingUser = await _userDLL.GetUserByEmailAsync(user.Email);

            

            if (existingUser == null)
            {
                return null;
            }

            if (existingUser.IsEmailVerified != true) return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, existingUser.PasswordHash);
            if (!isPasswordValid)
            {
                return null;
            }

            var token = _jwt.JwtToken(existingUser);
            if (token == null)
            {
                throw new Exception("token generation failed");
            }

            var res = new LoginResDTO
            {
                token = token,
                userId = existingUser.UserId,
                roleId = existingUser.RoleId
            };

            

            return res;
        }

        public async Task<bool> VerifyOTP(verifyotp req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.otp))
                return false;

            var existingUser = await _userDLL.GetUserByEmailAsync(req.Email);

            if (existingUser.IsEmailVerified)
                return true;

            if (existingUser.OtpExpiry < DateTime.UtcNow)
            {
                return false;
            }

            if (existingUser.EmailOtp.ToString().Trim() == req.otp.Trim())
            {

                bool update = await _userDLL.VerifyOtpUpdate(req.Email);
                return true;
                    
            }

            
            else return false;
        }
    }
}