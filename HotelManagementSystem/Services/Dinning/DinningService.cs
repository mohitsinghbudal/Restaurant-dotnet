using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Models.Dinning;

namespace HotelManagementSystem.Services.Dinning
{
    public class DinningService : IDinningService
    {
        private readonly IDinningDLL _dinningDLL;
        private readonly ITableDLL _table;


        public DinningService (IDinningDLL dinningDLL, ITableDLL table)
        {
            _dinningDLL = dinningDLL;
            _table = table;

        }

        public async Task<int> CreateDinningAsync(int tableId)
        {
            var table = await _table.GetTableByIdAsync(tableId);

            //if (table == null)
            //    throw new Exception("Table not found.");

            if (table.Status != "Occupied")
                throw new Exception("No guest on table.");

            var newDinning = new DinningModel
            {
                TableId = tableId,
                CustomerUserId = table.CreatedBy,
                StartedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SessionStatus = "Active"
            };

            return await _dinningDLL.CreateDinningAsync(newDinning);
        }

        public async Task<DinningModel> EndDinningSessionAsync(int sessionId)
        {
            var getDinning = await _dinningDLL.GetDinningByIdAsync(sessionId);

            getDinning.SessionStatus = "closed";
            getDinning.EndAt = DateTime.UtcNow;
            getDinning.UpdatedAt = DateTime.UtcNow;

            return await _dinningDLL.EndDinningSessionAsync(getDinning);
        }

    }
}
