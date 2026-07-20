using HotelManagementSystem.Helper.JWT;
using HotelManagementSystem.Interfaces.JWTInterface;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.User;

namespace HotelManagementSystem.Services.User
{
    public class UserServices : IUserService
    {
        private readonly IUserDLL _userDLL;
        private readonly IJWT _jwt;

        public UserServices(IUserDLL userDLL, IJWT jwt)
        {
            _userDLL = userDLL;
            _jwt = jwt;
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

            var newUser = new UserModel
            {
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNo = user.PhoneNo,
                PasswordHash = passwordHash,
                RoleId = user.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            return await _userDLL.SignUp(newUser);
        }
        
        public async Task<LoginResDTO> Login(LoginDTO user)
        {
            var existingUser = await _userDLL.GetUserByEmailAsync(user.Email);

            

            if (existingUser == null)
            {
                return null;
            }
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
    }
}