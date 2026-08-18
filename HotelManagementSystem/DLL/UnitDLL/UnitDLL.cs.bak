using Dapper;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.Units;
using HotelManagementSystem.Models.Units;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HotelManagementSystem.DLL.UnitDLL
{
    public class UnitDLL : IUnitDLL
    {
        private readonly IDbConnectionFactory _dbConn;

        public UnitDLL(IDbConnectionFactory dbConn)
        {
            _dbConn = dbConn;
        }

        // FIX: Changed return type to IEnumerable<Units> to retrieve the full list of units
        public async Task<IEnumerable<Unit>> GetAllUnitsAsync()
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"SELECT * 
                           FROM Units 
                           WHERE IsActive = 1;";

            return await conn.QueryAsync<Unit>(sql);
        }

        // FIX: Replaced 'Task task' with your 'Units unit' model payload
        public async Task<int> AddUnitAsync(Unit unit)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
        INSERT INTO Units
        (
            UnitName,
            [Description],
            ShortName,
            IsActive
        )
        VALUES
        (
            @UnitName,
            @Description,
            @ShortName,
            @IsActive
        );";

            return await conn.ExecuteAsync(sql, unit);
        }

        // FIX: Replaced 'Task task' with 'Units unit' and corrected method casing to UpdateUnitAsync
        public async Task<int> UpdateUnitAsync(Unit unit)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"UPDATE Units 
                           SET UnitName = @UnitName, 
                               [Description] = @Description, 
                               ShortName = @ShortName, 
                               IsActive = @IsActive
                           WHERE UnitId = @UnitId;";

            return await conn.ExecuteAsync(sql, unit);
        }
        public async Task<int> DeleteUnitAsync(int unitId)
        {
            using var conn = _dbConn.CreateConnection();

            string sql = @"
        UPDATE Units
        SET IsActive = 0
        WHERE UnitId = @UnitId;";

            return await conn.ExecuteAsync(sql, new { UnitId = unitId });
        }
    }
}