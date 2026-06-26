<p align="center">
  <img src="assets/banner.png" alt="MAB Cricket Zone Banner" width="100%">
</p>

<h1 align="center">🏏 MAB Cricket Zone</h1>

<p align="center">
A modern Cricket Shop Management System developed using <strong>C#</strong>, <strong>Windows Forms</strong>, and <strong>MySQL</strong>, applying Object-Oriented Programming (OOP) principles.
</p>

<p align="center">

![C#](https://img.shields.io/badge/C%23-512BD4?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-0078D4?style=for-the-badge&logo=windows&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=for-the-badge&logo=mysql&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91?style=for-the-badge&logo=visualstudio&logoColor=white)

</p>

<p align="center">

![GitHub stars](https://img.shields.io/github/stars/YOUR_USERNAME/MABCricketZone?style=social)
![GitHub forks](https://img.shields.io/github/forks/YOUR_USERNAME/MABCricketZone?style=social)

</p>

---

# 📖 About

**MAB Cricket Zone** is a desktop-based Cricket Shop Management System built as an Object-Oriented Programming semester project.

The application is an enhanced version of a console-based Cricket Shop Management System that was originally developed during the Programming Fundamentals course. The project was redesigned into a complete Windows Forms desktop application with a graphical interface, layered architecture, and MySQL database integration.

The system enables customers to browse cricket products, manage shopping carts, and place orders, while administrators can efficiently manage inventory, users, and customer orders.

---

# ✨ Features

## 👤 Customer

- Secure Login
- Browse Products
- Search Products
- Shopping Cart
- Place Orders
- View Order History
- User Profile

---

## 👨‍💼 Administrator

- Dashboard
- Add Products
- Update Products
- Delete Products
- Inventory Management
- User Management
- Order Management
- Sales Reports

---

# 🖥 Screenshots

> Create a folder named **screenshots** and place your images inside it.

| Login | Dashboard |
|-------|-----------|
| ![](screenshots/login.png) | ![](screenshots/dashboard.png) |

| Products | Cart |
|----------|------|
| ![](screenshots/products.png) | ![](screenshots/cart.png) |

| Orders |
|--------|
| ![](screenshots/orders.png) |

---

# 🏗 Architecture

```
MABCricketZone
│
├── BusinessLayer
│
├── DataLayer
│
│   ├── DatabaseConnection.cs
│   ├── ProductRepository.cs
│   ├── UserRepository.cs
│   └── CartOrderRepository.cs
│
├── UILayer
│
├── Database
│   └── cricketzonedb.sql
│
├── assets
│   └── banner.png
│
├── screenshots
│
└── README.md
```

---

# ⚙ Technologies Used

| Technology | Purpose |
|------------|---------|
| C# | Programming Language |
| Windows Forms | Desktop GUI |
| .NET Framework | Application Framework |
| MySQL | Database |
| ADO.NET | Database Connectivity |
| Visual Studio 2022 | IDE |

---

# 🧠 OOP Concepts Implemented

✔ Encapsulation

✔ Abstraction

✔ Inheritance

✔ Polymorphism

✔ Classes & Objects

✔ Constructors

✔ Repository Pattern

✔ Layered Architecture

---

# 🗄 Database

Database Name

```
cricketzonedb
```

Tables

- Users
- Products
- Cart
- Orders
- OrderItems

---

# 🚀 Getting Started

## Clone Repository

```bash
git clone https://github.com/YOUR_USERNAME/MABCricketZone.git
```

---

## Open in Visual Studio

Open

```
MABCricketZone.sln
```

---

## Import Database

Import

```
Database/cricketzonedb.sql
```

using MySQL Workbench.

---

## Update Database Connection

Open

```
DataLayer/DatabaseConnection.cs
```

Replace

```csharp
Server=localhost;
Database=cricketzonedb;
Uid=YOUR_USERNAME;
Pwd=YOUR_PASSWORD;
```

with your own MySQL credentials.

---

## Run

Press

```
F5
```

or

```
Start Debugging
```

---

# 📂 Project Structure

```
Business Layer
│
├── Business Logic
├── Validation
└── Processing

Data Layer
│
├── Database Connection
├── CRUD Operations
└── Repository Classes

UI Layer
│
├── Windows Forms
├── Navigation
└── User Interaction
```

---

# 📈 Future Improvements

- Online Payment Integration

- Invoice PDF Generation

- Email Notifications

- Barcode Scanner

- Product Reviews

- Sales Analytics Dashboard

- Dark Mode

- Cloud Database Support

- REST API Integration

---

# 🎯 Learning Outcomes

During this project I gained practical experience in:

- Object-Oriented Programming
- Windows Forms Development
- Layered Architecture
- Repository Pattern
- CRUD Operations
- MySQL Database Design
- Authentication & Authorization
- Software Engineering Best Practices

---

# 👨‍💻 Developer

**Muhammad Ahmad Bajwa**

Computer Science Student

University of Engineering and Technology (UET), Lahore

GitHub

https://github.com/muhammadahmadbajwa811-collab

LinkedIn

(Add your LinkedIn URL)

---

# 🙏 Acknowledgements

I would like to express my sincere gratitude to **Ma'am Nimra** for her continuous guidance, encouragement, and valuable feedback throughout the development of this project.

Special thanks to the **Department of Computer Science** at the **University of Engineering and Technology (UET), Lahore** for providing an excellent learning environment and encouraging practical software development.

---

# ⭐ Support

If you found this project helpful, consider giving it a ⭐ on GitHub.

---

<p align="center">

Made with ❤️ using C#, Windows Forms and MySQL.

</p>
