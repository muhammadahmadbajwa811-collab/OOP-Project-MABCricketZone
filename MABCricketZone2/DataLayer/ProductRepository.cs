// ============================================
// DataLayer/ProductRepository.cs
// ============================================
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MABCricketZone.BusinessLayer;

namespace MABCricketZone.DataLayer
{
    public class ProductRepository : RepositoryBase<Product>
    {
        public ProductRepository() { }

        public override bool Add(Product p)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string q = @"INSERT INTO Products (Name,Brand,Category,Description,Price,Quantity,ImagePath) 
                                 VALUES (@n,@b,@c,@d,@pr,@q,@ip)";
                    var cmd = new MySqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@n", p.Name);
                    cmd.Parameters.AddWithValue("@b", p.Brand);
                    cmd.Parameters.AddWithValue("@c", p.Category);
                    cmd.Parameters.AddWithValue("@d", p.Description);
                    cmd.Parameters.AddWithValue("@pr", p.Price);
                    cmd.Parameters.AddWithValue("@q", p.Quantity);
                    cmd.Parameters.AddWithValue("@ip", p.ImagePath);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }

        public override bool Update(Product p)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string q = @"UPDATE Products SET Name=@n,Brand=@b,Category=@c,Description=@d,
                                 Price=@pr,Quantity=@q,ImagePath=@ip,IsAvailable=@ia WHERE ProductID=@id";
                    var cmd = new MySqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@n", p.Name);
                    cmd.Parameters.AddWithValue("@b", p.Brand);
                    cmd.Parameters.AddWithValue("@c", p.Category);
                    cmd.Parameters.AddWithValue("@d", p.Description);
                    cmd.Parameters.AddWithValue("@pr", p.Price);
                    cmd.Parameters.AddWithValue("@q", p.Quantity);
                    cmd.Parameters.AddWithValue("@ip", p.ImagePath);
                    cmd.Parameters.AddWithValue("@ia", p.IsAvailable ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", p.ProductID);
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
                    var cmd = new MySqlCommand("DELETE FROM Products WHERE ProductID=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }

        public override Product GetById(int id)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM Products WHERE ProductID=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    var r = cmd.ExecuteReader();
                    if (r.Read()) return MapProduct(r);
                }
            }
            catch { }
            return null;
        }

        public override List<Product> GetAll()
        {
            var list = new List<Product>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM Products ORDER BY Category, Name", conn);
                    var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(MapProduct(r));
                }
            }
            catch { }
            return list;
        }

        public List<Product> GetByCategory(string category)
        {
            var list = new List<Product>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "SELECT * FROM Products WHERE Category=@c ORDER BY Name", conn);
                    cmd.Parameters.AddWithValue("@c", category);
                    var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(MapProduct(r));
                }
            }
            catch { }
            return list;
        }

        public List<Product> Search(string keyword)
        {
            var list = new List<Product>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "SELECT * FROM Products WHERE Name LIKE @k OR Brand LIKE @k OR Category LIKE @k", conn);
                    cmd.Parameters.AddWithValue("@k", $"%{keyword}%");
                    var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(MapProduct(r));
                }
            }
            catch { }
            return list;
        }

        public bool UpdateQuantity(int productId, int newQty)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE Products SET Quantity=@q WHERE ProductID=@id", conn);
                    cmd.Parameters.AddWithValue("@q", newQty);
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }

        private Product MapProduct(MySqlDataReader r)
        {
            return new Product(
                Convert.ToInt32(r["ProductID"]),
                r["Name"].ToString(),
                r["Brand"].ToString(),
                r["Category"].ToString(),
                r["Description"].ToString(),
                Convert.ToDecimal(r["Price"]),
                Convert.ToInt32(r["Quantity"]),
                r["ImagePath"].ToString(),
                Convert.ToInt32(r["IsAvailable"]) == 1
            );
        }
    }
}
