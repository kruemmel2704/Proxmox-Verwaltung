using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProxmoxVEGui
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Label lblSubtitle;
        private RoundedPanel panelCard;

        private Label lblHost;
        private ModernTextBox txtHost;

        private Label lblPort;
        private ModernTextBox txtPort;

        private Label lblUsername;
        private ModernTextBox txtUsername;

        private Label lblPassword;
        private ModernTextBox txtPassword;

        private Label lblRealm;
        private ModernComboBox cmbRealm;

        private CheckBox chkIgnoreSsl;
        private RoundedButton btnLogin;
        private Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblSubtitle = new Label();
            this.panelCard = new RoundedPanel();

            this.lblHost = new Label();
            this.txtHost = new ModernTextBox();

            this.lblPort = new Label();
            this.txtPort = new ModernTextBox();

            this.lblUsername = new Label();
            this.txtUsername = new ModernTextBox();

            this.lblPassword = new Label();
            this.txtPassword = new ModernTextBox();

            this.lblRealm = new Label();
            this.cmbRealm = new ModernComboBox();

            this.chkIgnoreSsl = new CheckBox();
            this.btnLogin = new RoundedButton();
            this.lblStatus = new Label();

            this.panelCard.SuspendLayout();
            this.SuspendLayout();

            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(10, 15, 25);
            this.ClientSize = new Size(620, 710);
            this.Font = new Font("Segoe UI", 10F);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Proxmox VE Manager - Login";
            this.AcceptButton = this.btnLogin;
            this.Load += new EventHandler(this.LoginForm_Load);

            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(40, 24);
            this.lblTitle.Text = "PROXMOX VE";

            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new Font("Segoe UI", 12F);
            this.lblSubtitle.ForeColor = Color.FromArgb(156, 163, 175);
            this.lblSubtitle.Location = new Point(43, 90);
            this.lblSubtitle.Text = "Desktop Administration Client";

            this.panelCard.BackColor = Color.FromArgb(17, 24, 39);
            this.panelCard.BorderColor = Color.FromArgb(55, 65, 81);
            this.panelCard.BorderRadius = 18;
            this.panelCard.BorderSize = 1;
            this.panelCard.Location = new Point(60, 150);
            this.panelCard.Size = new Size(500, 530);

            this.lblHost.AutoSize = true;
            this.lblHost.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            this.lblHost.ForeColor = Color.FromArgb(229, 231, 235);
            this.lblHost.Location = new Point(25, 25);
            this.lblHost.Text = "Host / Domain";

            this.txtHost.Location = new Point(25, 51);
            this.txtHost.Size = new Size(320, 44);
            this.txtHost.TabIndex = 1;

            this.lblPort.AutoSize = true;
            this.lblPort.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            this.lblPort.ForeColor = Color.FromArgb(229, 231, 235);
            this.lblPort.Location = new Point(374, 25);
            this.lblPort.Text = "Port";

            this.txtPort.Location = new Point(374, 51);
            this.txtPort.Size = new Size(100, 44);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "8006";

            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            this.lblUsername.ForeColor = Color.FromArgb(229, 231, 235);
            this.lblUsername.Location = new Point(25, 121);
            this.lblUsername.Text = "Benutzername";

            this.txtUsername.Location = new Point(25, 147);
            this.txtUsername.Size = new Size(450, 44);
            this.txtUsername.TabIndex = 3;
            this.txtUsername.Text = "root";

            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            this.lblPassword.ForeColor = Color.FromArgb(229, 231, 235);
            this.lblPassword.Location = new Point(25, 217);
            this.lblPassword.Text = "Passwort";

            this.txtPassword.Location = new Point(25, 243);
            this.txtPassword.Size = new Size(450, 44);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.PasswordChar = '●';

            this.lblRealm.AutoSize = true;
            this.lblRealm.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            this.lblRealm.ForeColor = Color.FromArgb(229, 231, 235);
            this.lblRealm.Location = new Point(25, 313);
            this.lblRealm.Text = "Authentifizierung";

            this.cmbRealm.Location = new Point(25, 339);
            this.cmbRealm.Size = new Size(450, 44);
            this.cmbRealm.TabIndex = 5;
            this.cmbRealm.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbRealm.FlatStyle = FlatStyle.Flat;
            this.cmbRealm.FormattingEnabled = true;
            this.cmbRealm.Items.AddRange(new object[]
            {
                "Linux PAM standard authentication (pam)",
                "Proxmox VE authentication server (pve)"
            });

            this.chkIgnoreSsl.AutoSize = true;
            this.chkIgnoreSsl.BackColor = Color.FromArgb(17, 24, 39);
            this.chkIgnoreSsl.Checked = true;
            this.chkIgnoreSsl.CheckState = CheckState.Checked;
            this.chkIgnoreSsl.Font = new Font("Segoe UI", 10F);
            this.chkIgnoreSsl.ForeColor = Color.FromArgb(209, 213, 219);
            this.chkIgnoreSsl.Location = new Point(25, 400);
            this.chkIgnoreSsl.TabIndex = 6;
            this.chkIgnoreSsl.Text = "SSL-Warnung ignorieren";
            this.chkIgnoreSsl.UseVisualStyleBackColor = false;

            this.btnLogin.BackColor = Color.FromArgb(249, 115, 22);
            this.btnLogin.BorderRadius = 12;
            this.btnLogin.Cursor = Cursors.Hand;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.Font = new Font("Segoe UI Semibold", 11.5F, FontStyle.Bold);
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.Location = new Point(25, 448);
            this.btnLogin.Size = new Size(450, 52);
            this.btnLogin.TabIndex = 7;
            this.btnLogin.Text = "Mit Host verbinden";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

            this.lblStatus.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            this.lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
            this.lblStatus.Location = new Point(30, 503);
            this.lblStatus.Size = new Size(450, 24);
            this.lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            this.panelCard.Controls.Add(this.lblHost);
            this.panelCard.Controls.Add(this.txtHost);
            this.panelCard.Controls.Add(this.lblPort);
            this.panelCard.Controls.Add(this.txtPort);
            this.panelCard.Controls.Add(this.lblUsername);
            this.panelCard.Controls.Add(this.txtUsername);
            this.panelCard.Controls.Add(this.lblPassword);
            this.panelCard.Controls.Add(this.txtPassword);
            this.panelCard.Controls.Add(this.lblRealm);
            this.panelCard.Controls.Add(this.cmbRealm);
            this.panelCard.Controls.Add(this.chkIgnoreSsl);
            this.panelCard.Controls.Add(this.btnLogin);
            this.panelCard.Controls.Add(this.lblStatus);

            this.Controls.Add(this.panelCard);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblTitle);

            this.panelCard.ResumeLayout(false);
            this.panelCard.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private class ModernTextBox : UserControl
        {
            private readonly TextBox innerTextBox = new TextBox();

            private static readonly Color OuterBackColor = Color.FromArgb(17, 24, 39);
            private static readonly Color InnerBackColor = Color.FromArgb(12, 18, 31);
            private static readonly Color NormalBorderColor = Color.FromArgb(75, 85, 99);
            private static readonly Color FocusBorderColor = Color.FromArgb(249, 115, 22);

            private const int BorderRadius = 11;
            private const int LeftPadding = 18;
            private const int RightPadding = 18;

            public char PasswordChar
            {
                get => innerTextBox.PasswordChar;
                set => innerTextBox.PasswordChar = value;
            }

            public override string Text
            {
                get => innerTextBox.Text;
                set
                {
                    innerTextBox.Text = value ?? "";
                    base.Text = value ?? "";
                }
            }

            public ModernTextBox()
            {
                this.SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.Selectable,
                    true
                );

                this.TabStop = true;
                this.BackColor = OuterBackColor;
                this.MinimumSize = new Size(50, 38);
                this.Size = new Size(250, 44);
                this.Cursor = Cursors.IBeam;

                innerTextBox.BorderStyle = BorderStyle.None;
                innerTextBox.BackColor = InnerBackColor;
                innerTextBox.ForeColor = Color.White;
                innerTextBox.Font = new Font("Segoe UI", 10.5F);
                innerTextBox.TabStop = false;
                innerTextBox.AutoSize = true;
                innerTextBox.Margin = Padding.Empty;
                innerTextBox.Cursor = Cursors.IBeam;

                innerTextBox.GotFocus += (s, e) => this.Invalidate();
                innerTextBox.LostFocus += (s, e) => this.Invalidate();

                innerTextBox.TextChanged += (s, e) =>
                {
                    base.Text = innerTextBox.Text;
                    this.OnTextChanged(e);
                };

                this.Controls.Add(innerTextBox);

                UpdateTextBoxBounds();
                UpdateRoundedRegion();
            }

            protected override void OnEnter(EventArgs e)
            {
                base.OnEnter(e);
                innerTextBox.Focus();
                this.Invalidate();
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                innerTextBox.Focus();
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                innerTextBox.Focus();
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                UpdateTextBoxBounds();
                UpdateRoundedRegion();
                this.Invalidate();
            }

            private void UpdateTextBoxBounds()
            {
                int innerHeight = innerTextBox.PreferredHeight;

                int y = Math.Max(8, (this.Height - innerHeight) / 2);
                int width = Math.Max(10, this.Width - LeftPadding - RightPadding);

                innerTextBox.SetBounds(
                    LeftPadding,
                    y,
                    width,
                    innerHeight
                );
            }

            private void UpdateRoundedRegion()
            {
                if (this.Width <= 0 || this.Height <= 0) return;

                using (GraphicsPath path = RoundedButton.GetRoundedPath(new Rectangle(0, 0, this.Width, this.Height), BorderRadius))
                {
                    this.Region = new Region(path);
                }
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                using (SolidBrush brush = new SolidBrush(OuterBackColor))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                bool focused = innerTextBox.Focused || this.Focused;

                Color borderColor = focused ? FocusBorderColor : NormalBorderColor;
                float borderWidth = focused ? 2F : 1F;

                Rectangle rect = new Rectangle(2, 2, this.Width - 5, this.Height - 5);

                using (GraphicsPath path = RoundedButton.GetRoundedPath(rect, BorderRadius))
                using (SolidBrush brush = new SolidBrush(InnerBackColor))
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        private class ModernComboBox : UserControl
        {
            private readonly ModernComboBoxItemCollection itemCollection;
            private int selectedIndex = -1;
            private bool isHover = false;
            private bool isOpen = false;
            private string customText = "";

            public event EventHandler SelectedIndexChanged;

            public ModernComboBoxItemCollection Items => itemCollection;

            public ComboBoxStyle DropDownStyle { get; set; } = ComboBoxStyle.DropDownList;
            public FlatStyle FlatStyle { get; set; } = FlatStyle.Flat;
            public bool FormattingEnabled { get; set; } = true;

            public int SelectedIndex
            {
                get => selectedIndex;
                set
                {
                    int newValue = value;

                    if (newValue < -1) newValue = -1;
                    if (newValue >= itemCollection.Count) newValue = itemCollection.Count - 1;

                    if (selectedIndex == newValue) return;

                    selectedIndex = newValue;
                    customText = "";
                    this.Invalidate();
                    SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public object SelectedItem
            {
                get
                {
                    if (selectedIndex < 0 || selectedIndex >= itemCollection.Count) return null;
                    return itemCollection[selectedIndex];
                }
                set
                {
                    int index = itemCollection.IndexOf(value);
                    SelectedIndex = index;
                }
            }

            public override string Text
            {
                get
                {
                    if (SelectedItem != null) return Convert.ToString(SelectedItem);
                    return customText;
                }
                set
                {
                    int index = itemCollection.FindIndex(x =>
                        string.Equals(Convert.ToString(x), value, StringComparison.OrdinalIgnoreCase));

                    if (index >= 0)
                    {
                        SelectedIndex = index;
                    }
                    else
                    {
                        selectedIndex = -1;
                        customText = value ?? "";
                        base.Text = customText;
                        this.Invalidate();
                    }
                }
            }

            public ModernComboBox()
            {
                itemCollection = new ModernComboBoxItemCollection(this);

                this.SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.Selectable,
                    true
                );

                this.TabStop = true;
                this.BackColor = Color.FromArgb(17, 24, 39);
                this.Font = new Font("Segoe UI", 10.5F);
                this.Size = new Size(250, 44);
                this.Cursor = Cursors.Hand;
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                UpdateRoundedRegion();
                this.Invalidate();
            }

            private void UpdateRoundedRegion()
            {
                if (this.Width <= 0 || this.Height <= 0) return;

                using (GraphicsPath path = RoundedButton.GetRoundedPath(new Rectangle(0, 0, this.Width, this.Height), 11))
                {
                    this.Region = new Region(path);
                }
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                isHover = true;
                this.Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                isHover = false;
                this.Invalidate();
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (e.Button == MouseButtons.Left)
                {
                    this.Focus();
                    ShowDropDownMenu();
                }
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);

                if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    ShowDropDownMenu();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Up && itemCollection.Count > 0)
                {
                    SelectedIndex = Math.Max(0, SelectedIndex - 1);
                    e.Handled = true;
                }
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                this.Invalidate();
            }

            protected override void OnLostFocus(EventArgs e)
            {
                base.OnLostFocus(e);
                this.Invalidate();
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                using (SolidBrush brush = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                bool active = this.Focused || isOpen;

                Color borderColor = active
                    ? Color.FromArgb(249, 115, 22)
                    : isHover
                        ? Color.FromArgb(107, 114, 128)
                        : Color.FromArgb(75, 85, 99);

                Rectangle rect = new Rectangle(1, 1, this.Width - 3, this.Height - 3);

                using (GraphicsPath path = RoundedButton.GetRoundedPath(rect, 11))
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(12, 18, 31)))
                using (Pen pen = new Pen(borderColor, active ? 2F : 1F))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }

                Rectangle textRect = new Rectangle(18, 0, this.Width - 62, this.Height);

                TextRenderer.DrawText(
                    e.Graphics,
                    this.Text,
                    this.Font,
                    textRect,
                    Color.White,
                    TextFormatFlags.Left |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis
                );

                DrawChevron(e.Graphics);
            }

            private void DrawChevron(Graphics g)
            {
                int centerY = this.Height / 2;
                int x = this.Width - 33;

                using (Pen pen = new Pen(Color.FromArgb(209, 213, 219), 2F))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;

                    g.DrawLine(pen, x, centerY - 3, x + 7, centerY + 4);
                    g.DrawLine(pen, x + 7, centerY + 4, x + 14, centerY - 3);
                }
            }

            private void ShowDropDownMenu()
            {
                if (itemCollection.Count <= 0) return;

                ContextMenuStrip menu = new ContextMenuStrip();
                menu.BackColor = Color.FromArgb(12, 18, 31);
                menu.ForeColor = Color.White;
                menu.Padding = new Padding(1);
                menu.ShowImageMargin = false;
                menu.Renderer = new DarkMenuRenderer();
                menu.Width = this.Width;

                for (int i = 0; i < itemCollection.Count; i++)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(Convert.ToString(itemCollection[i]));
                    item.AutoSize = false;
                    item.Size = new Size(this.Width - 2, 34);
                    item.Padding = new Padding(14, 0, 14, 0);
                    item.Margin = Padding.Empty;
                    item.ForeColor = Color.White;
                    item.BackColor = i == selectedIndex
                        ? Color.FromArgb(31, 41, 55)
                        : Color.FromArgb(12, 18, 31);
                    item.Font = new Font("Segoe UI", 10F);
                    item.Tag = i;

                    item.Click += (s, e) =>
                    {
                        ToolStripMenuItem clickedItem = (ToolStripMenuItem)s;
                        SelectedIndex = (int)clickedItem.Tag;
                    };

                    menu.Items.Add(item);
                }

                menu.Closed += (s, e) =>
                {
                    isOpen = false;
                    this.Invalidate();
                };

                isOpen = true;
                this.Invalidate();

                menu.Show(this, new Point(0, this.Height + 4));
            }

            public class ModernComboBoxItemCollection
            {
                private readonly ModernComboBox owner;
                private readonly List<object> items = new List<object>();

                public ModernComboBoxItemCollection(ModernComboBox owner)
                {
                    this.owner = owner;
                }

                public int Count => items.Count;

                public object this[int index] => items[index];

                public void Add(object item)
                {
                    items.Add(item);
                    owner.Invalidate();
                }

                public void AddRange(object[] values)
                {
                    if (values == null) return;

                    foreach (object value in values)
                    {
                        items.Add(value);
                    }

                    owner.Invalidate();
                }

                public void Clear()
                {
                    items.Clear();
                    owner.SelectedIndex = -1;
                    owner.Invalidate();
                }

                internal int IndexOf(object value)
                {
                    return items.IndexOf(value);
                }

                internal int FindIndex(Predicate<object> match)
                {
                    return items.FindIndex(match);
                }
            }

            private class DarkMenuRenderer : ToolStripProfessionalRenderer
            {
                protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
                {
                    using (Pen pen = new Pen(Color.FromArgb(75, 85, 99)))
                    {
                        Rectangle rect = new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
                        e.Graphics.DrawRectangle(pen, rect);
                    }
                }

                protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
                {
                    Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);

                    Color color = e.Item.Selected
                        ? Color.FromArgb(31, 41, 55)
                        : e.Item.BackColor;

                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                }
            }
        }

        private class RoundedPanel : Panel
        {
            public int BorderRadius { get; set; } = 16;
            public int BorderSize { get; set; } = 1;
            public Color BorderColor { get; set; } = Color.FromArgb(55, 65, 81);

            public RoundedPanel()
            {
                this.SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw,
                    true
                );
            }

            protected override void OnResize(EventArgs eventargs)
            {
                base.OnResize(eventargs);

                if (this.Width > 0 && this.Height > 0)
                {
                    using (GraphicsPath path = RoundedButton.GetRoundedPath(new Rectangle(0, 0, this.Width, this.Height), BorderRadius))
                    {
                        this.Region = new Region(path);
                    }
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

                using (GraphicsPath path = RoundedButton.GetRoundedPath(rect, BorderRadius))
                using (SolidBrush brush = new SolidBrush(this.BackColor))
                using (Pen pen = new Pen(BorderColor, BorderSize))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }
    }
}