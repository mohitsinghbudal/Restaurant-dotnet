using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.User;

namespace HotelManagementSystem.Services.User
{
    public class UserServices : IUserService
    {
        private readonly IUserDLL _userDLL;

        public UserServices(IUserDLL userDLL)
        {
            _userDLL = userDLL;
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
    }
}