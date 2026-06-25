// ============================================
// BusinessLayer/Services.cs
// Business Logic + File Handling (receipts)
// ============================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MABCricketZone.DataLayer;

namespace MABCricketZone.BusinessLayer
{
    // ============================================
    // ABSTRACTION: Abstract Service base
    // ============================================
    public abstract class ServiceBase
    {
        protected string ServiceName;

        protected ServiceBase(string name)
        {
            ServiceName = name;
        }

        public abstract bool IsReady();

        public virtual string GetServiceInfo() => $"Service: {ServiceName}";
    }

    // ============================================
    // UserService - Business Logic for Users
    // ============================================
    public class UserService : ServiceBase
    {
        private readonly UserRepository _repo;
        public static User CurrentUser { get; private set; }

        // Constructor
        public UserService() : base("UserService")
        {
            _repo = new UserRepository();
        }

        public override bool IsReady()
        {
            var db = new DatabaseConnection();
            return db.TestConnection();
        }

        // Login - auto detects admin vs customer
        public User Login(string username, string password)
        {
            var user = _repo.Login(username, password);
            if (user != null)
            {
                CurrentUser = user;
            }
            return user;
        }

        public bool Register(string username, string password, string fullName,
                             string email, string phone, string address, string city)
        {
            if (_repo.UsernameExists(username)) return false;

            var user = new User(0, username, password, fullName, email, phone, address, city, false);
            return _repo.Add(user);
        }

        public List<User> GetAllUsers() => _repo.GetAll();

        public bool UpdateProfile(User user) => _repo.Update(user);

        public void Logout() => CurrentUser = null;
    }

    // ProductService - Business Logic for Products
    public class ProductService : ServiceBase
    {
        private readonly ProductRepository _repo;

        public ProductService() : base("ProductService")
        {
            _repo = new ProductRepository();
        }

        public override bool IsReady() => true;

        public List<Product> GetAll() => _repo.GetAll();

        public List<Product> GetByCategory(string category) => _repo.GetByCategory(category);

        public List<Product> Search(string keyword) => _repo.Search(keyword);

        public Product GetById(int id) => _repo.GetById(id);

        // Admin: Add product
        public bool AddProduct(string name, string brand, string category, string desc,
                               decimal price, int qty, string imagePath)
        {
            var p = new Product(0, name, brand, category, desc, price, qty, imagePath, true);
            return _repo.Add(p);
        }

        // Admin: Update product
        public bool UpdateProduct(Product p) => _repo.Update(p);

        // Admin: Delete product
        public bool DeleteProduct(int id) => _repo.Delete(id);

        // Reduce stock after purchase
        public bool ReduceStock(int productId, int soldQty)
        {
            var p = _repo.GetById(productId);
            if (p == null || p.Quantity < soldQty) return false;
            return _repo.UpdateQuantity(productId, p.Quantity - soldQty);
        }

        public List<string> GetCategories()
        {
            return new List<string>
            {
                "All", "Bats", "Balls", "Protection", "Gloves", "Footwear", "Accessories", "Training"
            };
        }
    }

    // ============================================
    // CartService - Shopping Cart Logic
    // ============================================
    public class CartService : ServiceBase
    {
        private readonly CartRepository _repo;

        public CartService() : base("CartService")
        {
            _repo = new CartRepository();
        }

        public override bool IsReady() => true;

        public bool AddToCart(int userId, int productId, int quantity)
        {
            var item = new CartItem(0, userId, productId, "", "", 0, quantity, "");
            return _repo.Add(item);
        }

        public List<CartItem> GetCart(int userId) => _repo.GetByUser(userId);

        public bool UpdateQuantity(int cartId, int qty)
        {
            var item = new CartItem { CartID = cartId, Quantity = qty };
            return _repo.Update(item);
        }

        public bool RemoveItem(int cartId) => _repo.Delete(cartId);

        public bool ClearCart(int userId) => _repo.ClearCart(userId);

        public decimal GetCartTotal(List<CartItem> items)
        {
            decimal total = 0;
            foreach (var item in items)
                total += item.GetSubtotal();
            return total;
        }

        public int GetCartCount(int userId)
        {
            return _repo.GetByUser(userId).Count;
        }
    }

    // ============================================
    // OrderService + FILE HANDLING for receipts
    // ============================================
    public class OrderService : ServiceBase
    {
        private readonly OrderRepository _orderRepo;
        private readonly CartRepository _cartRepo;
        private readonly ProductService _productService;
        private readonly string _receiptsFolder;

        // Exposes the last DB error so the UI can display it.
        public string LastError { get; private set; } = string.Empty;

        public OrderService() : base("OrderService")
        {
            _orderRepo = new OrderRepository();
            _cartRepo = new CartRepository();
            _productService = new ProductService();
            _receiptsFolder = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Receipts");
            Directory.CreateDirectory(_receiptsFolder);
        }

        public override bool IsReady() => true;

        // Place order from cart
        public Order PlaceOrder(int userId, string customerName, string address,
                                string phone, string city, string paymentMethod)
        {
            LastError = string.Empty;
            var cartItems = _cartRepo.GetByUser(userId);
            if (cartItems.Count == 0) { LastError = "Cart is empty."; return null; }

            var order = new Order
            {
                UserID = userId,
                CustomerName = customerName,
                DeliveryAddress = address,
                Phone = phone,
                City = city,
                PaymentMethod = paymentMethod,
                Items = cartItems,
                TotalAmount = 0
            };

            foreach (var item in cartItems)
                order.TotalAmount += item.GetSubtotal();

            bool saved = _orderRepo.Add(order);
            if (!saved) { LastError = _orderRepo.LastError; return null; }

            // Reduce stock for each item
            foreach (var item in cartItems)
                _productService.ReduceStock(item.ProductID, item.Quantity);

            // Clear cart
            _cartRepo.ClearCart(userId);

            // Generate receipt file
            GenerateReceipt(order);

            return order;
        }

        // FILE HANDLING: Write receipt to disk
        public string GenerateReceipt(Order order)
        {
            string fileName = $"Receipt_Order{order.OrderID}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(_receiptsFolder, fileName);

            var sb = new StringBuilder();
            sb.AppendLine("╔══════════════════════════════════════════════════════════╗");
            sb.AppendLine("║              MAB CRICKET ZONE - OFFICIAL RECEIPT         ║");
            sb.AppendLine("╚══════════════════════════════════════════════════════════╝");
            sb.AppendLine();
            sb.AppendLine($"  Order ID      : #{order.OrderID}");
            sb.AppendLine($"  Order Date    : {order.OrderDate:dd MMM yyyy, hh:mm tt}");
            sb.AppendLine($"  Status        : {order.Status}");
            sb.AppendLine();
            sb.AppendLine("──────────────── CUSTOMER DETAILS ────────────────────────");
            sb.AppendLine($"  Name          : {order.CustomerName}");
            sb.AppendLine($"  Phone         : {order.Phone}");
            sb.AppendLine($"  City          : {order.City}");
            sb.AppendLine($"  Address       : {order.DeliveryAddress}");
            sb.AppendLine($"  Payment       : {order.PaymentMethod}");
            sb.AppendLine();
            sb.AppendLine("──────────────── ORDER ITEMS ──────────────────────────────");
            sb.AppendLine($"  {"Item",-30} {"Qty",5} {"Unit Price",12} {"Subtotal",12}");
            sb.AppendLine($"  {"────",-30} {"───",5} {"──────────",12} {"───────",12}");

            foreach (var item in order.Items)
            {
                string itemName = $"{item.Brand} {item.ProductName}";
                if (itemName.Length > 28) itemName = itemName.Substring(0, 28);
                sb.AppendLine($"  {itemName,-30} {item.Quantity,5} Rs.{item.UnitPrice,9:N0} Rs.{item.GetSubtotal(),8:N0}");
            }

            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────────────────────────");
            sb.AppendLine($"  {"TOTAL AMOUNT:",-40} Rs. {order.TotalAmount:N0}");
            sb.AppendLine("──────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("  Thank you for shopping at MAB Cricket Zone!");
            sb.AppendLine("  Gear up. Play hard. Win big.");
            sb.AppendLine();
            sb.AppendLine("  Contact: mabcricketzone@gmail.com | 0300-1234567");
            sb.AppendLine("  Address: Main Market, Gulberg, Lahore, Pakistan");
            sb.AppendLine("╚══════════════════════════════════════════════════════════╝");

            // FILE HANDLING: Write to file
            File.WriteAllText(filePath, sb.ToString());

            return filePath;
        }

        // FILE HANDLING: Read receipt from disk
        public string ReadReceipt(string filePath)
        {
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);
            return "Receipt not found.";
        }

        public List<Order> GetAllOrders() => _orderRepo.GetAll();
        public List<Order> GetUserOrders(int userId) => _orderRepo.GetByUser(userId);
        public string GetReceiptsFolder() => _receiptsFolder;
    }
}
