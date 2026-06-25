// BusinessLayer/Models.cs
// All OOP concepts: Classes, Constructors, Inheritance, Polymorphism, Abstraction

using System;
using System.Collections.Generic;

namespace MABCricketZone.BusinessLayer
{
    // ABSTRACTION: Abstract Person base class
    public abstract class Person
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Constructor (base)
        protected Person() { }

        protected Person(int id, string username, string password, string fullName, string email, string phone)
        {
            UserID = id;
            Username = username;
            PasswordHash = password;
            FullName = fullName;
            Email = email;
            Phone = phone;
        }

        // Abstract method - POLYMORPHISM
        public abstract string GetRole();
        public abstract string GetDashboardTitle();

        // Virtual method - can be overridden
        public virtual string GetWelcomeMessage()
        {
            return $"Welcome, {FullName}!";
        }

        public override string ToString()
        {
            return $"{GetRole()}: {FullName} (@{Username})";
        }
    }

    // ============================================
    // INHERITANCE: User inherits from Person
    // ============================================
    public class User : Person
    {
        public string Address { get; set; }
        public string City { get; set; }
        public bool IsAdmin { get; set; }

        // Default constructor
        public User() : base() { }

        // Parameterized constructor
        public User(int id, string username, string password, string fullName,
                    string email, string phone, string address, string city, bool isAdmin)
            : base(id, username, password, fullName, email, phone)
        {
            Address = address;
            City = city;
            IsAdmin = isAdmin;
        }

        // POLYMORPHISM: override abstract methods
        public override string GetRole() => IsAdmin ? "Admin" : "Customer";

        public override string GetDashboardTitle() =>
            IsAdmin ? "Admin Dashboard" : "Customer Dashboard";

        public override string GetWelcomeMessage() =>
            IsAdmin
                ? $"Welcome back, Admin {FullName}! Manage your cricket empire."
                : $"Welcome, {FullName}! Ready to gear up?";
    }

    // ============================================
    // INHERITANCE: Admin extends User (extra features)
    // ============================================
    public class Admin : User
    {
        public string AdminCode { get; set; }

        // Constructor calling base
        public Admin(int id, string username, string password, string fullName, string email)
            : base(id, username, password, fullName, email, "", "", "", true)
        {
            AdminCode = "MAB-ADMIN";
        }

        public override string GetRole() => "Super Admin";

        public override string GetWelcomeMessage() =>
            $"ADMIN ACCESS GRANTED — Welcome, {FullName}!";
    }

    // ============================================
    // ABSTRACTION: Abstract StoreItem base
    // ============================================
    public abstract class StoreItem
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }
        public bool IsAvailable { get; set; }

        protected StoreItem() { }

        protected StoreItem(int id, string name, string brand, decimal price, int qty, string img, bool avail)
        {
            ProductID = id;
            Name = name;
            Brand = brand;
            Price = price;
            Quantity = qty;
            ImagePath = img;
            IsAvailable = avail;
        }

        // Abstract - POLYMORPHISM
        public abstract string GetItemType();
        public abstract string GetDisplayInfo();

        public string GetFormattedPrice() => $"Rs. {Price:N0}";

        public bool InStock() => Quantity > 0 && IsAvailable;
    }

    // ============================================
    // INHERITANCE: Product extends StoreItem
    // ============================================
    public class Product : StoreItem
    {
        public string Category { get; set; }
        public string Description { get; set; }

        public Product() : base() { }

        public Product(int id, string name, string brand, string category, string description,
                       decimal price, int quantity, string imagePath, bool isAvailable)
            : base(id, name, brand, price, quantity, imagePath, isAvailable)
        {
            Category = category;
            Description = description;
        }

        // POLYMORPHISM
        public override string GetItemType() => Category;

        public override string GetDisplayInfo() =>
            $"{Brand} {Name} | {Category} | Rs. {Price:N0} | Qty: {Quantity}";

        public override string ToString() => $"{Brand} {Name} - Rs. {Price:N0}";
    }

    // ============================================
    // CartItem class
    // ============================================
    public class CartItem
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }

        // Constructor
        public CartItem() { }

        public CartItem(int cartId, int userId, int productId, string name, string brand,
                        decimal price, int qty, string img)
        {
            CartID = cartId;
            UserID = userId;
            ProductID = productId;
            ProductName = name;
            Brand = brand;
            UnitPrice = price;
            Quantity = qty;
            ImagePath = img;
        }

        public decimal GetSubtotal() => UnitPrice * Quantity;

        public override string ToString() =>
            $"{Brand} {ProductName} x{Quantity} = Rs. {GetSubtotal():N0}";
    }

    // ============================================
    // Order class
    // ============================================
    public class Order
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryAddress { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<CartItem> Items { get; set; }

        // Constructor
        public Order()
        {
            Items = new List<CartItem>();
            OrderDate = DateTime.Now;
            Status = "Confirmed";
        }

        public Order(int orderId, int userId, string username, decimal total, string payment,
                     string name, string address, string phone, string city, string status, DateTime date)
        {
            OrderID = orderId;
            UserID = userId;
            Username = username;
            TotalAmount = total;
            PaymentMethod = payment;
            CustomerName = name;
            DeliveryAddress = address;
            Phone = phone;
            City = city;
            Status = status;
            OrderDate = date;
            Items = new List<CartItem>();
        }

        public override string ToString() =>
            $"Order #{OrderID} | Rs. {TotalAmount:N0} | {PaymentMethod} | {Status}";
    }
}
