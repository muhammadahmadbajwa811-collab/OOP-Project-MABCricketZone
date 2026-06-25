// ============================================
// Program.cs — Entry Point
// ============================================
using System;
using System.Windows.Forms;
using MABCricketZone.UILayer;
using MABCricketZone.DataLayer;

namespace MABCricketZone
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // .NET 10 WinForms bootstrap: replaces EnableVisualStyles() +
            // SetCompatibleTextRenderingDefault(false) which are obsolete in .NET 6+.
            // Visual styles, font and text rendering are now declared in the .csproj
            // via ApplicationVisualStyles, ApplicationDefaultFont and
            // ApplicationUseCompatibleTextRendering properties, and applied here.
            ApplicationConfiguration.Initialize();

            // First launch: show DB config
            var db = new DatabaseConnection();
            if (!db.TestConnection())
            {
                var setup = new DbSetupForm();
                if (setup.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("Cannot connect to database. Application will exit.",
                        "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Application.Run(new LoginForm());
        }
    }
}

// ============================================
// UILayer/DbSetupForm.cs — First-run DB Config
// ============================================
namespace MABCricketZone.UILayer
{
    using System.Drawing;
    using System.Windows.Forms;
    using MABCricketZone.DataLayer;

    public class DbSetupForm : Form
    {
        private TextBox _txtServer, _txtDatabase, _txtUser, _txtPassword;
        private Label _lblMsg;

        public DbSetupForm()
        {
            Text = "Database Setup — MAB Cricket Zone";
            Size = new Size(480, 440);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = ThemeManager.DarkBg;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var logo = new Label { Text = "🏏 MAB Cricket Zone", Font = new Font("Segoe UI", 16f, FontStyle.Bold), ForeColor = ThemeManager.GoldPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(100, 20) };
            var sub = new Label { Text = "MySQL Database Configuration", Font = ThemeManager.FontBody, ForeColor = ThemeManager.DarkSubText, BackColor = Color.Transparent, AutoSize = true, Location = new Point(140, 50) };

            var card = new Panel { Width = 420, Height = 310, Location = new Point(20, 80), BackColor = ThemeManager.DarkCard };

            int y = 16;
            Label Lbl(string t) => new Label { Text = t, AutoSize = true, Location = new Point(16, y), Font = ThemeManager.FontSmall, ForeColor = ThemeManager.DarkSubText, BackColor = Color.Transparent };
            TextBox Fld(string ph = "") => new TextBox { Width = 386, Height = 34, Location = new Point(16, y + 20), BackColor = ThemeManager.DarkSurface, ForeColor = ThemeManager.DarkText, BorderStyle = BorderStyle.FixedSingle, Font = ThemeManager.FontBody, PlaceholderText = ph };

            card.Controls.Add(Lbl("Server")); _txtServer = Fld("localhost"); _txtServer.Text = "localhost"; card.Controls.Add(_txtServer); y += 60;
            card.Controls.Add(Lbl("Database")); _txtDatabase = Fld("MABCricketZone"); _txtDatabase.Text = "MABCricketZone"; card.Controls.Add(_txtDatabase); y += 60;
            card.Controls.Add(Lbl("Username")); _txtUser = Fld("root"); _txtUser.Text = "root"; card.Controls.Add(_txtUser); y += 60;
            card.Controls.Add(Lbl("Password")); _txtPassword = Fld("leave blank if none"); _txtPassword.UseSystemPasswordChar = true; card.Controls.Add(_txtPassword); y += 60;

            _lblMsg = new Label { Width = 386, Height = 20, Location = new Point(16, y), BackColor = Color.Transparent, Font = ThemeManager.FontSmall, ForeColor = ThemeManager.RedAccent };
            card.Controls.Add(_lblMsg);

            var btnConnect = ThemeManager.MakeGoldButton("Connect & Continue", 200, 42);
            btnConnect.Location = new Point(110, 396);
            btnConnect.Click += (s, e) =>
            {
                DatabaseConnection.SetConnectionString(
                    _txtServer.Text.Trim(), _txtDatabase.Text.Trim(),
                    _txtUser.Text.Trim(), _txtPassword.Text);
                var db = new DatabaseConnection();
                if (db.TestConnection()) { this.DialogResult = DialogResult.OK; this.Close(); }
                else { _lblMsg.Text = "Connection failed. Check credentials."; }
            };

            this.Controls.Add(logo);
            this.Controls.Add(sub);
            this.Controls.Add(card);
            this.Controls.Add(btnConnect);
        }
    }
}
