// ============================================
// UILayer/AdminDashboard.cs
// ============================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MABCricketZone.BusinessLayer;

namespace MABCricketZone.UILayer
{
    public class AdminDashboard : Form
    {
        private User _admin;
        private ProductService _productSvc = new ProductService();
        private UserService _userSvc = new UserService();
        private OrderService _orderSvc = new OrderService();

        private Panel _sidebar, _contentArea, _topBar;
        private Label _lblPageTitle;
        private Panel _activeSideBtn;

        public AdminDashboard(User admin)
        {
            _admin = admin;
            InitForm();
            BuildSidebar();
            BuildTopBar();
            BuildContentArea();
            LoadDashboardHome();
        }

        private void InitForm()
        {
            Text = "MAB Cricket Zone — Admin Dashboard";
            Size = new Size(1280, 780);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = ThemeManager.BgColor;
        }

        private void BuildTopBar()
        {
            _topBar = new Panel
            {
                Width = 1060, Height = 58,
                Location = new Point(220, 0),
                BackColor = ThemeManager.SurfaceColor,
            };

            var lbl = new Label
            {
                Text = $"👋  Welcome back, {_admin.FullName}",
                Font = new Font("Segoe UI", 11f),
                ForeColor = ThemeManager.SubTextColor,
                BackColor = Color.Transparent,
                AutoSize = true, Location = new Point(20, 17),
            };

            _lblPageTitle = new Label
            {
                Text = "Dashboard",
                Font = ThemeManager.FontHeading,
                ForeColor = ThemeManager.GoldPrimary,
                BackColor = Color.Transparent,
                AutoSize = true, Location = new Point(750, 14),
            };

            var btnTheme = new Button
            {
                Text = "🌙 / ☀", Width = 80, Height = 32,
                Location = new Point(960, 12),
                FlatStyle = FlatStyle.Flat,
                BackColor = ThemeManager.CardColor,
                ForeColor = ThemeManager.GoldPrimary,
                Font = ThemeManager.FontSmall,
                Cursor = Cursors.Hand,
            };
            btnTheme.FlatAppearance.BorderSize = 0;
            btnTheme.Click += (s, e) =>
            {
                ThemeManager.Toggle();
                this.BackColor = ThemeManager.BgColor;
                ThemeManager.Apply(this);
            };

            _topBar.Controls.Add(lbl);
            _topBar.Controls.Add(_lblPageTitle);
            _topBar.Controls.Add(btnTheme);
            this.Controls.Add(_topBar);
        }

        private void BuildSidebar()
        {
            _sidebar = new Panel
            {
                Width = 220, Height = this.Height,
                Location = new Point(0, 0),
                BackColor = ThemeManager.DarkSurface,
            };

            // Logo
            var logo = new Label
            {
                Text = "🏏 MAB", Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = ThemeManager.GoldPrimary,
                BackColor = Color.Transparent,
                AutoSize = true, Location = new Point(20, 16),
            };
            var logoSub = new Label
            {
                Text = "Cricket Zone", Font = new Font("Segoe UI", 9f),
                ForeColor = ThemeManager.DarkSubText,
                BackColor = Color.Transparent,
                AutoSize = true, Location = new Point(20, 44),
            };
            var sep = new Panel { Width = 180, Height = 1, Location = new Point(20, 68), BackColor = ThemeManager.DarkBorder };

            _sidebar.Controls.Add(logo);
            _sidebar.Controls.Add(logoSub);
            _sidebar.Controls.Add(sep);

            // Nav items
            string[] icons = { "📊", "📦", "➕", "🛒", "👥", "🚪" };
            string[] labels = { "Dashboard", "Inventory", "Add Product", "Orders", "Users", "Logout" };
            Action[] actions = {
                () => { LoadDashboardHome(); _lblPageTitle.Text = "Dashboard"; },
                () => { LoadInventory(); _lblPageTitle.Text = "Inventory"; },
                () => { LoadAddProduct(); _lblPageTitle.Text = "Add Product"; },
                () => { LoadOrders(); _lblPageTitle.Text = "Orders"; },
                () => { LoadUsers(); _lblPageTitle.Text = "Users"; },
                () => { this.Close(); }
            };

            int y = 80;
            for (int i = 0; i < labels.Length; i++)
            {
                int idx = i;
                var btn = new Panel
                {
                    Width = 200, Height = 46,
                    Location = new Point(10, y),
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand,
                    Tag = idx,
                };

                var ic = new Label
                {
                    Text = icons[i], Width = 30, Height = 46,
                    Location = new Point(14, 0),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent,
                    Font = new Font("Segoe UI", 14f),
                };
                var lb = new Label
                {
                    Text = labels[i], Width = 140, Height = 46,
                    Location = new Point(46, 0),
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = ThemeManager.DarkSubText,
                    BackColor = Color.Transparent,
                    Font = ThemeManager.FontBody,
                };

                void SetActive(Panel b, Label l)
                {
                    if (_activeSideBtn != null)
                    {
                        _activeSideBtn.BackColor = Color.Transparent;
                        foreach (Control c in _activeSideBtn.Controls)
                            if (c is Label ll) ll.ForeColor = ThemeManager.DarkSubText;
                    }
                    b.BackColor = Color.FromArgb(30, 212, 175, 55);
                    l.ForeColor = ThemeManager.GoldPrimary;
                    ic.ForeColor = ThemeManager.GoldPrimary;
                    _activeSideBtn = b;
                }

                int capturedIdx = idx;
                Action capturedAction = actions[capturedIdx];
                Label capturedLabel = lb;
                Panel capturedBtn = btn;

                btn.Click += (s, e) => { SetActive(capturedBtn, capturedLabel); capturedAction(); };
                // Forward clicks from child labels to the parent panel so the
                // navigation fires regardless of where inside the row the user clicks.
                ic.Click += (s, e) => { SetActive(capturedBtn, capturedLabel); capturedAction(); };
                lb.Click += (s, e) => { SetActive(capturedBtn, capturedLabel); capturedAction(); };
                btn.Controls.Add(ic);
                btn.Controls.Add(lb);
                _sidebar.Controls.Add(btn);
                y += 50;

                btn.MouseEnter += (s, e) =>
                {
                    if (_activeSideBtn != btn) btn.BackColor = Color.FromArgb(15, 212, 175, 55);
                };
                btn.MouseLeave += (s, e) =>
                {
                    if (_activeSideBtn != btn) btn.BackColor = Color.Transparent;
                };
                ic.MouseEnter += (s, e) => { if (_activeSideBtn != btn) btn.BackColor = Color.FromArgb(15, 212, 175, 55); };
                ic.MouseLeave += (s, e) => { if (_activeSideBtn != btn) btn.BackColor = Color.Transparent; };
                lb.MouseEnter += (s, e) => { if (_activeSideBtn != btn) btn.BackColor = Color.FromArgb(15, 212, 175, 55); };
                lb.MouseLeave += (s, e) => { if (_activeSideBtn != btn) btn.BackColor = Color.Transparent; };
            }

            this.Controls.Add(_sidebar);
        }

        private void BuildContentArea()
        {
            _contentArea = new Panel
            {
                Width = 1060, Height = 720,
                Location = new Point(220, 58),
                BackColor = ThemeManager.BgColor,
                AutoScroll = true,
            };
            this.Controls.Add(_contentArea);
        }

        private void ClearContent() => _contentArea.Controls.Clear();

        // ── DASHBOARD HOME ──
        private void LoadDashboardHome()
        {
            ClearContent();

            var products = _productSvc.GetAll();
            var users = _userSvc.GetAllUsers();
            var orders = _orderSvc.GetAllOrders();
            decimal totalRevenue = 0;
            foreach (var o in orders) totalRevenue += o.TotalAmount;

            // Stat cards
            var stats = new[]
            {
                ("📦 Products",   products.Count.ToString(), ThemeManager.GoldPrimary),
                ("👥 Customers",  users.Count.ToString(),    ThemeManager.GreenAccent),
                ("🛒 Orders",     orders.Count.ToString(),   Color.FromArgb(99, 179, 237)),
                ("💰 Revenue",    $"Rs. {totalRevenue:N0}",  Color.FromArgb(246, 173, 85)),
            };

            int sx = 20, sy = 20;
            foreach (var (title, val, color) in stats)
            {
                var card = new Panel
                {
                    Width = 230, Height = 110,
                    Location = new Point(sx, sy),
                    BackColor = ThemeManager.CardColor,
                };
                RoundPanel(card, 12);

                var stripe = new Panel
                {
                    Width = 6, Height = 110,
                    Location = new Point(0, 0),
                    BackColor = color,
                };
                var t = new Label
                {
                    Text = title, AutoSize = true,
                    Location = new Point(20, 20),
                    Font = new Font("Segoe UI", 9f),
                    ForeColor = ThemeManager.SubTextColor,
                    BackColor = Color.Transparent,
                };
                var v = new Label
                {
                    Text = val, AutoSize = true,
                    Location = new Point(20, 52),
                    Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                    ForeColor = color,
                    BackColor = Color.Transparent,
                };
                card.Controls.Add(stripe);
                card.Controls.Add(t);
                card.Controls.Add(v);
                _contentArea.Controls.Add(card);
                sx += 248;
            }

            // Recent orders table
            var tblLabel = new Label
            {
                Text = "Recent Orders",
                Font = ThemeManager.FontSubHeading,
                ForeColor = ThemeManager.GoldPrimary,
                BackColor = Color.Transparent,
                AutoSize = true, Location = new Point(20, 160),
            };
            _contentArea.Controls.Add(tblLabel);

            var dgv = BuildDataGridView(new Point(20, 192), 1010, 460);
            dgv.Columns.Add("OrderID", "Order #");
            dgv.Columns.Add("Customer", "Customer");
            dgv.Columns.Add("Amount", "Amount");
            dgv.Columns.Add("Payment", "Payment");
            dgv.Columns.Add("Date", "Date");
            dgv.Columns.Add("Status", "Status");

            foreach (var o in orders)
            {
                dgv.Rows.Add($"#{o.OrderID}", o.CustomerName,
                    $"Rs. {o.TotalAmount:N0}", o.PaymentMethod,
                    o.OrderDate.ToString("dd MMM yyyy"), o.Status);
            }
            _contentArea.Controls.Add(dgv);
        }

        // ── INVENTORY ──
        private void LoadInventory()
        {
            ClearContent();

            var searchBox = new TextBox
            {
                Width = 320, Height = 36,
                Location = new Point(20, 20),
                BackColor = ThemeManager.CardColor,
                ForeColor = ThemeManager.TextColor,
                BorderStyle = BorderStyle.FixedSingle,
                Font = ThemeManager.FontBody,
                PlaceholderText = "🔍  Search products...",
            };

            var dgv = BuildDataGridView(new Point(20, 72), 1010, 620);
            dgv.Columns.Add("ID", "ID");
            dgv.Columns.Add("Name", "Product Name");
            dgv.Columns.Add("Brand", "Brand");
            dgv.Columns.Add("Category", "Category");
            dgv.Columns.Add("Price", "Price");
            dgv.Columns.Add("Qty", "Stock");
            dgv.Columns.Add("Status", "Status");

            var btnEdit = ThemeManager.MakeOutlineButton("✏  Edit", 110, 36);
            btnEdit.Location = new Point(360, 20);
            var btnDelete = ThemeManager.MakeDangerButton("🗑  Delete", 110, 36);
            btnDelete.Location = new Point(480, 20);

            void RefreshGrid(string keyword = "")
            {
                dgv.Rows.Clear();
                var list = string.IsNullOrWhiteSpace(keyword)
                    ? _productSvc.GetAll()
                    : _productSvc.Search(keyword);
                foreach (var p in list)
                    dgv.Rows.Add(p.ProductID, p.Name, p.Brand, p.Category,
                        p.GetFormattedPrice(), p.Quantity,
                        p.InStock() ? "✔ In Stock" : "✘ Out of Stock");
            }

            searchBox.TextChanged += (s, e) => RefreshGrid(searchBox.Text);
            RefreshGrid();

            btnDelete.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0) { ShowMsg("Select a product first.", false); return; }
                int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["ID"].Value);
                if (MessageBox.Show("Delete this product?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (_productSvc.DeleteProduct(id)) { RefreshGrid(); ShowMsg("Product deleted.", true); }
                }
            };

            btnEdit.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0) { ShowMsg("Select a product first.", false); return; }
                int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["ID"].Value);
                var p = _productSvc.GetById(id);
                if (p != null) { new EditProductForm(p, _productSvc).ShowDialog(); RefreshGrid(); }
            };

            _contentArea.Controls.Add(searchBox);
            _contentArea.Controls.Add(btnEdit);
            _contentArea.Controls.Add(btnDelete);
            _contentArea.Controls.Add(dgv);
        }

        // ── ADD PRODUCT ──
        private void LoadAddProduct()
        {
            ClearContent();

            var card = new Panel
            {
                Width = 700, Height = 580,
                Location = new Point(160, 20),
                BackColor = ThemeManager.CardColor,
                Padding = new Padding(30),
            };
            RoundPanel(card, 14);

            int y = 20, lw = 160, fw = 460, fh = 36, lh = 26;

            Label Lbl(string t) => new Label { Text = t, AutoSize = false, Width = lw, Height = lh, BackColor = Color.Transparent, ForeColor = ThemeManager.SubTextColor, Font = ThemeManager.FontSmall, TextAlign = ContentAlignment.MiddleLeft };
            TextBox Fld(int yy) => new TextBox { Width = fw, Height = fh, Location = new Point(lw + 20, yy), BackColor = ThemeManager.SurfaceColor, ForeColor = ThemeManager.TextColor, BorderStyle = BorderStyle.FixedSingle, Font = ThemeManager.FontBody };

            var fields = new[] { "Product Name", "Brand", "Category", "Price (Rs.)", "Quantity" };
            TextBox[] txts = new TextBox[5];
            for (int i = 0; i < fields.Length; i++)
            {
                var l = Lbl(fields[i]); l.Location = new Point(20, y + 5); card.Controls.Add(l);
                txts[i] = Fld(y); card.Controls.Add(txts[i]);
                y += 50;
            }

            var lDesc = Lbl("Description"); lDesc.Location = new Point(20, y + 5); card.Controls.Add(lDesc);
            var txtDesc = new TextBox
            {
                Multiline = true, Width = fw, Height = 80,
                Location = new Point(lw + 20, y),
                BackColor = ThemeManager.SurfaceColor, ForeColor = ThemeManager.TextColor,
                BorderStyle = BorderStyle.FixedSingle, Font = ThemeManager.FontBody,
            };
            card.Controls.Add(txtDesc); y += 94;

            var lImg = Lbl("Product Image"); lImg.Location = new Point(20, y + 5); card.Controls.Add(lImg);
            var txtImg = new TextBox
            {
                Width = 340, Height = fh,
                Location = new Point(lw + 20, y),
                BackColor = ThemeManager.SurfaceColor, ForeColor = ThemeManager.TextColor,
                BorderStyle = BorderStyle.FixedSingle, Font = ThemeManager.FontBody,
                PlaceholderText = "Image file path...",
            };
            var btnBrowse = ThemeManager.MakeOutlineButton("Browse", 108, fh);
            btnBrowse.Location = new Point(lw + 370, y);
            btnBrowse.Click += (s, e) =>
            {
                var dlg = new OpenFileDialog { Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp" };
                if (dlg.ShowDialog() == DialogResult.OK) txtImg.Text = dlg.FileName;
            };
            card.Controls.Add(txtImg);
            card.Controls.Add(btnBrowse);
            y += 54;

            var lblMsg = new Label { Width = fw + lw + 20, Height = 24, Location = new Point(20, y), BackColor = Color.Transparent, Font = ThemeManager.FontSmall };
            card.Controls.Add(lblMsg);
            y += 28;

            var btnSave = ThemeManager.MakeGoldButton("➕  ADD PRODUCT", 200, 44);
            btnSave.Location = new Point(20, y);
            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txts[0].Text) || string.IsNullOrWhiteSpace(txts[1].Text))
                { lblMsg.ForeColor = ThemeManager.RedAccent; lblMsg.Text = "Name and Brand are required."; return; }

                decimal price = 0; int qty = 0;
                decimal.TryParse(txts[3].Text, out price);
                int.TryParse(txts[4].Text, out qty);

                bool ok = _productSvc.AddProduct(
                    txts[0].Text.Trim(), txts[1].Text.Trim(), txts[2].Text.Trim(),
                    txtDesc.Text.Trim(), price, qty, txtImg.Text.Trim());

                if (ok) { lblMsg.ForeColor = ThemeManager.GreenAccent; lblMsg.Text = "Product added successfully!"; foreach (var t in txts) t.Clear(); txtDesc.Clear(); txtImg.Clear(); }
                else { lblMsg.ForeColor = ThemeManager.RedAccent; lblMsg.Text = "Failed to add product."; }
            };
            card.Controls.Add(btnSave);
            _contentArea.Controls.Add(card);
        }

        // ── ORDERS ──
        private void LoadOrders()
        {
            ClearContent();

            var dgv = BuildDataGridView(new Point(20, 20), 1010, 680);
            dgv.Columns.Add("OrderID", "Order #");
            dgv.Columns.Add("User", "Username");
            dgv.Columns.Add("Customer", "Customer Name");
            dgv.Columns.Add("Amount", "Total");
            dgv.Columns.Add("Payment", "Payment");
            dgv.Columns.Add("City", "City");
            dgv.Columns.Add("Date", "Order Date");
            dgv.Columns.Add("Status", "Status");

            foreach (var o in _orderSvc.GetAllOrders())
            {
                dgv.Rows.Add($"#{o.OrderID}", o.Username, o.CustomerName,
                    $"Rs. {o.TotalAmount:N0}", o.PaymentMethod, o.City,
                    o.OrderDate.ToString("dd MMM yyyy"), o.Status);
            }
            _contentArea.Controls.Add(dgv);
        }

        // ── USERS ──
        private void LoadUsers()
        {
            ClearContent();

            var dgv = BuildDataGridView(new Point(20, 20), 1010, 680);
            dgv.Columns.Add("ID", "ID");
            dgv.Columns.Add("Username", "Username");
            dgv.Columns.Add("FullName", "Full Name");
            dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("Phone", "Phone");
            dgv.Columns.Add("City", "City");
            dgv.Columns.Add("Role", "Role");
            dgv.Columns.Add("Joined", "Joined");

            foreach (var u in _userSvc.GetAllUsers())
            {
                dgv.Rows.Add(u.UserID, u.Username, u.FullName, u.Email,
                    u.Phone, u.City, u.GetRole(),
                    u.UserID.ToString());
            }
            _contentArea.Controls.Add(dgv);
        }

        // ── HELPERS ──
        private DataGridView BuildDataGridView(Point loc, int w, int h)
        {
            var dgv = new DataGridView
            {
                Location = loc, Width = w, Height = h,
                BackgroundColor = ThemeManager.SurfaceColor,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = ThemeManager.FontBody,
                GridColor = ThemeManager.BorderColor,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            };
            dgv.DefaultCellStyle.BackColor = ThemeManager.CardColor;
            dgv.DefaultCellStyle.ForeColor = ThemeManager.TextColor;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(50, 212, 175, 55);
            dgv.DefaultCellStyle.SelectionForeColor = ThemeManager.GoldPrimary;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = ThemeManager.SurfaceColor;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.DarkBorder;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.GoldPrimary;
            dgv.ColumnHeadersDefaultCellStyle.Font = ThemeManager.FontBold;
            dgv.ColumnHeadersHeight = 40;
            dgv.RowTemplate.Height = 38;
            return dgv;
        }

        private void ShowMsg(string msg, bool success)
        {
            MessageBox.Show(msg, success ? "Success" : "Error",
                MessageBoxButtons.OK,
                success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private void RoundPanel(Panel p, int r)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            var rect = new Rectangle(0, 0, p.Width, p.Height);
            path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
            path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
            path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
            path.CloseAllFigures();
            p.Region = new Region(path);
        }
    }
}
