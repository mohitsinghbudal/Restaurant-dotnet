using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.Dinning;
using HotelManagementSystem.Models.Table;
using QRCoder;
using System.Drawing; // Note: For .NET 6/7/8+ on Linux, use a cross-platform drawing lib like 'SixLabors.ImageSharp'


namespace HotelManagementSystem.Services.Table
{
    public class TableService : ITableService
    {
        private readonly ITableDLL _table;
        private readonly IUserDLL _userDLL;

        public TableService(ITableDLL table, IUserDLL userDLL) 
        {
            _table = table;
            _userDLL = userDLL;
        }

        public async Task<TableModel> CreateTableAsync(TableModel table)
        {
            var newTable = new TableModel
            {
                TableNo = table.TableNo,
                Capacity = table.Capacity,
                Status = "Available"
            };
            return await _table.CreateTableAsync(newTable);
        }
        public async Task<int> UpdateTableAsync(UpdateTable table)
        {
            var newTable = new UpdateTable
            {
                TableNo = table.TableNo,
                UpdatedBy = table.UpdatedBy,
                UpdatedAt = DateTime.UtcNow,
                Status = table.Status
            };
            return await _table.UpdateTableAsync(newTable);

        }


        public async Task<int> BookTableAsync(UpdateTable table)
        {
            // 1. Fetch the existing table
            var existingTable = await _table.GetTableByTableNoAsync(table.TableNo);

            // FIX: Check if the table even exists first!
            if (existingTable == null)
            {
                throw new Exception($"Table number {table.TableNo} does not exist.");
            }

            // 2. Validate Statuses
            if (existingTable.Status == "Occupied")
            {
                throw new Exception("Table is already occupied");
            }
            if (existingTable.Status == "Cleaning")
            {
                throw new Exception("Table is currently cleaning");
            }

            // 3. Automatically get the best waiter
            var getWaiter = await _userDLL.AssignWaiterAsync();

            Console.WriteLine($"Assigned Waiter: {getWaiter}");

            // FIX: Modify the existing object properties instead of instantiating a brand new one.
            // This preserves all other columns (like Table ID, capacity, etc.) in the database.
            existingTable.Status = "Occupied";
            existingTable.WaiterId = getWaiter;
            existingTable.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            existingTable.CreatedBy = table.CreatedBy;
            existingTable.UpdatedBy = table.CreatedBy;


            // 4. Save updates back to the database
            var booktable = await _table.BookTableAsync(existingTable);

            return booktable;
        }
        public async Task<int> FreeTableAsync(UpdateTable table)
        {
            var existingTable = await _table.GetTableByTableNoAsync(table.TableNo);
            Console.WriteLine(existingTable.TableNo);

            if (existingTable.Status == "Available")
            {
                throw new Exception("Table is already available");
            }
            if(existingTable.Status == "Occupied")
            {
                throw new Exception("Sorry table is Occupied");
            }

            var newTable = new UpdateTable
            {
                TableNo = table.TableNo,
                UpdatedBy = table.UpdatedBy,
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                Status = "Available",
                WaiterId = null
            };

            Console.WriteLine(newTable.Status);
            Console.WriteLine(newTable.UpdatedBy);
            Console.WriteLine(newTable.WaiterId);
            return await _table.UpdateTableAsync(newTable);

        }
        public async Task<int> CleanTableAsync(UpdateTable table)
        {
            var existingTable = await _table.GetTableByTableNoAsync(table.TableNo);

            if (existingTable.Status == "Available")
            {
                throw new Exception("Table is already Cleaned");
            }
            if(existingTable.Status == "Cleanning")
            {
                throw new Exception("Table is Cleaning");
            }
                
            var newTable = new UpdateTable
            {
                TableNo = table.TableNo,
                UpdatedBy = table.UpdatedBy,
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                Status = "Cleanning"
            };
            Console.WriteLine(newTable.UpdatedAt);
            return await _table.UpdateTableAsync(newTable);

        }
        
            public string GenerateTableQRCode(int tableNo, int updatedById)
                {
                        // 1. Construct the payload (this is what is encoded in the QR)
                        // Example: A JSON string containing the data
                    string payload = $"TableNo:{tableNo}|UpdatedBy:{updatedById}|Timestamp:{DateTime.UtcNow}";

                    // 2. Generate the QR Code
                    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                    {
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                        PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

                        // 3. Convert to byte array
                        byte[] qrCodeImage = qrCode.GetGraphic(20);

                        // 4. Return as Base64 string to send to the frontend
                        return Convert.ToBase64String(qrCodeImage);
                    }
                }
}
}
