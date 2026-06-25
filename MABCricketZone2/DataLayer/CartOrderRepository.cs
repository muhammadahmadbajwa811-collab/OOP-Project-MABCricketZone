// ============================================
// DataLayer/CartRepository.cs
// ============================================
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MABCricketZone.BusinessLayer;

namespace MABCricketZone.DataLayer
{
    public class CartRepository : RepositoryBase<CartItem>
    {
        public CartRepository() { }

        public override bool Add(CartItem item)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // Check if already in cart
                    var check = new MySqlCommand(
                        "SELECT CartID FROM Cart WHERE UserID=@u AND ProductID=@p", conn);
                    check.Parameters.AddWithValue("@u", item.UserID);
                    check.Parameters.AddWithValue("@p", item.ProductID);
                    var existing = check.ExecuteScalar();
                    if (existing != null)
                    {
                        var upd = new MySqlCommand(
                            "UPDATE Cart SET Quantity=Quantity+@q WHERE CartID=@id", conn);
                        upd.Parameters.AddWithValue("@q", item.Quantity);
                        upd.Parameters.AddWithValue("@id", existing);
                        upd.ExecuteNonQuery();
                    }
                    else
                    {
                        var cmd = new MySqlCommand(
                            "INSERT INTO Cart(UserID,ProductID,Quantity) VALUES(@u,@p,@q)", conn);
                        cmd.Parameters.AddWithValue("@u", item.UserID);
                        cmd.Parameters.AddWithValue("@p", item.ProductID);
                        cmd.Parameters.AddWithValue("@q", item.Quantity);
                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch { return false; }
        }

        public override bool Update(CartItem item)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE Cart SET Quantity=@q WHERE CartID=@id", conn);
                    cmd.Parameters.AddWithValue("@q", item.Quantity);
                    cmd.Parameters.AddWithValue("@id", item.CartID);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }

        public override bool Delete(int cartId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM Cart WHERE CartID=@id", conn);
                    cmd.Parameters.AddWithValue("@id", cartId);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }

        public override CartItem GetById(int id) => null;

        public override List<CartItem> GetAll() => new List<CartItem>();

        public List<CartItem> GetByUser(int userId)
        {
            var list = new List<CartItem>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string q = @"SELECT c.CartID, c.UserID, c.ProductID, c.Quantity,
                                        p.Name, p.Brand, p.Price, p.ImagePath
                                 FROM Cart c 
                                 JOIN Products p ON c.ProductID = p.ProductID
                                 WHERE c.UserID=@u";
                    var cmd = new MySqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@u", userId);
                    var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        list.Add(new CartItem(
                            Convert.ToInt32(r["CartID"]),
                            userId,
                            Convert.ToInt32(r["ProductID"]),
                            r["Name"].ToString(),
                            r["Brand"].ToString(),
                            Convert.ToDecimal(r["Price"]),
                            Convert.ToInt32(r["Quantity"]),
                            r["ImagePath"].ToString()
                        ));
                    }
                }
            }
            catch { }
            return list;
        }

        public bool ClearCart(int userId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM Cart WHERE UserID=@u", conn);
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }
    }

    // ============================================
    // DataLayer/OrderRepository.cs
    // ============================================
    public class OrderRepository : RepositoryBase<Order>
    {
        public OrderRepository() { }

        // Stores the last error so callers can show a meaningful message.
        public string LastError { get; private set; } = string.Empty;

        public override bool Add(Order order)
        {
            LastError = string.Empty;
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    // Use a transaction: if any OrderItem insert fails, the whole order is rolled back.
                    using var tx = conn.BeginTransaction();

                    string q = @"INSERT INTO Orders (UserID,TotalAmount,PaymentMethod,CustomerName,
                                  DeliveryAddress,Phone,City,Status) VALUES(@u,@t,@pm,@cn,@da,@ph,@ci,@st)";
                    var cmd = new MySqlCommand(q, conn, tx);
                    cmd.Parameters.AddWithValue("@u", order.UserID);
                    cmd.Parameters.AddWithValue("@t", order.TotalAmount);
                    cmd.Parameters.AddWithValue("@pm", order.PaymentMethod);
                    cmd.Parameters.AddWithValue("@cn", order.CustomerName);
                    cmd.Parameters.AddWithValue("@da", order.DeliveryAddress);
                    cmd.Parameters.AddWithValue("@ph", order.Phone);
                    cmd.Parameters.AddWithValue("@ci", order.City);
                    cmd.Parameters.AddWithValue("@st", order.Status);
                    cmd.ExecuteNonQuery();
                    order.OrderID = Convert.ToInt32(cmd.LastInsertedId);

                    foreach (var item in order.Items)
                    {
                        string qi = @"INSERT INTO OrderItems(OrderID,ProductID,ProductName,Brand,Quantity,UnitPrice)
                                      VALUES(@oid,@pid,@pn,@br,@q,@up)";
                        var ci = new MySqlCommand(qi, conn, tx);
                        ci.Parameters.AddWithValue("@oid", order.OrderID);
                        ci.Parameters.AddWithValue("@pid", item.ProductID);
                        ci.Parameters.AddWithValue("@pn", item.ProductName);
                        ci.Parameters.AddWithValue("@br", item.Brand);
                        ci.Parameters.AddWithValue("@q", item.Quantity);
                        ci.Parameters.AddWithValue("@up", item.UnitPrice);
                        ci.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public override bool Update(Order o) => false;
        public override bool Delete(int id) => false;
        public override Order GetById(int id) => null;

        public override List<Order> GetAll()
        {
            var list = new List<Order>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string q = @"SELECT o.*, u.Username FROM Orders o 
                                 JOIN Users u ON o.UserID=u.UserID
                                 ORDER BY o.OrderDate DESC";
                    var cmd = new MySqlCommand(q, conn);
                    var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        list.Add(new Order(
                            Convert.ToInt32(r["OrderID"]),
                            Convert.ToInt32(r["UserID"]),
                            r["Username"].ToString(),
                            Convert.ToDecimal(r["TotalAmount"]),
                            r["PaymentMethod"].ToString(),
                            r["CustomerName"].ToString(),
                            r["DeliveryAddress"].ToString(),
                            r["Phone"].ToString(),
                            r["City"].ToString(),
                            r["Status"].ToString(),
                            Convert.ToDateTime(r["OrderDate"])
                        ));
                    }
                }
            }
            catch { }
            return list;
        }

        public List<Order> GetByUser(int userId)
        {
            var list = new List<Order>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "SELECT * FROM Orders WHERE UserID=@u ORDER BY OrderDate DESC", conn);
                    cmd.Parameters.AddWithValue("@u", userId);
                    var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        list.Add(new Order(
                            Convert.ToInt32(r["OrderID"]),
                            userId, "",
                            Convert.ToDecimal(r["TotalAmount"]),
                            r["PaymentMethod"].ToString(),
                            r["CustomerName"].ToString(),
                            r["DeliveryAddress"].ToString(),
                            r["Phone"].ToString(),
                            r["City"].ToString(),
                            r["Status"].ToString(),
                            Convert.ToDateTime(r["OrderDate"])
                        ));
                    }
                }
            }
            catch { }
            return list;
        }
    }
}
