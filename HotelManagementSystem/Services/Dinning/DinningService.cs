using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Models.Dinning;
using HotelManagementSystem.Models.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace HotelManagementSystem.Services.Dinning
{
    public class DinningService : IDinningService
    {
        private readonly IDinningDLL _dinningDLL;
        private readonly ITableDLL _tableDLL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DinningService(IDinningDLL dinningDLL, ITableDLL tableDLL, IHttpContextAccessor httpContextAccessor )
        {
            _dinningDLL = dinningDLL;
            _tableDLL = tableDLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> GetDiningSession(int userId)
        {
            return await _dinningDLL.GetDiningSession( userId);
        }

        public async Task<int> CreateDinningAsync(int tableId, int userId)
        {
            var table = await _tableDLL.GetTableByIdAsync(tableId);

            
            if (table == null)
            {
                throw new KeyNotFoundException($"Table with ID {tableId} was not found.");
            }

            
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

        
        public async Task<int> EndDinningSessionAsync(int sessionId)
        {
            var claimPrincipal = _httpContextAccessor.HttpContext?.User;
            if (claimPrincipal == null)
            {
                throw new Exception("user is not allowed");
            }
            int userId = ClaimHelper.GetUserId(claimPrincipal);

            Console.WriteLine(userId);
            
            var getDinning = await _dinningDLL.GetDinningByIdAsync(sessionId);
            if (getDinning == null)
            {
                throw new KeyNotFoundException($"Dining session with ID {sessionId} was not found.");
            }

            if (getDinning.SessionStatus == "Closed")
            {
                throw new InvalidOperationException("This dining session is already closed.");
            }

            
            var table = await _tableDLL.GetTableByIdAsync(getDinning.TableId);
            if (table == null)
            {
                throw new KeyNotFoundException($"The table associated with this session (TableID: {getDinning.TableId}) no longer exists.");
            }

            
            getDinning.SessionStatus = "Closed"; 
            getDinning.EndAt = DateTime.UtcNow;
            getDinning.UpdatedAt = DateTime.UtcNow;
            getDinning.EndedBy = userId;

            
            var rowsAffected = await _dinningDLL.EndDinningSessionAsync(getDinning);

            if (rowsAffected > 0)
            {
                
                var updateTableDto = new UpdateTable
                {
                    TableNo = table.TableNo,
                    Status = "Cleaning",
                    UpdatedBy = table.UpdatedBy 
                };

                await _tableDLL.UpdateTableAsync(updateTableDto);
            }

            return rowsAffected;
        }

        public async Task<IEnumerable<DinningModel>> GetAllDinningSessions()
        {
            return await _dinningDLL.GetAllDinningSessions();
        }
        public async Task<int?> GetMySessionId(int userId)
        {
            // IDinningDLL does not define GetMySessionId; use existing GetDiningSession which returns int.
            var sessionId = await _dinningDLL.GetDiningSession(userId);
            return sessionId;
        }
        public async Task<int> GetCustomerId(int tableId)
        {
            return await _dinningDLL.GetCustomerId(tableId);
        }
    }
}
