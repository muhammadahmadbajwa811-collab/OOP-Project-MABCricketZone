// ============================================
// UILayer/ThemeManager.cs
// ============================================
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MABCricketZone.UILayer
{
    public enum AppTheme { Dark, Light }

    public static class ThemeManager
    {
        public static AppTheme CurrentTheme { get; private set; } = AppTheme.Dark;

        // ── DARK THEME COLORS ──
        public static Color DarkBg          = Color.FromArgb(10, 10, 15);
        public static Color DarkSurface     = Color.FromArgb(18, 18, 25);
        public static Color DarkCard        = Color.FromArgb(26, 26, 36);
        public static Color DarkBorder      = Color.FromArgb(45, 45, 60);
        public static Color DarkText        = Color.FromArgb(240, 240, 240);
        public static Color DarkSubText     = Color.FromArgb(160, 160, 180);
        public static Color GoldPrimary     = Color.FromArgb(212, 175, 55);
        public static Color GoldLight       = Color.FromArgb(255, 215, 80);
        public static Color GoldDark        = Color.FromArgb(160, 130, 30);
        public static Color GreenAccent     = Color.FromArgb(34, 197, 94);
        public static Color RedAccent       = Color.FromArgb(239, 68, 68);

        // ── LIGHT THEME COLORS ──
        public static Color LightBg         = Color.FromArgb(245, 245, 250);
        public static Color LightSurface    = Color.FromArgb(255, 255, 255);
        public static Color LightCard       = Color.FromArgb(235, 235, 245);
        public static Color LightBorder     = Color.FromArgb(200, 200, 215);
        public static Color LightText       = Color.FromArgb(20, 20, 30);
        public static Color LightSubText    = Color.FromArgb(90, 90, 110);

        // ── RESOLVED COLORS (change with theme) ──
        public static Color BgColor        => CurrentTheme == AppTheme.Dark ? DarkBg       : LightBg;
        public static Color SurfaceColor   => CurrentTheme == AppTheme.Dark ? DarkSurface  : LightSurface;
        public static Color CardColor      => CurrentTheme == AppTheme.Dark ? DarkCard     : LightCard;
        public static Color BorderColor    => CurrentTheme == AppTheme.Dark ? DarkBorder   : LightBorder;
        public static Color TextColor      => CurrentTheme == AppTheme.Dark ? DarkText     : LightText;
        public static Color SubTextColor   => CurrentTheme == AppTheme.Dark ? DarkSubText  : LightSubText;

        // ── FONTS ──
        public static Font FontTitle      = new Font("Segoe UI", 26f, FontStyle.Bold);
        public static Font FontHeading    = new Font("Segoe UI", 16f, FontStyle.Bold);
        public static Font FontSubHeading = new Font("Segoe UI", 12f, FontStyle.Bold);
        public static Font FontBody       = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static Font FontSmall      = new Font("Segoe UI", 8f,  FontStyle.Regular);
        public static Font FontBold       = new Font("Segoe UI", 10f, FontStyle.Bold);
        public static Font FontMono       = new Font("Consolas",  9f, FontStyle.Regular);
        public static Font FontLogo       = new Font("Segoe UI", 20f, FontStyle.Bold);
        public static Font FontPrice      = new Font("Segoe UI", 13f, FontStyle.Bold);

        public static void Toggle()
        {
            CurrentTheme = CurrentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        }

        public static void Apply(Form form)
        {
            form.BackColor = BgColor;
            form.ForeColor = TextColor;
            ApplyToControls(form.Controls);
        }

        private static void ApplyToControls(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c is Panel || c is GroupBox || c is TabPage)
                {
                    c.BackColor = SurfaceColor;
                    c.ForeColor = TextColor;
                }
                else if (c is Label lbl)
                {
                    lbl.ForeColor = TextColor;
                    lbl.BackColor = Color.Transparent;
                }
                else if (c is TextBox tb)
                {
                    tb.BackColor = CardColor;
                    tb.ForeColor = TextColor;
                    tb.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (c is ComboBox cb)
                {
                    cb.BackColor = CardColor;
                    cb.ForeColor = TextColor;
                    cb.FlatStyle = FlatStyle.Flat;
                }
                else if (c is DataGridView dgv)
                {
                    dgv.BackgroundColor = SurfaceColor;
                    dgv.DefaultCellStyle.BackColor = CardColor;
                    dgv.DefaultCellStyle.ForeColor = TextColor;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = DarkBorder;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = GoldPrimary;
                    dgv.GridColor = BorderColor;
                }
                if (c.HasChildren)
                    ApplyToControls(c.Controls);
            }
        }

        // Styled Button factory
        public static Button MakeGoldButton(string text, int w = 160, int h = 42)
        {
            var btn = new Button
            {
                Text = text, Width = w, Height = h,
                FlatStyle = FlatStyle.Flat,
                BackColor = GoldPrimary,
                ForeColor = Color.FromArgb(10, 10, 15),
                Font = FontBold,
                Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => ((Button)s).BackColor = GoldLight;
            btn.MouseLeave += (s, e) => ((Button)s).BackColor = GoldPrimary;
            return btn;
        }

        public static Button MakeOutlineButton(string text, int w = 160, int h = 42)
        {
            var btn = new Button
            {
                Text = text, Width = w, Height = h,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = GoldPrimary,
                Font = FontBold,
                Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderColor = GoldPrimary;
            btn.FlatAppearance.BorderSize = 1;
            btn.MouseEnter += (s, e) => { ((Button)s).BackColor = GoldPrimary; ((Button)s).ForeColor = DarkBg; };
            btn.MouseLeave += (s, e) => { ((Button)s).BackColor = Color.Transparent; ((Button)s).ForeColor = GoldPrimary; };
            return btn;
        }

        public static Button MakeDangerButton(string text, int w = 140, int h = 38)
        {
            var btn = new Button
            {
                Text = text, Width = w, Height = h,
                FlatStyle = FlatStyle.Flat,
                BackColor = RedAccent,
                ForeColor = Color.White,
                Font = FontBold,
                Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => ((Button)s).BackColor = Color.FromArgb(220, 50, 50);
            btn.MouseLeave += (s, e) => ((Button)s).BackColor = RedAccent;
            return btn;
        }

        public static TextBox MakeTextBox(string placeholder = "", int w = 280, int h = 38)
        {
            var tb = new TextBox
            {
                Width = w, Height = h,
                BackColor = CardColor,
                ForeColor = TextColor,
                BorderStyle = BorderStyle.FixedSingle,
                Font = FontBody,
            };
            return tb;
        }

        public static Panel MakeCard(int w, int h)
        {
            var p = new Panel
            {
                Width = w, Height = h,
                BackColor = CardColor,
                Padding = new Padding(12),
            };
            return p;
        }

        public static Label MakeLabel(string text, Font font = null, Color? color = null)
        {
            return new Label
            {
                Text = text,
                Font = font ?? FontBody,
                ForeColor = color ?? TextColor,
                BackColor = Color.Transparent,
                AutoSize = true,
            };
        }
    }
}
