using HotelManagementSystem.Models.Table;

namespace HotelManagementSystem.Interfaces.TableInterface
{
    public interface ITableService
    {
        Task<TableModel> CreateTableAsync(TableModel table);
        Task<int> UpdateTableAsync(UpdateTable table);
        Task<int> BookTableAsync(UpdateTable table);
        Task<int> FreeTableAsync(UpdateTable table);
        Task<int> CleanTableAsync(UpdateTable table);
        string GenerateTableQRCode(int tableNo, int updatedById);
    }
    public interface ITableDLL
    {
        Task<TableModel> CreateTableAsync(TableModel table);
        Task<int> UpdateTableAsync(UpdateTable table);
        Task<TableModel> GetTableByTableNoAsync(int tableNo);
        Task<TableModel> GetTableByIdAsync(int tableId);
        Task<int> BookTableAsync(TableModel table);

    }
}
