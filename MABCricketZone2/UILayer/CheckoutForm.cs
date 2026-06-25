// ============================================
// UILayer/CheckoutForm.cs
// ============================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MABCricketZone.BusinessLayer;

namespace MABCricketZone.UILayer
{
    public class CheckoutForm : Form
    {
        private User _user;
        private List<CartItem> _cartItems;
        private decimal _total;
        private OrderService _orderSvc;
        private CartService _cartSvc;

        private TextBox _txtName, _txtPhone, _txtCity, _txtAddress;
        private ComboBox _cmbPayment;
        private Label _lblMsg;
        private int _step = 1; // 1=details, 2=confirm

        public CheckoutForm(User user, List<CartItem> cartItems, decimal total,
                            OrderService orderSvc, CartService cartSvc)
        {
            _user = user;
            _cartItems = cartItems;
            _total = total;
            _orderSvc = orderSvc;
            _cartSvc = cartSvc;
            InitForm();
            BuildUI();
        }

        private void InitForm()
        {
            Text = "Checkout — MAB Cricket Zone";
            Size = new Size(760, 680);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            BackColor = ThemeManager.BgColor;
        }

        private void BuildUI()
        {
            // Header
            var hdr = new Panel { Width = 760, Height = 56, Location = new Point(0, 0), BackColor = ThemeManager.SurfaceColor };
            var hdrLbl = new Label { Text = "🛒  Checkout", Font = ThemeManager.FontHeading, ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(20, 14) };
            hdr.Controls.Add(hdrLbl);
            this.Controls.Add(hdr);

            // ── LEFT: ORDER SUMMARY ──
            var summaryPanel = new Panel { Width = 310, Height = 580, Location = new Point(20, 66), BackColor = ThemeManager.CardColor };
            RoundPanel(summaryPanel, 12);

            var slbl = new Label { Text = "Order Summary", Font = ThemeManager.FontSubHeading, ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(16, 14) };
            summaryPanel.Controls.Add(slbl);

            int sy = 50;
            foreach (var item in _cartItems)
            {
                var il = new Label
                {
                    Text = $"{item.Brand} {item.ProductName}",
                    Width = 180, Height = 20, Location = new Point(16, sy),
                    Font = ThemeManager.FontSmall, ForeColor = ThemeManager.TextColor,
                    BackColor = Color.Transparent,
                };
                var iq = new Label
                {
                    Text = $"x{item.Quantity}",
                    Width = 30, Height = 20, Location = new Point(200, sy),
                    Font = ThemeManager.FontSmall, ForeColor = ThemeManager.SubTextColor,
                    BackColor = Color.Transparent,
                };
                var ip = new Label
                {
                    Text = $"Rs.{item.GetSubtotal():N0}",
                    Width = 70, Height = 20, Location = new Point(234, sy),
                    Font = ThemeManager.FontSmall, ForeColor = ThemeManager.GoldPrimary,
                    BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight,
                };
                summaryPanel.Controls.Add(il);
                summaryPanel.Controls.Add(iq);
                summaryPanel.Controls.Add(ip);
                sy += 28;
            }

            var sep2 = new Panel { Width = 278, Height = 1, Location = new Point(16, sy + 4), BackColor = ThemeManager.DarkBorder };
            summaryPanel.Controls.Add(sep2); sy += 14;

            var totalLbl = new Label { Text = "Total:", Width = 100, Height = 28, Location = new Point(16, sy + 8), Font = ThemeManager.FontBold, ForeColor = ThemeManager.TextColor, BackColor = Color.Transparent };
            var totalVal = new Label { Text = $"Rs. {_total:N0}", Width = 160, Height = 28, Location = new Point(120, sy + 8), Font = ThemeManager.FontPrice, ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight };
            summaryPanel.Controls.Add(totalLbl);
            summaryPanel.Controls.Add(totalVal);

            this.Controls.Add(summaryPanel);

            // ── RIGHT: CUSTOMER DETAILS FORM ──
            var formPanel = new Panel { Width = 390, Height = 580, Location = new Point(345, 66), BackColor = ThemeManager.CardColor };
            RoundPanel(formPanel, 12);

            var flbl = new Label { Text = "Delivery Details", Font = ThemeManager.FontSubHeading, ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(16, 14) };
            formPanel.Controls.Add(flbl);

            int fy = 52; int fw = 358;
            TextBox MakeField(string ph, int yy) => new TextBox { Width = fw, Height = 36, Location = new Point(16, yy), BackColor = ThemeManager.SurfaceColor, ForeColor = ThemeManager.TextColor, BorderStyle = BorderStyle.FixedSingle, Font = ThemeManager.FontBody, PlaceholderText = ph };
            Label MakeLbl(string t, int yy) => new Label { Text = t, AutoSize = true, Location = new Point(16, yy), Font = ThemeManager.FontSmall, ForeColor = ThemeManager.SubTextColor, BackColor = Color.Transparent };

            formPanel.Controls.Add(MakeLbl("Full Name", fy));
            _txtName = MakeField("e.g. Muhammad Ahmad Bajwa", fy + 20);
            _txtName.Text = _user.FullName;
            formPanel.Controls.Add(_txtName); fy += 70;

            formPanel.Controls.Add(MakeLbl("Phone Number", fy));
            _txtPhone = MakeField("e.g. 03001234567", fy + 20);
            _txtPhone.Text = _user.Phone;
            formPanel.Controls.Add(_txtPhone); fy += 70;

            formPanel.Controls.Add(MakeLbl("City", fy));
            _txtCity = MakeField("e.g. Lahore", fy + 20);
            _txtCity.Text = _user.City;
            formPanel.Controls.Add(_txtCity); fy += 70;

            formPanel.Controls.Add(MakeLbl("Delivery Address", fy));
            _txtAddress = new TextBox { Multiline = true, Width = fw, Height = 56, Location = new Point(16, fy + 20), BackColor = ThemeManager.SurfaceColor, ForeColor = ThemeManager.TextColor, BorderStyle = BorderStyle.FixedSingle, Font = ThemeManager.FontBody, PlaceholderText = "House #, Street, Area, City..." };
            _txtAddress.Text = _user.Address;
            formPanel.Controls.Add(_txtAddress); fy += 86;

            formPanel.Controls.Add(MakeLbl("Payment Method", fy));
            _cmbPayment = new ComboBox { Width = fw, Height = 36, Location = new Point(16, fy + 20), BackColor = ThemeManager.SurfaceColor, ForeColor = ThemeManager.TextColor, FlatStyle = FlatStyle.Flat, Font = ThemeManager.FontBody, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbPayment.Items.AddRange(new[] { "Cash on Delivery (COD)", "JazzCash", "EasyPaisa", "Bank Transfer", "Credit/Debit Card" });
            _cmbPayment.SelectedIndex = 0;
            formPanel.Controls.Add(_cmbPayment); fy += 70;

            _lblMsg = new Label { Width = fw, Height = 22, Location = new Point(16, fy), BackColor = Color.Transparent, Font = ThemeManager.FontSmall };
            formPanel.Controls.Add(_lblMsg); fy += 26;

            var btnPlace = ThemeManager.MakeGoldButton("✔  PLACE ORDER", fw, 46);
            btnPlace.Location = new Point(16, fy);
            btnPlace.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            btnPlace.Click += BtnPlaceOrder_Click;
            formPanel.Controls.Add(btnPlace); fy += 54;

            var btnCancel = ThemeManager.MakeOutlineButton("Cancel", fw, 34);
            btnCancel.Location = new Point(16, fy);
            btnCancel.Click += (s, e) => this.Close();
            formPanel.Controls.Add(btnCancel);

            this.Controls.Add(formPanel);
        }

        private void BtnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text) ||
                string.IsNullOrWhiteSpace(_txtPhone.Text) ||
                string.IsNullOrWhiteSpace(_txtCity.Text) ||
                string.IsNullOrWhiteSpace(_txtAddress.Text))
            { _lblMsg.ForeColor = ThemeManager.RedAccent; _lblMsg.Text = "Please fill all delivery details."; return; }

            var order = _orderSvc.PlaceOrder(
                _user.UserID,
                _txtName.Text.Trim(),
                _txtAddress.Text.Trim(),
                _txtPhone.Text.Trim(),
                _txtCity.Text.Trim(),
                _cmbPayment.SelectedItem?.ToString() ?? "COD"
            );

            if (order == null)
            {
                // Show the actual DB error so it's easy to diagnose (e.g. missing table)
                string errDetail = string.IsNullOrWhiteSpace(_orderSvc.LastError)
                    ? "Unknown error."
                    : _orderSvc.LastError;
                _lblMsg.ForeColor = ThemeManager.RedAccent;
                _lblMsg.Text = "Order failed: " + errDetail;
                return;
            }

            // PlaceOrder already generates the receipt internally; just read it back.
            string receiptFile = System.IO.Path.Combine(
                _orderSvc.GetReceiptsFolder(),
                $"Receipt_Order{order.OrderID}_");
            var matches = System.IO.Directory.GetFiles(
                _orderSvc.GetReceiptsFolder(), $"Receipt_Order{order.OrderID}_*.txt");
            string receiptText = matches.Length > 0
                ? System.IO.File.ReadAllText(matches[0])
                : order.ToString();

            this.Hide();
            new ReceiptViewer(receiptText, isNewOrder: true).ShowDialog();
            this.Close();
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

    // ============================================
    // UILayer/ReceiptViewer.cs
    // ============================================
    public class ReceiptViewer : Form
    {
        private string _receiptText;
        private bool _isNewOrder;

        public ReceiptViewer(string receiptText, bool isNewOrder = false)
        {
            _receiptText = receiptText;
            _isNewOrder = isNewOrder;
            InitForm();
            BuildUI();
        }

        private void InitForm()
        {
            Text = "Order Receipt — MAB Cricket Zone";
            Size = new Size(680, 620);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            BackColor = ThemeManager.BgColor;
        }

        private void BuildUI()
        {
            if (_isNewOrder)
            {
                var successBanner = new Panel { Width = 680, Height = 54, Location = new Point(0, 0), BackColor = ThemeManager.GreenAccent };
                var successLbl = new Label { Text = "✔  Order Placed Successfully! Thank you for shopping with us.", Font = ThemeManager.FontBold, ForeColor = Color.White, BackColor = Color.Transparent, AutoSize = true, Location = new Point(20, 15) };
                successBanner.Controls.Add(successLbl);
                this.Controls.Add(successBanner);
            }

            var hdr = new Label { Text = "📄  Your Receipt", Font = ThemeManager.FontSubHeading, ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(20, _isNewOrder ? 64 : 14) };
            this.Controls.Add(hdr);

            int top = _isNewOrder ? 96 : 46;
            var txt = new TextBox
            {
                Width = 636, Height = 460,
                Location = new Point(20, top),
                Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
                BackColor = ThemeManager.CardColor, ForeColor = ThemeManager.TextColor,
                BorderStyle = BorderStyle.FixedSingle,
                Font = ThemeManager.FontMono,
                Text = _receiptText,
            };
            this.Controls.Add(txt);

            var btnPrint = ThemeManager.MakeGoldButton("🖨  Print / Save", 180, 38);
            btnPrint.Location = new Point(20, top + 468);
            btnPrint.Click += (s, e) =>
            {
                var dlg = new SaveFileDialog { Filter = "Text File|*.txt", FileName = "Receipt.txt" };
                if (dlg.ShowDialog() == DialogResult.OK)
                    System.IO.File.WriteAllText(dlg.FileName, _receiptText);
            };

            var btnClose = ThemeManager.MakeOutlineButton("Close", 110, 38);
            btnClose.Location = new Point(210, top + 468);
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(btnPrint);
            this.Controls.Add(btnClose);
        }
    }
}
