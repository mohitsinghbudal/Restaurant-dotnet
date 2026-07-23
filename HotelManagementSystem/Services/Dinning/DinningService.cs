using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Models.Dinning;
using HotelManagementSystem.Models.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelManagementSystem.Services.Dinning
{
    public class DinningService : IDinningService
    {
        private readonly IDinningDLL _dinningDLL;
        private readonly ITableDLL _tableDLL;

        public DinningService(IDinningDLL dinningDLL, ITableDLL tableDLL)
        {
            _dinningDLL = dinningDLL;
            _tableDLL = tableDLL;
        }

        public async Task<int> GetDiningSession(int userId)
        {
            return await _dinningDLL.GetDiningSession( userId);
        }

        public async Task<int> CreateDinningAsync(int tableId, int userId)
        {
            var table = await _tableDLL.GetTableByIdAsync(tableId);

            // FIX: Null guard check must happen BEFORE accessing properties like table.Status
            if (table == null)
            {
                throw new KeyNotFoundException($"Table with ID {tableId} was not found.");
            }

            // Standard operational guard: A session can only start if the table has been booked/occupied
            if (table.Status != "Occupied")
            {
                throw new InvalidOperationException($"Cannot start a dining session on table {table.TableNo} because its current status is '{table.Status}'.");
            }

            var newDinning = new DinningModel
            {
                TableId = tableId,
                StartedAt = DateTime.UtcNow,
                SessionStatus = "Active",
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            return await _dinningDLL.CreateDinningAsync(newDinning);
        }

        // FIX: Changed return type contract from DinningModel to int to match your interface
        public async Task<int> EndDinningSessionAsync(int sessionId)
        {
            // 1. Fetch the active dining session
            var getDinning = await _dinningDLL.GetDinningByIdAsync(sessionId);
            if (getDinning == null)
            {
                throw new KeyNotFoundException($"Dining session with ID {sessionId} was not found.");
            }

            if (getDinning.SessionStatus == "Closed")
            {
                throw new InvalidOperationException("This dining session is already closed.");
            }

            // 2. Fetch the associated physical table configuration
            var table = await _tableDLL.GetTableByIdAsync(getDinning.TableId);
            if (table == null)
            {
                throw new KeyNotFoundException($"The table associated with this session (TableID: {getDinning.TableId}) no longer exists.");
            }

            // 3. Mutate the session tracking properties
            getDinning.SessionStatus = "Closed"; // Consolidated casing format
            getDinning.EndAt = DateTime.UtcNow;
            getDinning.UpdatedAt = DateTime.UtcNow;

            // 4. Persist the session close down to the database
            var rowsAffected = await _dinningDLL.EndDinningSessionAsync(getDinning);

            if (rowsAffected > 0)
            {
                // 5. CRITICAL STEP: Sync table state back to "Cleaning" using our existing repository method
                var updateTableDto = new UpdateTable
                {
                    TableNo = table.TableNo,
                    Status = "Cleaning",
                    UpdatedBy = table.UpdatedBy // Cascade the tracking user context
                };

                await _tableDLL.UpdateTableAsync(updateTableDto);
            }

            return rowsAffected;
        }
    }
}