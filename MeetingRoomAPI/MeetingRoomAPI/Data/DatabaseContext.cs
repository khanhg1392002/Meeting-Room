using MySql.Data.MySqlClient;
using System.Data;

namespace MeetingRoomAPI.Data
{
    public class DatabaseContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString; // Chuỗi kết nối tới MySQL
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}