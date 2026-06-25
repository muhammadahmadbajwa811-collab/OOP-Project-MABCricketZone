// ============================================
// DataLayer/DatabaseConnection.cs
// ============================================
using System;
using MySql.Data.MySqlClient;

namespace MABCricketZone.DataLayer
{
    // Abstraction: Abstract base class for DB operations
    public abstract class DatabaseBase
    {
        protected static string ConnectionString = 
            "Server=localhost;Database=MABCricketZone;Uid=root;Pwd=;";

        public static void SetConnectionString(string server, string db, string user, string pass)
        {
            ConnectionString = $"Server={server};Database={db};Uid={user};Pwd={pass};";
        }

        // Abstract method - polymorphism
        public abstract bool TestConnection();
    }

    public class DatabaseConnection : DatabaseBase
    {
        // Constructor
        public DatabaseConnection() { }

        public DatabaseConnection(string server, string database, string user, string password)
        {
            SetConnectionString(server, database, user, password);
        }

        public override bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch { return false; }
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
