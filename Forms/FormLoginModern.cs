using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormLoginModern : Form
    {
        private Panel mainPanel;
        private Panel userPanel;
        private Panel passPanel;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnClose;

        public FormLoginModern()
        {
            InitializeComponent();
            SetupModernUI();
        }

        private void SetupModernUI()
        {
            // Form configuration
            this.Text = "";
            this.Size = new Size(500, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(52, 152, 219);

            // Background gradient panel
            Panel bgPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ModernUIHelper.Colors.Primary
            };
            bgPanel.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    bgPanel.ClientRectangle,
                    ModernUIHelper.Colors.Primary,
                    ModernUIHelper.Colors.PrimaryDark,
                    45f))
                {
                    e.Graphics.FillRectangle(brush, bgPanel.ClientRectangle);
                }
            };

            // Main white card
            mainPanel = new Panel
            {
                Size = new Size(420, 550),
                Location = new Point(40, 50),
                BackColor = Color.White
            };
            mainPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(mainPanel.ClientRectangle, 25))
                {
                    mainPanel.Region = new Region(path);
                    
                    // Shadow
                    Rectangle shadowRect = new Rectangle(5, 5, mainPanel.Width - 10, mainPanel.Height - 10);
                    using (GraphicsPath shadowPath = GetRoundedRectangle(shadowRect, 25))
                    {
                        using (PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath))
                        {
                            shadowBrush.CenterColor = Color.FromArgb(80, 0, 0, 0);
                            shadowBrush.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                        }
                    }
                }
            };

            // Logo/Icon circle
            Panel iconCircle = ModernUIHelper.CreateCircularIcon("👤", ModernUIHelper.Colors.Primary, 120);
            iconCircle.Location = new Point(150, 40);

            // Title
            Label lblTitle = new Label
            {
                Text = "ĐĂNG NHẬP",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Size = new Size(400, 50),
                Location = new Point(10, 180),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblSubtitle = new Label
            {
                Text = "Hệ thống quản lý thư viện",
                Font = new Font("Segoe UI", 12),
                ForeColor = ModernUIHelper.Colors.Gray,
                Size = new Size(400, 30),
                Location = new Point(10, 230),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Username field
            Label lblUser = new Label
            {
                Text = "Tên đăng nhập",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(40, 280),
                AutoSize = true
            };

            userPanel = CreateInputField("👤", "Nhập tên đăng nhập");
            userPanel.Location = new Point(40, 305);
            txtUsername = (TextBox)userPanel.Tag;

            // Password field
            Label lblPass = new Label
            {
                Text = "Mật khẩu",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(40, 365),
                AutoSize = true
            };

            passPanel = CreateInputField("🔒", "Nhập mật khẩu", true);
            passPanel.Location = new Point(40, 390);
            txtPassword = (TextBox)passPanel.Tag;

            // Remember me checkbox
            CheckBox chkRemember = new CheckBox
            {
                Text = "Ghi nhớ đăng nhập",
                Font = new Font("Segoe UI", 9),
                ForeColor = ModernUIHelper.Colors.Gray,
                Location = new Point(40, 445),
                AutoSize = true
            };

            // Login button
            btnLogin = ModernUIHelper.CreateModernButton("ĐĂNG NHẬP", ModernUIHelper.Colors.Success, 340, 50);
            btnLogin.Location = new Point(40, 475);
            btnLogin.Click += BtnLogin_Click;

            // Close button
            btnClose = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Size = new Size(40, 40),
                Location = new Point(445, 10),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = ModernUIHelper.Colors.Danger;
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.Transparent;
            btnClose.Click += (s, e) => Application.Exit();

            // Add controls
            mainPanel.Controls.AddRange(new Control[] {
                iconCircle, lblTitle, lblSubtitle,
                lblUser, userPanel, lblPass, passPanel,
                chkRemember, btnLogin
            });

            bgPanel.Controls.AddRange(new Control[] { mainPanel, btnClose });
            this.Controls.Add(bgPanel);

            // Make form draggable
            ModernUIHelper.MakeFormDraggable(this, bgPanel);
            ModernUIHelper.MakeFormDraggable(this, mainPanel);

            // Enter key handling
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    BtnLogin_Click(null, null);
            };
        }

        private Panel CreateInputField(string icon, string placeholder, bool isPassword = false)
        {
            Panel panel = new Panel
            {
                Size = new Size(340, 50),
                BackColor = ModernUIHelper.Colors.Light
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(panel.ClientRectangle, 10))
                {
                    panel.Region = new Region(path);
                }
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 18),
                Size = new Size(45, 50),
                Location = new Point(5, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            TextBox textBox = new TextBox
            {
                Location = new Point(55, 13),
                Size = new Size(275, 25),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.None,
                BackColor = ModernUIHelper.Colors.Light,
                Text = placeholder,
                ForeColor = ModernUIHelper.Colors.Gray
            };

            if (isPassword)
                textBox.PasswordChar = '●';

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = ModernUIHelper.Colors.Dark;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = ModernUIHelper.Colors.Gray;
                    if (isPassword) textBox.PasswordChar = '\0';
                }
                else if (isPassword)
                {
                    textBox.PasswordChar = '●';
                }
            };

            textBox.TextChanged += (s, e) =>
            {
                if (isPassword && textBox.Text != placeholder && !string.IsNullOrEmpty(textBox.Text))
                    textBox.PasswordChar = '●';
            };

            panel.Controls.AddRange(new Control[] { lblIcon, textBox });
            panel.Tag = textBox;
            return panel;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || username == "Nhập tên đăng nhập")
            {
                ShowModernMessage("Vui lòng nhập tên đăng nhập!", "warning");
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password == "Nhập mật khẩu")
            {
                ShowModernMessage("Vui lòng nhập mật khẩu!", "warning");
                return;
            }

            try
            {
                string query = "SELECT UserID, FullName, Role FROM Users WHERE Username = @Username AND Password = @Password";
                SqlParameter[] parameters = {
                    new SqlParameter("@Username", username),
                    new SqlParameter("@Password", password)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    string fullName = dt.Rows[0]["FullName"].ToString();
                    string role = dt.Rows[0]["Role"].ToString();

                    ShowModernMessage($"Đăng nhập thành công!\n\nXin chào, {fullName}", "success");

                    this.Hide();
                    FormMainModern mainForm = new FormMainModern(fullName, role);
                    mainForm.FormClosed += (s, ev) => this.Close();
                    mainForm.Show();
                }
                else
                {
                    ShowModernMessage("Tên đăng nhập hoặc mật khẩu không đúng!", "error");
                }
            }
            catch (Exception ex)
            {
                ShowModernMessage($"Lỗi đăng nhập:\n{ex.Message}", "error");
            }
        }

        private void ShowModernMessage(string message, string type)
        {
            MessageBoxIcon icon = MessageBoxIcon.Information;
            if (type == "error") icon = MessageBoxIcon.Error;
            else if (type == "warning") icon = MessageBoxIcon.Warning;

            MessageBox.Show(message, type == "success" ? "Thành công" : type == "error" ? "Lỗi" : "Thông báo",
                MessageBoxButtons.OK, icon);
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(500, 650);
            this.Name = "FormLoginModern";
            this.ResumeLayout(false);
        }
    }
}
