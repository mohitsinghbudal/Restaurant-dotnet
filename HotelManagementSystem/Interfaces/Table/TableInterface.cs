using HotelManagementSystem.Models.Table;
using System.Threading.Tasks;

namespace HotelManagementSystem.Interfaces.TableInterface
{
    public interface ITableService
    {
        Task<TableModel> CreateTableAsync(CreateTable table, int userid);
        Task<bool> UpdateTableAsync(UpdateTable table, int tableId);
        Task<int> BookTableAsync(int table, int userId);
        Task<bool> FreeTableAsync(UpdateTable table);
        Task<bool> CleanTableAsync(CleanTable table);
        byte[] GenerateTableQRCode(int tableNo);
        public Task<TableModel> SeeTableInfo(int tableNo);
        Task<IEnumerable<TableModel>> GetAllTable();

        
        Task<IEnumerable<TableModel>> GetMyAllBookings(int userId);
        Task<bool> UpdateTableInfoAsync(TableModel table, int userid);
        Task<bool> DeleteTableAsync(int tableid, int userid);
        Task<TableModel?> GetAssignedTableAsync(int userId);
    }

    public interface ITableDLL
    {
        Task<TableModel> CreateTableAsync(TableModel table);
        Task<bool> UpdateTableAsync(UpdateTable table);
        Task<TableModel> GetTableByTableNoAsync(int tableNo);
        Task<TableModel> GetTableByIdAsync(int tableId);

        
        Task<int> BookTableAsync(TableModel table);

        Task<TableModel> GetTableByNo(int Id);
        Task<IEnumerable<TableModel>> GetAllTable();

        
        Task<IEnumerable<TableModel>> GetMyAllBookings(int userId);
        Task<bool> UpdateTableInfoAsync(TableModel table);
        Task<bool> DeleteTableAsync(int tableid, int userid);
        Task<TableModel?> GetAssignedTableAsync(int userId);
    }
}
