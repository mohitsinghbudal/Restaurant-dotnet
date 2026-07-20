using HotelManagementSystem.Models.User;

namespace HotelManagementSystem.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetUsersAsync();
         Task<int> SignUp(SignUpDTO user);
        Task<LoginResDTO> Login(LoginDTO user);
    }
    public interface IUserDLL
    {
        Task<IEnumerable<UserModel>> GetUsersAsync();
        Task<UserModel?> GetUserByEmailAsync(string email);
        Task<int> SignUp(UserModel user);
        Task<int> AssignWaiterAsync();


    }
}
