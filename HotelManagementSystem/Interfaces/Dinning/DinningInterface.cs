using HotelManagementSystem.Models.Dinning;
using System.Threading.Tasks;

namespace HotelManagementSystem.Interfaces.DinningInterface
{
    public interface IDinningService
    {
        Task<int> CreateDinningAsync(int tableId,int userId);

        // Recommended addition for your upcoming Service Layer implementation:
        Task<int> EndDinningSessionAsync(int sessionId);
        Task<int> GetDiningSession(int userId);

    }

    public interface IDinningDLL
    {
        Task<int> CreateDinningAsync(DinningModel dinning);
        Task<DinningModel> GetDinningByIdAsync(int sessionId);
        Task<int> GetDiningSession(int userId);

        // FIX: Match the updated signature from your DinningDLL implementation
        Task<int> EndDinningSessionAsync(DinningModel dinning);

        // Fixed parameter name to remain consistent across implementations
        Task<DinningModel> GetDinningBySessionId(int id);
    }
}