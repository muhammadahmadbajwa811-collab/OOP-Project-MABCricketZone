// ============================================
// UILayer/LoginForm.cs
// ============================================
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MABCricketZone.BusinessLayer;

namespace MABCricketZone.UILayer
{
    public class LoginForm : Form
    {
        private UserService _userService = new UserService();
        private bool _showingLogin = true;

        // Controls
        private Panel _leftPanel, _rightPanel, _formCard;
        private Label _lblLogo, _lblTagline, _lblTitle, _lblSub, _lblMsg;
        private TextBox _txtUsername, _txtPassword, _txtFullName, _txtEmail, _txtPhone;
        private Button _btnSubmit, _btnToggle, _btnTheme;
        private CheckBox _chkShowPass;
        private Panel _tabBar;
        private Label _tabLogin, _tabRegister;

        public LoginForm()
        {
            InitializeComponent();
            SetupLayout();
            ShowLogin();
        }

        private void InitializeComponent()
        {
            this.Text = "MAB Cricket Zone";
            this.Size = new Size(1000, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = ThemeManager.BgColor;
            this.Icon = SystemIcons.Application;
        }

        private void SetupLayout()
        {
            this.Controls.Clear();

            // ── LEFT HERO PANEL ──
            _leftPanel = new Panel
            {
                Width = 420, Height = this.Height,
                Location = new Point(0, 0),
                BackColor = ThemeManager.DarkSurface,
            };
            _leftPanel.Paint += LeftPanel_Paint;

            _lblLogo = new Label
            {
                Text = "🏏 MAB Cricket Zone",
                Font = ThemeManager.FontLogo,
                ForeColor = ThemeManager.GoldPrimary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(40, 120),
            };

            _lblTagline = new Label
            {
                Text = "Pakistan's Premier\nCricket Equipment Store",
                Font = new Font("Segoe UI", 13f, FontStyle.Regular),
                ForeColor = ThemeManager.DarkText,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(40, 175),
            };

            var lblFeatures = new Label
            {
                Text = "✔  Premium International Brands\n✔  Secure Online Checkout\n✔  Fast Delivery Across Pakistan\n✔  Expert Cricket Gear Advice",
                Font = new Font("Segoe UI", 10f),
                ForeColor = ThemeManager.DarkSubText,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(40, 280),
            };

            var lblFooter = new Label
            {
                Text = "Gear Up. Play Hard. Win Big.",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = ThemeManager.GoldDark,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(40, 530),
            };

            _leftPanel.Controls.AddRange(new Control[] { _lblLogo, _lblTagline, lblFeatures, lblFooter });
            this.Controls.Add(_leftPanel);

            // ── RIGHT PANEL ──
            _rightPanel = new Panel
            {
                Width = 580, Height = this.Height,
                Location = new Point(420, 0),
                BackColor = ThemeManager.BgColor,
            };

            // Theme toggle button (top-right)
            _btnTheme = new Button
            {
                Text = "☀", Width = 36, Height = 36,
                Location = new Point(525, 12),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = ThemeManager.GoldPrimary,
                Font = new Font("Segoe UI", 14f),
                Cursor = Cursors.Hand,
            };
            _btnTheme.FlatAppearance.BorderSize = 0;
            _btnTheme.Click += (s, e) => { ThemeManager.Toggle(); SetupLayout(); };

            // ── FORM CARD ──
            _formCard = new Panel
            {
                Width = 420, Height = 520,
                Location = new Point(80, 55),
                BackColor = ThemeManager.CardColor,
                Padding = new Padding(30),
            };
            RoundCorners(_formCard, 14);

            // Tab bar
            _tabBar = new Panel
            {
                Width = 360, Height = 44,
                Location = new Point(30, 20),
                BackColor = ThemeManager.SurfaceColor,
            };
            RoundCorners(_tabBar, 8);

            _tabLogin = new Label
            {
                Text = "Sign In", Width = 178, Height = 44,
                Location = new Point(2, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = ThemeManager.FontBold,
                ForeColor = ThemeManager.GoldPrimary,
                BackColor = ThemeManager.CardColor,
                Cursor = Cursors.Hand,
            };
            RoundCorners(_tabLogin, 7);
            _tabLogin.Click += (s, e) => ShowLogin();

            _tabRegister = new Label
            {
                Text = "Sign Up", Width = 178, Height = 44,
                Location = new Point(182, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = ThemeManager.FontBold,
                ForeColor = ThemeManager.SubTextColor,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
            };
            _tabRegister.Click += (s, e) => ShowRegister();

            _tabBar.Controls.Add(_tabLogin);
            _tabBar.Controls.Add(_tabRegister);
            _formCard.Controls.Add(_tabBar);

            // Title + sub
            _lblTitle = new Label
            {
                Font = ThemeManager.FontHeading,
                ForeColor = ThemeManager.TextColor,
                BackColor = Color.Transparent,
                AutoSize = true, Location = new Point(30, 80),
            };
            _lblSub = new Label
            {
                Font = ThemeManager.FontSmall,
                ForeColor = ThemeManager.SubTextColor,
                BackColor = Color.Transparent,
                AutoSize = true, Location = new Point(30, 106),
            };
            _formCard.Controls.Add(_lblTitle);
            _formCard.Controls.Add(_lblSub);

            // FullName (register only)
            _txtFullName = MakeField("Full Name", 30, 140);
            _formCard.Controls.Add(_txtFullName);

            // Username
            _txtUsername = MakeField("Username", 30, 140);
            _formCard.Controls.Add(_txtUsername);

            // Email (register)
            _txtEmail = MakeField("Email Address", 30, 200);
            _formCard.Controls.Add(_txtEmail);

            // Phone (register)
            _txtPhone = MakeField("Phone Number", 30, 260);
            _formCard.Controls.Add(_txtPhone);

            // Password
            _txtPassword = MakeField("Password", 30, 200, true);
            _formCard.Controls.Add(_txtPassword);

            // Show password
            _chkShowPass = new CheckBox
            {
                Text = "Show password",
                Font = ThemeManager.FontSmall,
                ForeColor = ThemeManager.SubTextColor,
                BackColor = Color.Transparent,
                Location = new Point(30, 260),
                AutoSize = true, Cursor = Cursors.Hand,
            };
            _chkShowPass.CheckedChanged += (s, e) =>
                _txtPassword.UseSystemPasswordChar = !_chkShowPass.Checked;
            _formCard.Controls.Add(_chkShowPass);

            // Message label
            _lblMsg = new Label
            {
                Text = "", Width = 360, Height = 22,
                Location = new Point(30, 292),
                Font = ThemeManager.FontSmall,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
            };
            _formCard.Controls.Add(_lblMsg);

            // Submit button
            _btnSubmit = ThemeManager.MakeGoldButton("SIGN IN", 360, 46);
            _btnSubmit.Location = new Point(30, 318);
            _btnSubmit.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            _btnSubmit.Click += BtnSubmit_Click;
            _formCard.Controls.Add(_btnSubmit);

            // Toggle link
            _btnToggle = new Button
            {
                Width = 360, Height = 28,
                Location = new Point(30, 372),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = ThemeManager.GoldPrimary,
                Font = ThemeManager.FontSmall,
                Cursor = Cursors.Hand,
            };
            _btnToggle.FlatAppearance.BorderSize = 0;
            _btnToggle.Click += (s, e) => { if (_showingLogin) ShowRegister(); else ShowLogin(); };
            _formCard.Controls.Add(_btnToggle);

            _rightPanel.Controls.Add(_formCard);
            _rightPanel.Controls.Add(_btnTheme);
            this.Controls.Add(_rightPanel);
        }

        private TextBox MakeField(string hint, int x, int y, bool isPassword = false)
        {
            var tb = new TextBox
            {
                Width = 360, Height = 36,
                Location = new Point(x, y),
                BackColor = ThemeManager.SurfaceColor,
                ForeColor = ThemeManager.TextColor,
                BorderStyle = BorderStyle.FixedSingle,
                Font = ThemeManager.FontBody,
                PlaceholderText = hint,
                UseSystemPasswordChar = isPassword,
            };
            return tb;
        }

        private void ShowLogin()
        {
            _showingLogin = true;
            _lblTitle.Text = "Welcome Back";
            _lblSub.Text = "Sign in to your account";

            _txtFullName.Visible = false;
            _txtEmail.Visible = false;
            _txtPhone.Visible = false;

            _txtUsername.Location = new Point(30, 140);
            _txtPassword.Location = new Point(30, 196);
            _chkShowPass.Location = new Point(30, 242);
            _lblMsg.Location = new Point(30, 268);
            _btnSubmit.Location = new Point(30, 294);
            _btnSubmit.Text = "SIGN IN";
            _btnToggle.Location = new Point(30, 348);
            _btnToggle.Text = "Don't have an account? Sign Up";

            _tabLogin.BackColor = ThemeManager.CardColor;
            _tabLogin.ForeColor = ThemeManager.GoldPrimary;
            _tabRegister.BackColor = Color.Transparent;
            _tabRegister.ForeColor = ThemeManager.SubTextColor;

            _lblMsg.Text = "";
        }

        private void ShowRegister()
        {
            _showingLogin = false;
            _lblTitle.Text = "Create Account";
            _lblSub.Text = "Join MAB Cricket Zone today";

            _txtFullName.Visible = true;
            _txtEmail.Visible = true;
            _txtPhone.Visible = true;

            _txtFullName.Location = new Point(30, 130);
            _txtUsername.Location = new Point(30, 178);
            _txtEmail.Location = new Point(30, 226);
            _txtPhone.Location = new Point(30, 274);
            _txtPassword.Location = new Point(30, 322);
            _chkShowPass.Location = new Point(30, 368);
            _lblMsg.Location = new Point(30, 392);
            _btnSubmit.Location = new Point(30, 412);
            _btnSubmit.Text = "CREATE ACCOUNT";
            _btnToggle.Location = new Point(30, 466);
            _btnToggle.Text = "Already have an account? Sign In";

            _tabRegister.BackColor = ThemeManager.CardColor;
            _tabRegister.ForeColor = ThemeManager.GoldPrimary;
            _tabLogin.BackColor = Color.Transparent;
            _tabLogin.ForeColor = ThemeManager.SubTextColor;

            _lblMsg.Text = "";
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            _lblMsg.ForeColor = ThemeManager.RedAccent;

            if (_showingLogin)
            {
                if (string.IsNullOrWhiteSpace(_txtUsername.Text) ||
                    string.IsNullOrWhiteSpace(_txtPassword.Text))
                { _lblMsg.Text = "Please fill in all fields."; return; }

                var user = _userService.Login(_txtUsername.Text.Trim(), _txtPassword.Text);
                if (user == null)
                { _lblMsg.Text = "Invalid username or password."; return; }

                _lblMsg.ForeColor = ThemeManager.GreenAccent;
                _lblMsg.Text = user.GetWelcomeMessage();

                this.Hide();
                if (user.IsAdmin)
                    new AdminDashboard(user).ShowDialog();
                else
                    new CustomerDashboard(user).ShowDialog();

                this.Show();
                ShowLogin();
                _txtUsername.Clear(); _txtPassword.Clear();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_txtFullName.Text) ||
                    string.IsNullOrWhiteSpace(_txtUsername.Text) ||
                    string.IsNullOrWhiteSpace(_txtPassword.Text))
                { _lblMsg.Text = "Full name, username and password are required."; return; }

                bool ok = _userService.Register(
                    _txtUsername.Text.Trim(), _txtPassword.Text,
                    _txtFullName.Text.Trim(),
                    _txtEmail.Text.Trim(),
                    _txtPhone.Text.Trim(),
                    "", "");

                if (!ok) { _lblMsg.Text = "Username already taken. Try another."; return; }

                _lblMsg.ForeColor = ThemeManager.GreenAccent;
                _lblMsg.Text = "Account created! Please sign in.";
                ShowLogin();
            }
        }

        private void LeftPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            // decorative gold cricket ball arc
            using (var pen = new Pen(Color.FromArgb(40, 212, 175, 55), 3))
            {
                g.DrawEllipse(pen, -60, 350, 300, 300);
                g.DrawEllipse(pen, 200, 420, 200, 200);
            }
            // gradient separator on right edge
            using (var br = new LinearGradientBrush(
                new Rectangle(415, 0, 6, _leftPanel.Height),
                ThemeManager.GoldDark, Color.Transparent, 90f))
            {
                g.FillRectangle(br, 415, 0, 5, _leftPanel.Height);
            }
        }

        private void RoundCorners(Control c, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            var rect = new Rectangle(0, 0, c.Width, c.Height);
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseAllFigures();
            c.Region = new Region(path);
        }
    }
}
