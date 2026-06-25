// ============================================
// UILayer/CustomerDashboard.cs
// ============================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MABCricketZone.BusinessLayer;

namespace MABCricketZone.UILayer
{
    public class CustomerDashboard : Form
    {
        private User _user;
        private ProductService _productSvc = new ProductService();
        private CartService _cartSvc = new CartService();
        private OrderService _orderSvc = new OrderService();

        private Panel _sidebar, _contentArea, _topBar;
        private Label _lblCartCount, _lblPageTitle;
        private Panel _activeSideBtn;

        public CustomerDashboard(User user)
        {
            _user = user;
            InitForm();
            BuildTopBar();
            BuildSidebar();
            BuildContentArea();
            LoadShop();
        }

        private void InitForm()
        {
            Text = $"MAB Cricket Zone — {_user.FullName}";
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

            var greet = new Label
            {
                Text = $"👋  Welcome, {_user.FullName}",
                Font = new Font("Segoe UI", 11f),
                ForeColor = ThemeManager.SubTextColor,
                BackColor = Color.Transparent,
                AutoSize = true, Location = new Point(20, 17),
            };

            _lblPageTitle = new Label
            {
                Text = "Shop",
                Font = ThemeManager.FontHeading,
                ForeColor = ThemeManager.GoldPrimary,
                BackColor = Color.Transparent,
                AutoSize = true, Location = new Point(750, 14),
            };

            _lblCartCount = new Label
            {
                Text = "🛒 Cart (0)",
                Font = ThemeManager.FontBold,
                ForeColor = ThemeManager.GoldPrimary,
                BackColor = Color.Transparent,
                AutoSize = true, Location = new Point(860, 18),
                Cursor = Cursors.Hand,
            };
            _lblCartCount.Click += (s, e) => { LoadCart(); _lblPageTitle.Text = "My Cart"; };

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
            btnTheme.Click += (s, e) => { ThemeManager.Toggle(); this.BackColor = ThemeManager.BgColor; ThemeManager.Apply(this); };

            _topBar.Controls.Add(greet);
            _topBar.Controls.Add(_lblPageTitle);
            _topBar.Controls.Add(_lblCartCount);
            _topBar.Controls.Add(btnTheme);
            this.Controls.Add(_topBar);

            RefreshCartCount();
        }

        private void BuildSidebar()
        {
            _sidebar = new Panel
            {
                Width = 220, Height = this.Height,
                Location = new Point(0, 0),
                BackColor = ThemeManager.DarkSurface,
            };

            var logo = new Label { Text = "🏏 MAB", Font = new Font("Segoe UI", 18f, FontStyle.Bold), ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(20, 16) };
            var logoSub = new Label { Text = "Cricket Zone", Font = new Font("Segoe UI", 9f), ForeColor = ThemeManager.DarkSubText, BackColor = Color.Transparent, AutoSize = true, Location = new Point(20, 44) };
            var sep = new Panel { Width = 180, Height = 1, Location = new Point(20, 68), BackColor = ThemeManager.DarkBorder };
            _sidebar.Controls.Add(logo); _sidebar.Controls.Add(logoSub); _sidebar.Controls.Add(sep);

            string[] icons = { "🏪", "🛒", "📋", "👤", "🚪" };
            string[] labels = { "Shop", "My Cart", "My Orders", "Profile", "Logout" };
            Action[] actions = {
                () => { LoadShop(); _lblPageTitle.Text = "Shop"; },
                () => { LoadCart(); _lblPageTitle.Text = "My Cart"; },
                () => { LoadMyOrders(); _lblPageTitle.Text = "My Orders"; },
                () => { LoadProfile(); _lblPageTitle.Text = "My Profile"; },
                () => this.Close(),
            };

            int y = 80;
            for (int i = 0; i < labels.Length; i++)
            {
                int idx = i;
                var btn = new Panel { Width = 200, Height = 46, Location = new Point(10, y), BackColor = Color.Transparent, Cursor = Cursors.Hand };
                var ic = new Label { Text = icons[i], Width = 30, Height = 46, Location = new Point(14, 0), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent, Font = new Font("Segoe UI", 14f) };
                var lb = new Label { Text = labels[i], Width = 140, Height = 46, Location = new Point(46, 0), TextAlign = ContentAlignment.MiddleLeft, ForeColor = ThemeManager.DarkSubText, BackColor = Color.Transparent, Font = ThemeManager.FontBody };

                void SetActive(Panel b, Label l)
                {
                    if (_activeSideBtn != null) { _activeSideBtn.BackColor = Color.Transparent; foreach (Control c in _activeSideBtn.Controls) if (c is Label ll) ll.ForeColor = ThemeManager.DarkSubText; }
                    b.BackColor = Color.FromArgb(30, 212, 175, 55); l.ForeColor = ThemeManager.GoldPrimary; _activeSideBtn = b;
                }

                Panel cb = btn; Label cl = lb; Action ca = actions[idx];
                btn.Click += (s, e) => { SetActive(cb, cl); ca(); };
                // Forward clicks from child labels to the parent panel.
                ic.Click += (s, e) => { SetActive(cb, cl); ca(); };
                lb.Click += (s, e) => { SetActive(cb, cl); ca(); };
                btn.Controls.Add(ic); btn.Controls.Add(lb);
                _sidebar.Controls.Add(btn);
                y += 50;
                btn.MouseEnter += (s, e) => { if (_activeSideBtn != btn) btn.BackColor = Color.FromArgb(15, 212, 175, 55); };
                btn.MouseLeave += (s, e) => { if (_activeSideBtn != btn) btn.BackColor = Color.Transparent; };
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

        // ── SHOP / BROWSE PRODUCTS ──
        private void LoadShop(string searchKw = "", string cat = "All")
        {
            ClearContent();

            // Search bar
            var searchBox = new TextBox
            {
                Width = 340, Height = 36, Location = new Point(20, 20),
                BackColor = ThemeManager.CardColor, ForeColor = ThemeManager.TextColor,
                BorderStyle = BorderStyle.FixedSingle, Font = ThemeManager.FontBody,
                PlaceholderText = "🔍  Search bats, balls, pads...",
            };

            // Category filter
            var cmbCat = new ComboBox
            {
                Width = 160, Height = 36, Location = new Point(370, 20),
                BackColor = ThemeManager.CardColor, ForeColor = ThemeManager.TextColor,
                FlatStyle = FlatStyle.Flat, Font = ThemeManager.FontBody,
                DropDownStyle = ComboBoxStyle.DropDownList,
            };
            foreach (var c in _productSvc.GetCategories()) cmbCat.Items.Add(c);
            cmbCat.SelectedIndex = 0;

            // Products flow panel
            var flow = new FlowLayoutPanel
            {
                Width = 1040, AutoSize = true,
                Location = new Point(10, 72),
                BackColor = ThemeManager.BgColor,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(4),
            };

            void RenderProducts()
            {
                flow.Controls.Clear();
                var kw = searchBox.Text.Trim();
                var category = cmbCat.SelectedItem?.ToString() ?? "All";
                List<Product> list;
                if (!string.IsNullOrWhiteSpace(kw)) list = _productSvc.Search(kw);
                else if (category != "All") list = _productSvc.GetByCategory(category);
                else list = _productSvc.GetAll();

                if (list.Count == 0)
                {
                    var nf = new Label { Text = "No products found.", Font = ThemeManager.FontHeading, ForeColor = ThemeManager.SubTextColor, BackColor = Color.Transparent, AutoSize = true, Margin = new Padding(20) };
                    flow.Controls.Add(nf);
                    return;
                }

                foreach (var p in list)
                {
                    var card = MakeProductCard(p);
                    flow.Controls.Add(card);
                }
            }

            searchBox.TextChanged += (s, e) => RenderProducts();
            cmbCat.SelectedIndexChanged += (s, e) => RenderProducts();
            RenderProducts();

            _contentArea.Controls.Add(searchBox);
            _contentArea.Controls.Add(cmbCat);
            _contentArea.Controls.Add(flow);
        }

        private Panel MakeProductCard(Product p)
        {
            var card = new Panel
            {
                Width = 230, Height = 300,
                BackColor = ThemeManager.CardColor,
                Margin = new Padding(8),
                Cursor = Cursors.Hand,
            };
            RoundPanel(card, 12);

            // Image area
            var imgPanel = new Panel
            {
                Width = 230, Height = 150,
                Location = new Point(0, 0),
                BackColor = ThemeManager.SurfaceColor,
            };

            var imgLbl = new Label
            {
                Text = GetProductEmoji(p.Category),
                Font = new Font("Segoe UI", 44f),
                AutoSize = false, Width = 230, Height = 150,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
            };

            // Try to load real image
            if (!string.IsNullOrWhiteSpace(p.ImagePath) && File.Exists(p.ImagePath))
            {
                try
                {
                    var pb = new PictureBox
                    {
                        Width = 230, Height = 150,
                        Location = new Point(0, 0),
                        Image = Image.FromFile(p.ImagePath),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BackColor = ThemeManager.SurfaceColor,
                    };
                    imgPanel.Controls.Add(pb);
                }
                catch { imgPanel.Controls.Add(imgLbl); }
            }
            else { imgPanel.Controls.Add(imgLbl); }

            // Stock badge
            if (!p.InStock())
            {
                var badge = new Label { Text = "Out of Stock", Width = 100, Height = 22, Location = new Point(126, 4), BackColor = ThemeManager.RedAccent, ForeColor = Color.White, Font = ThemeManager.FontSmall, TextAlign = ContentAlignment.MiddleCenter };
                imgPanel.Controls.Add(badge);
            }

            card.Controls.Add(imgPanel);

            var lblBrand = new Label { Text = p.Brand, AutoSize = true, Location = new Point(10, 156), Font = ThemeManager.FontSmall, ForeColor = ThemeManager.SubTextColor, BackColor = Color.Transparent };
            var lblName = new Label { Text = p.Name.Length > 24 ? p.Name.Substring(0, 24) + "…" : p.Name, AutoSize = false, Width = 210, Height = 36, Location = new Point(10, 172), Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = ThemeManager.TextColor, BackColor = Color.Transparent };
            var lblPrice = new Label { Text = p.GetFormattedPrice(), AutoSize = true, Location = new Point(10, 210), Font = ThemeManager.FontPrice, ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent };

            var btnAdd = ThemeManager.MakeGoldButton(p.InStock() ? "Add to Cart" : "Unavailable", 210, 36);
            btnAdd.Location = new Point(10, 252);
            if (!p.InStock()) { btnAdd.Enabled = false; btnAdd.BackColor = ThemeManager.DarkBorder; }
            btnAdd.Click += (s, e) =>
            {
                if (_cartSvc.AddToCart(_user.UserID, p.ProductID, 1))
                {
                    RefreshCartCount();
                    MessageBox.Show($"{p.Brand} {p.Name} added to cart!", "Added ✔",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            card.Controls.Add(lblBrand);
            card.Controls.Add(lblName);
            card.Controls.Add(lblPrice);
            card.Controls.Add(btnAdd);
            return card;
        }

        // ── CART ──
        private void LoadCart()
        {
            ClearContent();

            var cartItems = _cartSvc.GetCart(_user.UserID);
            decimal total = _cartSvc.GetCartTotal(cartItems);

            var lblTitle = new Label { Text = $"🛒  Your Cart  ({cartItems.Count} items)", Font = ThemeManager.FontSubHeading, ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(20, 20) };
            _contentArea.Controls.Add(lblTitle);

            if (cartItems.Count == 0)
            {
                var empty = new Label { Text = "Your cart is empty.\nBrowse the shop to add items!", Font = ThemeManager.FontHeading, ForeColor = ThemeManager.SubTextColor, BackColor = Color.Transparent, AutoSize = true, Location = new Point(300, 200) };
                var btnShop = ThemeManager.MakeGoldButton("Go to Shop", 180, 44);
                btnShop.Location = new Point(430, 320);
                btnShop.Click += (s, e) => { LoadShop(); _lblPageTitle.Text = "Shop"; };
                _contentArea.Controls.Add(empty);
                _contentArea.Controls.Add(btnShop);
                return;
            }

            // Cart items table
            var dgv = new DataGridView
            {
                Location = new Point(20, 60), Width = 750, Height = cartItems.Count * 44 + 60,
                BackgroundColor = ThemeManager.SurfaceColor, BorderStyle = BorderStyle.None,
                RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = ThemeManager.FontBody, GridColor = ThemeManager.BorderColor,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            };
            dgv.DefaultCellStyle.BackColor = ThemeManager.CardColor;
            dgv.DefaultCellStyle.ForeColor = ThemeManager.TextColor;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.DarkBorder;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.GoldPrimary;
            dgv.ColumnHeadersDefaultCellStyle.Font = ThemeManager.FontBold;
            dgv.ColumnHeadersHeight = 40; dgv.RowTemplate.Height = 44;

            dgv.Columns.Add("CartID", "Cart ID"); dgv.Columns["CartID"].Visible = false;
            dgv.Columns.Add("Product", "Product");
            dgv.Columns.Add("Brand", "Brand");
            dgv.Columns.Add("UnitPrice", "Unit Price");
            dgv.Columns.Add("Quantity", "Qty");
            dgv.Columns.Add("Subtotal", "Subtotal");

            foreach (var item in cartItems)
                dgv.Rows.Add(item.CartID, item.ProductName, item.Brand,
                    $"Rs. {item.UnitPrice:N0}", item.Quantity, $"Rs. {item.GetSubtotal():N0}");

            // Summary card
            var summaryCard = new Panel { Width = 260, Height = 260, Location = new Point(790, 60), BackColor = ThemeManager.CardColor };
            RoundPanel(summaryCard, 12);

            var lblSum = new Label { Text = "Order Summary", Font = ThemeManager.FontSubHeading, ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(16, 16) };
            var lblItems = new Label { Text = $"Items: {cartItems.Count}", Font = ThemeManager.FontBody, ForeColor = ThemeManager.SubTextColor, BackColor = Color.Transparent, AutoSize = true, Location = new Point(16, 50) };
            var sep = new Panel { Width = 228, Height = 1, Location = new Point(16, 80), BackColor = ThemeManager.DarkBorder };
            var lblTotalLbl = new Label { Text = "Total", Font = ThemeManager.FontBold, ForeColor = ThemeManager.TextColor, BackColor = Color.Transparent, AutoSize = true, Location = new Point(16, 92) };
            var lblTotalVal = new Label { Text = $"Rs. {total:N0}", Font = ThemeManager.FontPrice, ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(16, 116) };

            var btnCheckout = ThemeManager.MakeGoldButton("CHECKOUT →", 228, 46);
            btnCheckout.Location = new Point(16, 170);
            btnCheckout.Click += (s, e) =>
            {
                var co = new CheckoutForm(_user, cartItems, total, _orderSvc, _cartSvc);
                co.ShowDialog();
                LoadCart();
                RefreshCartCount();
            };

            var btnClear = ThemeManager.MakeDangerButton("Clear Cart", 228, 34);
            btnClear.Location = new Point(16, 222);
            btnClear.Click += (s, e) =>
            {
                if (MessageBox.Show("Clear entire cart?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                { _cartSvc.ClearCart(_user.UserID); LoadCart(); RefreshCartCount(); }
            };

            summaryCard.Controls.Add(lblSum); summaryCard.Controls.Add(lblItems);
            summaryCard.Controls.Add(sep); summaryCard.Controls.Add(lblTotalLbl);
            summaryCard.Controls.Add(lblTotalVal); summaryCard.Controls.Add(btnCheckout);
            summaryCard.Controls.Add(btnClear);

            // Remove selected item
            var btnRemove = ThemeManager.MakeDangerButton("Remove Item", 140, 34);
            btnRemove.Location = new Point(20, dgv.Location.Y + dgv.Height + 10);
            btnRemove.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0) return;
                int cid = Convert.ToInt32(dgv.SelectedRows[0].Cells["CartID"].Value);
                _cartSvc.RemoveItem(cid);
                LoadCart(); RefreshCartCount();
            };

            _contentArea.Controls.Add(dgv);
            _contentArea.Controls.Add(summaryCard);
            _contentArea.Controls.Add(btnRemove);
        }

        // ── MY ORDERS ──
        private void LoadMyOrders()
        {
            ClearContent();
            var orders = _orderSvc.GetUserOrders(_user.UserID);

            var lbl = new Label { Text = "📋  My Orders", Font = ThemeManager.FontSubHeading, ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(20, 20) };
            _contentArea.Controls.Add(lbl);

            if (orders.Count == 0)
            {
                var empty = new Label { Text = "No orders yet. Start shopping!", Font = ThemeManager.FontHeading, ForeColor = ThemeManager.SubTextColor, BackColor = Color.Transparent, AutoSize = true, Location = new Point(300, 200) };
                _contentArea.Controls.Add(empty); return;
            }

            var dgv = new DataGridView
            {
                Location = new Point(20, 60), Width = 1010, Height = 620,
                BackgroundColor = ThemeManager.SurfaceColor, BorderStyle = BorderStyle.None,
                RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = ThemeManager.FontBody, GridColor = ThemeManager.BorderColor,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            };
            dgv.DefaultCellStyle.BackColor = ThemeManager.CardColor;
            dgv.DefaultCellStyle.ForeColor = ThemeManager.TextColor;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.DarkBorder;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.GoldPrimary;
            dgv.ColumnHeadersDefaultCellStyle.Font = ThemeManager.FontBold;
            dgv.ColumnHeadersHeight = 40; dgv.RowTemplate.Height = 44;

            dgv.Columns.Add("OrderID", "Order #");
            dgv.Columns.Add("Amount", "Total Amount");
            dgv.Columns.Add("Payment", "Payment");
            dgv.Columns.Add("City", "City");
            dgv.Columns.Add("Status", "Status");
            dgv.Columns.Add("Date", "Order Date");

            foreach (var o in orders)
                dgv.Rows.Add($"#{o.OrderID}", $"Rs. {o.TotalAmount:N0}",
                    o.PaymentMethod, o.City, o.Status, o.OrderDate.ToString("dd MMM yyyy"));

            var btnReceipt = ThemeManager.MakeGoldButton("📄  View Receipt", 180, 36);
            btnReceipt.Location = new Point(20, 690);
            btnReceipt.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0) return;
                var folder = _orderSvc.GetReceiptsFolder();
                var orderId = dgv.SelectedRows[0].Cells["OrderID"].Value.ToString().Replace("#", "");
                var files = Directory.GetFiles(folder, $"Receipt_Order{orderId}_*.txt");
                if (files.Length > 0)
                {
                    var rf = new ReceiptViewer(File.ReadAllText(files[0]));
                    rf.ShowDialog();
                }
                else MessageBox.Show("Receipt file not found.", "Info");
            };

            _contentArea.Controls.Add(dgv);
            _contentArea.Controls.Add(btnReceipt);
        }

        // ── PROFILE ──
        private void LoadProfile()
        {
            ClearContent();

            var card = new Panel { Width = 560, Height = 420, Location = new Point(240, 30), BackColor = ThemeManager.CardColor };
            RoundPanel(card, 14);

            var avatar = new Label { Text = "👤", Width = 80, Height = 80, Location = new Point(240, 20), Font = new Font("Segoe UI", 36f), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent };
            var lblName = new Label { Text = _user.FullName, AutoSize = true, Location = new Point(30, 108), Font = ThemeManager.FontHeading, ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent };
            var lblRole = new Label { Text = _user.GetRole(), AutoSize = true, Location = new Point(30, 140), Font = ThemeManager.FontSmall, ForeColor = ThemeManager.SubTextColor, BackColor = Color.Transparent };

            int y = 170;
            var info = new[] { ("Username", _user.Username), ("Email", _user.Email), ("Phone", _user.Phone), ("City", _user.City), ("Address", _user.Address) };
            foreach (var (label, val) in info)
            {
                var l = new Label { Text = label + ":", Width = 100, Height = 26, Location = new Point(30, y), Font = ThemeManager.FontSmall, ForeColor = ThemeManager.SubTextColor, BackColor = Color.Transparent };
                var v = new Label { Text = string.IsNullOrWhiteSpace(val) ? "—" : val, Width = 390, Height = 26, Location = new Point(140, y), Font = ThemeManager.FontBody, ForeColor = ThemeManager.TextColor, BackColor = Color.Transparent };
                card.Controls.Add(l); card.Controls.Add(v);
                y += 32;
            }

            card.Controls.Add(avatar); card.Controls.Add(lblName); card.Controls.Add(lblRole);
            _contentArea.Controls.Add(card);
        }

        private void RefreshCartCount()
        {
            int count = _cartSvc.GetCartCount(_user.UserID);
            if (_lblCartCount != null) _lblCartCount.Text = $"🛒 Cart ({count})";
        }

        private static string GetProductEmoji(string category) => category switch
        {
            "Bats"        => "🏏",
            "Balls"       => "🔴",
            "Protection"  => "🛡",
            "Gloves"      => "🧤",
            "Footwear"    => "👟",
            "Accessories" => "🎒",
            "Training"    => "🎯",
            _             => "📦",
        };

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
