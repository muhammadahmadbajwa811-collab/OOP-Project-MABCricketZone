// ============================================
// UILayer/EditProductForm.cs
// ============================================
using System;
using System.Drawing;
using System.Windows.Forms;
using MABCricketZone.BusinessLayer;

namespace MABCricketZone.UILayer
{
    public class EditProductForm : Form
    {
        private Product _product;
        private ProductService _svc;

        private TextBox _txtName, _txtBrand, _txtCategory, _txtDesc, _txtPrice, _txtQty, _txtImage;
        private Label _lblMsg;

        public EditProductForm(Product product, ProductService svc)
        {
            _product = product;
            _svc = svc;
            InitForm();
            BuildUI();
            LoadData();
        }

        private void InitForm()
        {
            Text = "Edit Product";
            Size = new Size(640, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            BackColor = ThemeManager.BgColor;
        }

        private void BuildUI()
        {
            var card = new Panel
            {
                Width = 580, Height = 540,
                Location = new Point(20, 20),
                BackColor = ThemeManager.CardColor,
            };

            var title = new Label
            {
                Text = $"✏  Edit — {_product.Brand} {_product.Name}",
                Font = ThemeManager.FontSubHeading,
                ForeColor = ThemeManager.GoldPrimary,
                BackColor = Color.Transparent,
                AutoSize = true, Location = new Point(20, 16),
            };
            card.Controls.Add(title);

            int y = 56; int lw = 130; int fw = 400;
            Label Lbl(string t) => new Label { Text = t, Width = lw, Height = 28, Location = new Point(20, y + 4), BackColor = Color.Transparent, ForeColor = ThemeManager.SubTextColor, Font = ThemeManager.FontSmall, TextAlign = ContentAlignment.MiddleLeft };
            TextBox Fld() => new TextBox { Width = fw, Height = 34, Location = new Point(lw + 30, y), BackColor = ThemeManager.SurfaceColor, ForeColor = ThemeManager.TextColor, BorderStyle = BorderStyle.FixedSingle, Font = ThemeManager.FontBody };

            card.Controls.Add(Lbl("Product Name")); _txtName = Fld(); card.Controls.Add(_txtName); y += 48;
            card.Controls.Add(Lbl("Brand")); _txtBrand = Fld(); card.Controls.Add(_txtBrand); y += 48;
            card.Controls.Add(Lbl("Category")); _txtCategory = Fld(); card.Controls.Add(_txtCategory); y += 48;
            card.Controls.Add(Lbl("Price (Rs.)")); _txtPrice = Fld(); card.Controls.Add(_txtPrice); y += 48;
            card.Controls.Add(Lbl("Quantity")); _txtQty = Fld(); card.Controls.Add(_txtQty); y += 48;

            var lDesc = Lbl("Description"); card.Controls.Add(lDesc);
            _txtDesc = new TextBox { Multiline = true, Width = fw, Height = 70, Location = new Point(lw + 30, y), BackColor = ThemeManager.SurfaceColor, ForeColor = ThemeManager.TextColor, BorderStyle = BorderStyle.FixedSingle, Font = ThemeManager.FontBody };
            card.Controls.Add(_txtDesc); y += 84;

            var lImg = Lbl("Image Path"); card.Controls.Add(lImg);
            _txtImage = new TextBox { Width = fw - 100, Height = 34, Location = new Point(lw + 30, y), BackColor = ThemeManager.SurfaceColor, ForeColor = ThemeManager.TextColor, BorderStyle = BorderStyle.FixedSingle, Font = ThemeManager.FontBody };
            card.Controls.Add(_txtImage);
            var btnBrowse = ThemeManager.MakeOutlineButton("Browse", 90, 34);
            btnBrowse.Location = new Point(lw + 30 + fw - 94, y);
            btnBrowse.Click += (s, e) => { var dlg = new OpenFileDialog { Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp" }; if (dlg.ShowDialog() == DialogResult.OK) _txtImage.Text = dlg.FileName; };
            card.Controls.Add(btnBrowse); y += 48;

            _lblMsg = new Label { Width = 540, Height = 22, Location = new Point(20, y), BackColor = Color.Transparent, Font = ThemeManager.FontSmall };
            card.Controls.Add(_lblMsg);
            y += 28;

            var btnSave = ThemeManager.MakeGoldButton("💾  SAVE CHANGES", 200, 42);
            btnSave.Location = new Point(20, y);
            btnSave.Click += BtnSave_Click;
            card.Controls.Add(btnSave);

            var btnCancel = ThemeManager.MakeOutlineButton("Cancel", 100, 42);
            btnCancel.Location = new Point(228, y);
            btnCancel.Click += (s, e) => this.Close();
            card.Controls.Add(btnCancel);

            this.Controls.Add(card);
        }

        private void LoadData()
        {
            _txtName.Text = _product.Name;
            _txtBrand.Text = _product.Brand;
            _txtCategory.Text = _product.Category;
            _txtPrice.Text = _product.Price.ToString();
            _txtQty.Text = _product.Quantity.ToString();
            _txtDesc.Text = _product.Description;
            _txtImage.Text = _product.ImagePath;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text))
            { _lblMsg.ForeColor = ThemeManager.RedAccent; _lblMsg.Text = "Product name is required."; return; }

            decimal.TryParse(_txtPrice.Text, out decimal price);
            int.TryParse(_txtQty.Text, out int qty);

            _product.Name = _txtName.Text.Trim();
            _product.Brand = _txtBrand.Text.Trim();
            _product.Category = _txtCategory.Text.Trim();
            _product.Description = _txtDesc.Text.Trim();
            _product.Price = price;
            _product.Quantity = qty;
            _product.ImagePath = _txtImage.Text.Trim();

            if (_svc.UpdateProduct(_product))
            {
                _lblMsg.ForeColor = ThemeManager.GreenAccent;
                _lblMsg.Text = "Product updated successfully!";
            }
            else
            {
                _lblMsg.ForeColor = ThemeManager.RedAccent;
                _lblMsg.Text = "Update failed. Try again.";
            }
        }
    }
}
