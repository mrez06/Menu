using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private sealed class Product
        {
            public string Name { get; init; }
            public string Symbol { get; init; }
            public decimal Price { get; init; }
            public override string ToString() => $"{Name} - {Price:0.00} AZN";
        }

        private readonly List<Product> products = new()
        {
            new() { Name = "Tort", Symbol = "🍰", Price = 8.50m },
            new() { Name = "Kola", Symbol = "🥤", Price = 1.50m },
            new() { Name = "Kokteyl", Symbol = "🍹", Price = 3.25m },
            new() { Name = "Burger", Symbol = "🍔", Price = 6.75m },
            new() { Name = "Sendviç", Symbol = "🥪", Price = 4.00m },
            new() { Name = "Pizza", Symbol = "🍕", Price = 10.00m },
            new() { Name = "Keks", Symbol = "🧁", Price = 2.00m },
            new() { Name = "Hot-doq", Symbol = "🌭", Price = 3.50m },
            new() { Name = "Peçenye", Symbol = "🍪", Price = 1.00m }
        };

        private readonly List<Product> cart = new();
        private readonly PictureBox[] pictures = new PictureBox[9];
        private readonly Label[] names = new Label[9];
        private readonly Label[] prices = new Label[9];
        private Panel centerPanel, leftPanel, rightPanel;
        private ListBox cartList;
        private TextBox amountBox, changeBox, totalBox;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
            Resize += (_, _) => SetMainLayout();
        }

        private void BuildInterface()
        {
            leftPanel = new Panel { Width = 220, Dock = DockStyle.Left, BackColor = Color.LightGray };
            rightPanel = new Panel { Width = 220, Dock = DockStyle.Right, BackColor = Color.LightGray };
            centerPanel = new Panel { BackColor = Color.WhiteSmoke };
            var header = new Label { Text = "MENU", Font = new Font("Segoe UI", 20, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };

            AddLeftControls();
            AddRightControls();
            AddMenuControls();
            Controls.AddRange(new Control[] { leftPanel, rightPanel, centerPanel, header });
            SetMainLayout();
        }

        private void AddLeftControls()
        {
            leftPanel.Controls.Add(new Label { Text = "Cafe", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Location = new Point(65, 10) });
            amountBox = AddTextBox(leftPanel, "Məbləğ:", 65);
            changeBox = AddTextBox(leftPanel, "Qalıq:", 130);
            changeBox.ReadOnly = true;
            var calculate = AddButton(leftPanel, "Hesabla", 10, 175, Color.ForestGreen, 100);
            var clear = AddButton(leftPanel, "Təmizlə", 115, 175, Color.DarkRed, 100);
            calculate.Click += Calculate;
            clear.Click += (_, _) => { amountBox.Clear(); changeBox.Clear(); };
        }

        private void AddRightControls()
        {
            rightPanel.Controls.Add(new Label { Text = "Səbət", Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true, Location = new Point(10, 10) });
            cartList = new ListBox { Location = new Point(10, 45), Size = new Size(200, 300), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            rightPanel.Controls.Add(cartList);
            var remove = AddButton(rightPanel, "Səbətdən sil", 10, 360, Color.LightGray);
            var refresh = AddButton(rightPanel, "Yenilə", 10, 400, Color.LightGray);
            rightPanel.Controls.Add(new Label { Text = "Hesab:", AutoSize = true, Location = new Point(10, 445) });
            totalBox = new TextBox { Location = new Point(10, 470), Width = 200, ReadOnly = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            rightPanel.Controls.Add(totalBox);
            var total = AddButton(rightPanel, "Yekun hesab", 10, 510, Color.LightGray);
            remove.Click += Remove;
            refresh.Click += RefreshForm;
            total.Click += ShowTotal;
        }

        private void AddMenuControls()
        {
            for (int i = 0; i < products.Count; i++)
            {
                int x = 10 + i % 3 * 175, y = 10 + i / 3 * 185;
                var product = products[i];
                pictures[i] = new PictureBox { Location = new Point(x + 10, y), Size = new Size(140, 100), Tag = i, Cursor = Cursors.Hand };
                pictures[i].Paint += DrawSymbol;
                names[i] = new Label { Text = product.Name, Location = new Point(x, y + 105), Size = new Size(160, 25), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10, FontStyle.Italic), Tag = i, Cursor = Cursors.Hand };
                prices[i] = new Label { Text = $"{product.Price:0.00} AZN", Location = new Point(x, y + 130), Size = new Size(160, 20), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.DimGray };
                pictures[i].Click += AddProduct;
                names[i].Click += AddProduct;
                centerPanel.Controls.AddRange(new Control[] { pictures[i], names[i], prices[i] });
            }
        }

        private void SetMainLayout()
        {
            int x = leftPanel.Width, w = Math.Max(520, ClientSize.Width - leftPanel.Width - rightPanel.Width);
            centerPanel.SetBounds(x, 50, w, Math.Max(540, ClientSize.Height - 50));
            var header = Controls.OfType<Label>().FirstOrDefault(l => l.Text == "MENU");
            header?.SetBounds(x, 0, w, 50);
        }

        private static TextBox AddTextBox(Control parent, string label, int y)
        {
            parent.Controls.Add(new Label { Text = label, AutoSize = true, Location = new Point(10, y) });
            var box = new TextBox { Location = new Point(10, y + 25), Width = 190 };
            parent.Controls.Add(box);
            return box;
        }

        private static Button AddButton(Control parent, string text, int x, int y, Color color, int width = 200)
        {
            var button = new Button { Text = text, Location = new Point(x, y), Width = width, Height = 34, BackColor = color, ForeColor = color == Color.LightGray ? Color.Black : Color.White, FlatStyle = FlatStyle.Flat };
            parent.Controls.Add(button);
            return button;
        }

        private void DrawSymbol(object sender, PaintEventArgs e)
        {
            int index = (int)((Control)sender).Tag;
            using var font = new Font("Segoe UI Emoji", 42);
            var text = products[index].Symbol;
            var size = e.Graphics.MeasureString(text, font);
            e.Graphics.DrawString(text, font, Brushes.Black, (140 - size.Width) / 2, (100 - size.Height) / 2);
        }

        private void AddProduct(object sender, EventArgs e)
        {
            var product = products[(int)((Control)sender).Tag];
            cart.Add(product);
            cartList.Items.Add(product);
        }

        private void Remove(object sender, EventArgs e)
        {
            int i = cartList.SelectedIndex;
            if (i < 0) return;
            var product = cart[i];
            cart.RemoveAt(i);
            cartList.Items.RemoveAt(i);
            MessageBox.Show($"{product.Name} səbətdən silindi");
        }

        private void RefreshForm(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xanalar sıfırlansınmı?", "Təsdiq", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            cart.Clear(); cartList.Items.Clear(); amountBox.Clear(); changeBox.Clear(); totalBox.Clear();
        }

        private void ShowTotal(object sender, EventArgs e)
        {
            if (cart.Count == 0) { MessageBox.Show("Səbətdə yemək yoxdur!"); return; }
            totalBox.Text = cart.Sum(x => x.Price).ToString("0.00");
        }

        private void Calculate(object sender, EventArgs e)
        {
            if (!decimal.TryParse(totalBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var total) || total <= 0) { MessageBox.Show("Əvvəlcədən yekun hesabı hesablayın."); return; }
            if (!decimal.TryParse(amountBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount)) { MessageBox.Show("Düzgün məbləğ daxil edin."); return; }
            if (amount < total) { MessageBox.Show("Daxil edilən məbləğ hesabdan azdır"); return; }
            changeBox.Text = (amount - total).ToString("0.00");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
