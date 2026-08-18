using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.Table;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TableService(ITableDLL table, IUserDLL userDLL, IDinningService din, IHttpContextAccessor httpContextAccessor)
        {
            _table = table;
            _userDLL = userDLL;
            _din = din;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
      
        public async Task<IEnumerable<TableModel>> GetMyAllBookings(int userId)
        {
            return await _table.GetMyAllBookings(userId);
        }
        public async Task<TableModel> CreateTableAsync(CreateTable table, int userId)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));

            var existingTable = await _table.GetTableByTableNoAsync(table.TableNo);

            if (existingTable != null && existingTable.TableNo == table.TableNo)
            {
                throw new InvalidOperationException($"Table number {table.TableNo} already exists.");
            }

            var newTable = new TableModel
            {
                TableNo = table.TableNo,
                Capacity = table.Capacity,
                Status = "Available",
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            return await _table.CreateTableAsync(newTable);
        }

        public async Task<bool> UpdateTableAsync(UpdateTable table,int tableId)
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            if (user == null) throw new InvalidOperationException("Unable to determine current user from HttpContext.");

            int userId = ClaimHelper.GetUserId(user);

            if (!user.IsInRole("Admin")) throw new Exception("User not allowed");

            if (table == null) throw new ArgumentNullException(nameof(table));
            var existingTable = await _table.GetTableByTableNoAsync(table.TableNo);

            if(existingTable!=null && existingTable.TableId != tableId)
            {
                throw new Exception("table no already exists");
            }
            return await _table.UpdateTableAsync(table);
        }

        public async Task<int> BookTableAsync(int tableNo , int userId)
        {
            if (tableNo <=0) throw new ArgumentNullException(nameof(tableNo));

            var existingTable = await _table.GetTableByTableNoAsync(tableNo);
            var allTables = await _table.GetAllTable();

            foreach(var user in allTables)
            {
                if (user.UpdatedBy == userId)
                    throw new Exception("User has already a booking");
            }


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

            
            existingTable.Status = "Occupied";
            existingTable.UpdatedBy = userId;
            existingTable.UpdatedAt = DateTime.UtcNow;

            
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

        public async Task<bool> FreeTableAsync(UpdateTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));

            var existingTable = await _table.GetTableByTableNoAsync(table.TableNo);

            if (existingTable == null)
            {
                throw new KeyNotFoundException($"Table number {table.TableNo} does not exist.");
            }

            if (existingTable.Status == "Available")
            {
                throw new InvalidOperationException("Table is already available.");
            }
            if (existingTable.Status != "Cleaning")
            {  
                throw new InvalidOperationException($"Cannot free a table that is currently '{existingTable.Status}'. It must be Cleaned first.");
            }

            
            var updatedData = new UpdateTable
            {
                TableNo = table.TableNo,
                
                Status = "Available" 
            };

            return await _table.UpdateTableAsync(updatedData);
        }

        public async Task<bool> CleanTableAsync(CleanTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));

            var existingTable = await _table.GetTableByTableNoAsync(table.tableno);

            if (existingTable == null)
            {
                throw new KeyNotFoundException($"Table number {table.tableno} does not exist.");
            }

            
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
                
                Status = "Cleaning" 
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

        public async Task<bool> UpdateTableInfoAsync(TableModel table, int userId)
        {
            var existingTable = await _table.GetTableByIdAsync(table.TableId);

            if (existingTable == null) throw new Exception("table doesnot exits");

            if (existingTable.TableNo != table.TableNo)
            {
                var checktableno = await _table.GetTableByNo(table.TableNo);

                if (checktableno != null && checktableno.TableId != existingTable.TableId )
                {
                    throw new Exception("table no already exists.");
                }
                existingTable.TableNo = table.TableNo;
            }
           
            
            existingTable.Status = table.Status;
            existingTable.IsActive = table.IsActive;
            existingTable.UpdatedAt = DateTime.UtcNow;
            existingTable.Capacity = table.Capacity;
            existingTable.UpdatedBy = userId;

            return await _table.UpdateTableInfoAsync(existingTable);

        }
        public async Task<bool> DeleteTableAsync(int tableId, int userid)
        {
            return await _table.DeleteTableAsync(tableId, userid);
        }
        public async Task<TableModel?> GetAssignedTableAsync(int userId)
        {
            return await _table.GetAssignedTableAsync(userId);
        }
    }
}
