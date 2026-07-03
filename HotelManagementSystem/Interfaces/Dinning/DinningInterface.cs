using HotelManagementSystem.Models.Dinning;

namespace HotelManagementSystem.Interfaces.DinningInterface
{
    public interface IDinningService
    {
        Task<int> CreateDinningAsync(int tableId);
    }
    public interface IDinningDLL
    {
        Task<int> CreateDinningAsync(DinningModel dinning);
        //Task<int?> AssignWaiterAsync();
        Task<DinningModel> GetDinningByIdAsync(int sessionId);
        Task<DinningModel> EndDinningSessionAsync(DinningModel sessionId);

        Task<DinningModel> GetDinningBySessionId(int id);
    }
}
