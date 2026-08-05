using DocumentFormat.OpenXml.Spreadsheet;
using HotelManagementSystem.Controllers.usercontroller;
using HotelManagementSystem.Helper.JWT;
using HotelManagementSystem.Interfaces.EmailInterface;
using HotelManagementSystem.Interfaces.JWTInterface;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.User;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data;

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

        public async Task<IEnumerable<UserModelShow>> GetUsersAsync()
        {
            var users = await _userDLL.GetAllUserAsync();

            var result = new List<UserModelShow>();

            foreach (var user in users)
            {
               List<int> roles = await _userDLL.GetUserRoleIdsAsync(user.UserId);

                result.Add(new UserModelShow
                {
                    
                    User = user,
                    Roles = roles
                });
            }

            return result;
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
                return null;

            if (!existingUser.IsEmailVerified)
                return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
                user.Password,
                existingUser.PasswordHash);

            if (!isPasswordValid)
                return null;

            // Get all roles of the user
            List<int> roles = await _userDLL.GetUserRoleIdsAsync(existingUser.UserId);

            // Generate JWT with multiple roles
            var token = _jwt.JwtToken(existingUser, roles);


            if (string.IsNullOrEmpty(token))
                throw new Exception("Token generation failed.");

            return new LoginResDTO
            {
                token = token,
                userId = existingUser.UserId,
                roles = roles
            };
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

        public async Task<bool> UpdateUser(UserModel user)
        {
            return await _userDLL.UpdateUser(user);
        }
        public async Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds, int assignedBy)
        {
            var currentRoles = await _userDLL.GetUserRoleIdsAsync(userId);

            var existingRoles = currentRoles.OrderBy(x => x).ToList();
            var newRoles = roleIds.Distinct().OrderBy(x => x).ToList();

            // Nothing changed
            if (existingRoles.SequenceEqual(newRoles))
                return true;

            // Roles removed
            var rolesToRemove = existingRoles.Except(newRoles).ToList();

            // Roles added
            var rolesToAdd = newRoles.Except(existingRoles).ToList();

            if (rolesToRemove.Any())
            {
                await _userDLL.SoftDeleteRolesAsync(userId, rolesToRemove, assignedBy);
            }

            if (rolesToAdd.Any())
            {
                await _userDLL.AddRolesAsync(userId, rolesToAdd, assignedBy);
            }

            return true;
        }

        public async Task<bool> AssignRolesAsync(int userId, IEnumerable<int> roleIds, int adminUserId)
        {
            // Delegate to the existing UpdateUserRolesAsync implementation to avoid duplication
            return await UpdateUserRolesAsync(userId, roleIds, adminUserId);
        }
    }
}