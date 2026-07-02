using System.Data;

namespace HotelManagementSystem.Interfaces.DatabaseConnection
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
