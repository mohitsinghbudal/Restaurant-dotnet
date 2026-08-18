using HotelManagementSystem.Models.Dinning;

namespace HotelManagementSystem.Interfaces.DinningInterface;

    public interface IDinningDLL
{
    Task<IEnumerable<DinningModel>> GetAllDinningSessions();
    Task<int> GetDiningSession(int userId);
    Task<DinningModel> GetDinningBySessionId(int id);
    Task<int> EndDinningSessionAsync(DinningModel dinning);
    Task<DinningModel> GetDinningByIdAsync(int sessionId);
    Task<int> CreateDinningAsync(DinningModel dinning);
    Task<int?> GetMySessionId(int userId);
    Task<int> GetCustomerId(int tableId);


}
public interface IDinningService
{
    Task<int> GetDiningSession(int userId);
    Task<int> CreateDinningAsync(int tableId, int userId);
    Task<int> EndDinningSessionAsync(int sessionId);
    Task<IEnumerable<DinningModel>> GetAllDinningSessions();
    Task<int?> GetMySessionId(int userId);
    Task<int> GetCustomerId(int tableId);

}
