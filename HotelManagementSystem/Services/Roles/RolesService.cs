using HotelManagementSystem.Interfaces.Roles;
using HotelManagementSystem.Models.Roles;

namespace HotelManagementSystem.Services.Roles
{
    public class RolesService : IRoleService
    {
        private readonly IRoleDLL _roleDLL;
        public RolesService(IRoleDLL roleDLL)
        {
            _roleDLL = roleDLL;
        }
        public async Task<IEnumerable<Role>> GetRoleAsync()
        {
            return await _roleDLL.GetRoleAsync();
        }
        public async Task<int> CreateRoleAsync(Role role)
        {
            return await _roleDLL.CreateRoleAsync(role);
        }
        public async Task<bool> UpdateRoleAsync(Role role)
        {
            return await _roleDLL.UpdateRoleAsync(role);
        }
        public async Task<bool> DeleteRoleAsync(int id)
        {
            return await _roleDLL.DeleteRoleAsync(id);
        }
    }
}
