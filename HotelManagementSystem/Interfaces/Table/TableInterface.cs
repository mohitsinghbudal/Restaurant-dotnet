using HotelManagementSystem.Models.Table;
using System.Threading.Tasks;

namespace HotelManagementSystem.Interfaces.TableInterface
{
    public interface ITableService
    {
        Task<TableModel> CreateTableAsync(CreateTable table);
        Task<int> UpdateTableAsync(UpdateTable table);
        Task<int> BookTableAsync(BookTable table, int userId);
        Task<int> FreeTableAsync(UpdateTable table);
        Task<int> CleanTableAsync(CleanTable table);
        byte[] GenerateTableQRCode(int tableNo, int updatedById);
    }

    public interface ITableDLL
    {
        Task<TableModel> CreateTableAsync(TableModel table);
        Task<int> UpdateTableAsync(UpdateTable table);
        Task<TableModel> GetTableByTableNoAsync(int tableNo);
        Task<TableModel> GetTableByIdAsync(int tableId);

        // Match the model signature used in your TableDLL implementation
        Task<int> BookTableAsync(TableModel table);

        Task<TableModel> GetTableByNo(int Id);
    }
}