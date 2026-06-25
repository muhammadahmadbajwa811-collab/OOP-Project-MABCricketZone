// ============================================
// DataLayer/UserRepository.cs
// ============================================
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MABCricketZone.BusinessLayer;

namespace MABCricketZone.DataLayer
{
    // Abstraction: Abstract CRUD interface
    public abstract class RepositoryBase<T>
    {
        public abstract bool Add(T entity);
        public abstract bool Update(T entity);
        public abstract bool Delete(int id);
        public abstract T GetById(int id);
        public abstract List<T> GetAll();
    }

    public class UserRepository : RepositoryBase<User>
    {
        // Constructor
        public UserRepository() { }

        public override bool Add(User user)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Users (Username, PasswordHash, FullName, Email, Phone, Address, City, IsAdmin) 
                                     VALUES (@u, @p, @fn, @em, @ph, @ad, @ci, @ia)";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", user.Username);
                    cmd.Parameters.AddWithValue("@p", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@fn", user.FullName);
                    cmd.Parameters.AddWithValue("@em", user.Email);
                    cmd.Parameters.AddWithValue("@ph", user.Phone);
                    cmd.Parameters.AddWithValue("@ad", user.Address);
                    cmd.Parameters.AddWithValue("@ci", user.City);
                    cmd.Parameters.AddWithValue("@ia", user.IsAdmin ? 1 : 0);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }

        public override bool Update(User user)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Users SET FullName=@fn, Email=@em, Phone=@ph, 
                                     Address=@ad, City=@ci WHERE UserID=@id";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@fn", user.FullName);
                    cmd.Parameters.AddWithValue("@em", user.Email);
                    cmd.Parameters.AddWithValue("@ph", user.Phone);
                    cmd.Parameters.AddWithValue("@ad", user.Address);
                    cmd.Parameters.AddWithValue("@ci", user.City);
                    cmd.Parameters.AddWithValue("@id", user.UserID);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }

        public override bool Delete(int id)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM Users WHERE UserID=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }

        public override User GetById(int id)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM Users WHERE UserID=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();
                    if (reader.Read()) return MapUser(reader);
                }
            }
            catch { }
            return null;
        }

        public override List<User> GetAll()
        {
            var list = new List<User>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM Users ORDER BY CreatedAt DESC", conn);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read()) list.Add(MapUser(reader));
                }
            }
            catch { }
            return list;
        }

        // Login: returns User or null
        public User Login(string username, string password)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "SELECT * FROM Users WHERE Username=@u AND PasswordHash=@p", conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);
                    var reader = cmd.ExecuteReader();
                    if (reader.Read()) return MapUser(reader);
                }
            }
            catch { }
            return null;
        }

        public bool UsernameExists(string username)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u", conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
            catch { return false; }
        }

        private User MapUser(MySqlDataReader r)
        {
            return new User(
                Convert.ToInt32(r["UserID"]),
                r["Username"].ToString(),
                r["PasswordHash"].ToString(),
                r["FullName"].ToString(),
                r["Email"].ToString(),
                r["Phone"].ToString(),
                r["Address"].ToString(),
                r["City"].ToString(),
                Convert.ToInt32(r["IsAdmin"]) == 1
            );
        }
    }
}
