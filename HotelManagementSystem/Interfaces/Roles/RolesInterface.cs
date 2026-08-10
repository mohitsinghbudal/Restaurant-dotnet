using HotelManagementSystem.Models.Roles;

namespace HotelManagementSystem.Interfaces.Roles
{
    public interface IRoleDLL
    {
        Task<IEnumerable<Role>> GetRoleAsync();
        Task<int> CreateRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int id);
    }
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetRoleAsync();
        Task<int> CreateRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int id);
    }

}
