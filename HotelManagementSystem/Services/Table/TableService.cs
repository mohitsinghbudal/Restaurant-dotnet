using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.Table;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Threading.Tasks;

namespace HotelManagementSystem.Services.Table
{
    public class TableService : ITableService
    {
        private readonly ITableDLL _table;
        private readonly IUserDLL _userDLL;
        private readonly IDinningService _din;

        public TableService(ITableDLL table, IUserDLL userDLL, IDinningService din)
        {
            _table = table;
            _userDLL = userDLL;
            _din = din;
        }
        public async Task<TableModel> GetMyBookings(int userId)
        {
            return await _table.GetMyBookings(userId);
        }
        public async Task<IEnumerable<TableModel>> GetMyAllBookings(int userId)
        {
            return await _table.GetMyAllBookings(userId);
        }
        public async Task<TableModel> CreateTableAsync(CreateTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));

            var existingTable = await _table.GetTableByTableNoAsync(table.TableNo);

            if (existingTable != null && existingTable.TableNo > 0)
            {
                throw new InvalidOperationException($"Table number {table.TableNo} already exists.");
            }

            var newTable = new TableModel
            {
                TableNo = table.TableNo,
                Capacity = table.Capacity,
                Status = "Available",
                CreatedBy = table.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            return await _table.CreateTableAsync(newTable);
        }

        public async Task<int> UpdateTableAsync(UpdateTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            return await _table.UpdateTableAsync(table);
        }

        public async Task<int> BookTableAsync(int tableNo , int userId)
        {
            if (tableNo <=0) throw new ArgumentNullException(nameof(tableNo));

            var existingTable = await _table.GetTableByTableNoAsync(tableNo);

            if (existingTable == null)
            {
                throw new KeyNotFoundException($"Table number {tableNo} does not exist.");
            }

            if (existingTable.Status == "Occupied")
            {
                throw new InvalidOperationException("Table is already occupied.");
            }
            if (existingTable.Status == "Cleaning")
            {
                throw new InvalidOperationException("Table is currently being cleaned.");
            }

            // 1. Double Assignment Protection: Handled internally inside TableDLL via AssignWaiterAsync
            existingTable.Status = "Occupied";
            existingTable.UpdatedBy = userId;
            existingTable.UpdatedAt = DateTime.UtcNow;

            // 2. Delegate to DLL which safely assigns the workload-based waiter
            var table = await _table.BookTableAsync(existingTable);

            if (table <=0)
            {
                throw new Exception("error in table");
            }

            var sessionId = await _din.CreateDinningAsync(existingTable.TableId, userId);

            if (sessionId <=0)
            {
                return 0;
            }

            return sessionId;
        }

        public async Task<int> FreeTableAsync(UpdateTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));

            var existingTable = await _table.GetTableByTableNoAsync(table.TableNo);

            if (existingTable == null)
            {
                throw new KeyNotFoundException($"Table number {table.TableNo} does not exist.");
            }

            // FIX: Logical inversion. To FREE a table, it MUST be occupied. 
            if (existingTable.Status == "Occupied")
            {
                throw new InvalidOperationException("Table is already available.");
            }
            if (existingTable.Status != "Cleaning")
            {  
                throw new InvalidOperationException($"Cannot free a table that is currently '{existingTable.Status}'. It must be Cleaned first.");
            }

            // Instead of instantiating a raw UpdateTable DTO manually, map it cleanly
            var updatedData = new UpdateTable
            {
                TableNo = table.TableNo,
                UpdatedBy = table.UpdatedBy,
                Status = "Available" // Standard Restaurant Workflow: Available -> Occupied -> Cleaning -> 
            };

            return await _table.UpdateTableAsync(updatedData);
        }

        public async Task<int> CleanTableAsync(CleanTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));

            var existingTable = await _table.GetTableByTableNoAsync(table.tableno);

            if (existingTable == null)
            {
                throw new KeyNotFoundException($"Table number {table.tableno} does not exist.");
            }

            // FIX: Typo matching ("Cleaning" vs "Cleanning")   
            if (existingTable.Status == "Available")
            {
                throw new InvalidOperationException("Table is already cleaned and available.");
            }
            if (existingTable.Status == "Cleaning")
            {
                throw new InvalidOperationException("Table is already in the cleaning process.");
            }
            if(existingTable.Status != "Occupied")
            {
                throw new InvalidOperationException($"Cannot free a table that is currently '{existingTable.Status}'. It must be Occupied first.");
            }

            var updatedData = new UpdateTable
            {
                TableNo = table.tableno,
                UpdatedBy = table.updatedby,
                Status = "Cleaning" // Once cleaning is done, it transitions back to Available
            };

            return await _table.UpdateTableAsync(updatedData);
        }

       
        public byte[] GenerateTableQRCode(int tableNo)
        {
            string payload = $"https://localhost:7186/api/Table/get-table-info";
        

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

                return qrCode.GetGraphic(20);
            }
        }

        public  Task<TableModel> SeeTableInfo(int tableNo)
        {
            return  _table.GetTableByNo(tableNo);
        }

        public async Task<IEnumerable<TableModel>> GetAllTable()
        {
            return await _table.GetAllTable();
        }
    }
}