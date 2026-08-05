using HotelManagementSystem.Controllers.usercontroller;
using HotelManagementSystem.Models.User;

namespace HotelManagementSystem.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserModelShow>> GetUsersAsync();
        Task<int> SignUp(SignUpDTO user);
        Task<LoginResDTO> Login(LoginDTO user);

        Task<bool> VerifyOTP(verifyotp req);
        Task<bool> UpdateUser(UserModel user);
        Task<bool> AssignRolesAsync(int userId, IEnumerable<int> roleIds, int adminUserId);
    }    
    public interface IUserDLL
    {
        Task<IEnumerable<UserModel>> GetUsersAsync();
        Task<UserModel?> GetUserByEmailAsync(string email);
        Task<int> SignUp(UserModel user);
        Task<int> AssignWaiterAsync();
        Task<bool> VerifyOtpUpdate(string email);
        Task<bool> UpdateUser(UserModel user);
        Task<List<int>> GetUserRoleIdsAsync(int userId);
        Task SoftDeleteRolesAsync(int userId, IEnumerable<int> roleIds, int deletedBy);
        Task AddRolesAsync(int userId, IEnumerable<int> roleIds, int assignedBy);
        Task<IEnumerable<UserModel>> GetAllUserAsync();
        Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds, int assignedBy);
    }
}
