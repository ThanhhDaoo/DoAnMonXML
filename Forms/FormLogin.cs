using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            SetupUI();
        }

        /// <summary>
        /// Thiết lập giao diện form
        /// </summary>
        private void SetupUI()
        {
            // Cấu hình Form
            this.Text = "Đăng Nhập - Quản Lý Thư Viện";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);

            // Panel gradient background
            Panel bgPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(41, 128, 185)
            };
            bgPanel.Paint += (s, e) =>
            {
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    bgPanel.ClientRectangle,
                    Color.FromArgb(52, 152, 219),
                    Color.FromArgb(41, 128, 185),
                    45f);
                e.Graphics.FillRectangle(brush, bgPanel.ClientRectangle);
            };

            // Panel chính với shadow effect
            Panel mainPanel = new Panel
            {
                Size = new Size(400, 480),
                Location = new Point(50, 60),
                BackColor = Color.White
            };
            mainPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, 20, 20, 180, 90);
                path.AddArc(mainPanel.Width - 20, 0, 20, 20, 270, 90);
                path.AddArc(mainPanel.Width - 20, mainPanel.Height - 20, 20, 20, 0, 90);
                path.AddArc(0, mainPanel.Height - 20, 20, 20, 90, 90);
                path.CloseFigure();
                mainPanel.Region = new Region(path);
            };

            // Icon người dùng với background circle
            Panel iconPanel = new Panel
            {
                Size = new Size(100, 100),
                Location = new Point(150, 30),
                BackColor = Color.FromArgb(52, 152, 219)
            };
            iconPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(52, 152, 219)), 0, 0, 100, 100);
            };

            Label lblIcon = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 42),
                Size = new Size(100, 100),
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            iconPanel.Controls.Add(lblIcon);

            // Tiêu đề
            Label lblTitle = new Label
            {
                Text = "ĐĂNG NHẬP",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Size = new Size(380, 40),
                Location = new Point(10, 145),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblSubtitle = new Label
            {
                Text = "Hệ thống quản lý thư viện",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(149, 165, 166),
                Size = new Size(380, 25),
                Location = new Point(10, 185),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Username Panel
            Panel userPanel = new Panel
            {
                Size = new Size(340, 50),
                Location = new Point(30, 230),
                BackColor = Color.FromArgb(236, 240, 241)
            };
            userPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, 10, 10, 180, 90);
                path.AddArc(userPanel.Width - 10, 0, 10, 10, 270, 90);
                path.AddArc(userPanel.Width - 10, userPanel.Height - 10, 10, 10, 0, 90);
                path.AddArc(0, userPanel.Height - 10, 10, 10, 90, 90);
                path.CloseFigure();
                userPanel.Region = new Region(path);
            };

            Label lblUserIcon = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 16),
                Size = new Size(40, 50),
                Location = new Point(5, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            TextBox txtUsername = new TextBox
            {
                Name = "txtUsername",
                Font = new Font("Segoe UI", 11),
                Size = new Size(285, 50),
                Location = new Point(50, 13),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(236, 240, 241),
                Text = "Tên đăng nhập"
            };
            txtUsername.GotFocus += (s, e) => { if (txtUsername.Text == "Tên đăng nhập") txtUsername.Text = ""; };
            txtUsername.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtUsername.Text)) txtUsername.Text = "Tên đăng nhập"; };

            userPanel.Controls.AddRange(new Control[] { lblUserIcon, txtUsername });

            // Password Panel
            Panel passPanel = new Panel
            {
                Size = new Size(340, 50),
                Location = new Point(30, 295),
                BackColor = Color.FromArgb(236, 240, 241)
            };
            passPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, 10, 10, 180, 90);
                path.AddArc(passPanel.Width - 10, 0, 10, 10, 270, 90);
                path.AddArc(passPanel.Width - 10, passPanel.Height - 10, 10, 10, 0, 90);
                path.AddArc(0, passPanel.Height - 10, 10, 10, 90, 90);
                path.CloseFigure();
                passPanel.Region = new Region(path);
            };

            Label lblPassIcon = new Label
            {
                Text = "🔒",
                Font = new Font("Segoe UI", 16),
                Size = new Size(40, 50),
                Location = new Point(5, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            TextBox txtPassword = new TextBox
            {
                Name = "txtPassword",
                Font = new Font("Segoe UI", 11),
                Size = new Size(285, 50),
                Location = new Point(50, 13),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(236, 240, 241),
                PasswordChar = '●'
            };

            passPanel.Controls.AddRange(new Control[] { lblPassIcon, txtPassword });

            // Button Đăng nhập với gradient
            Button btnLogin = new Button
            {
                Text = "ĐĂNG NHẬP",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(340, 50),
                Location = new Point(30, 370),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Paint += (s, e) =>
            {
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    btnLogin.ClientRectangle,
                    Color.FromArgb(46, 204, 113),
                    Color.FromArgb(39, 174, 96),
                    System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(brush, btnLogin.ClientRectangle);
                TextRenderer.DrawText(e.Graphics, btnLogin.Text, btnLogin.Font, btnLogin.ClientRectangle, btnLogin.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(39, 174, 96);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(46, 204, 113);
            btnLogin.Click += (s, e) => Login(txtUsername.Text, txtPassword.Text);

            // Button Thoát
            Button btnExit = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(35, 35),
                Location = new Point(455, 10),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.MouseEnter += (s, e) => { btnExit.BackColor = Color.FromArgb(231, 76, 60); };
            btnExit.MouseLeave += (s, e) => { btnExit.BackColor = Color.Transparent; };
            btnExit.Click += (s, e) => Application.Exit();

            // Thêm controls vào panel
            mainPanel.Controls.AddRange(new Control[] {
                iconPanel, lblTitle, lblSubtitle, userPanel, passPanel, btnLogin
            });

            bgPanel.Controls.AddRange(new Control[] { mainPanel, btnExit });
            this.Controls.Add(bgPanel);

            // Xử lý phím Enter
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Login(txtUsername.Text, txtPassword.Text);
                }
            };

            // Cho phép kéo form
            bool dragging = false;
            Point dragCursorPoint = Point.Empty;
            Point dragFormPoint = Point.Empty;

            bgPanel.MouseDown += (s, e) =>
            {
                dragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = this.Location;
            };
            bgPanel.MouseMove += (s, e) =>
            {
                if (dragging)
                {
                    Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                    this.Location = Point.Add(dragFormPoint, new Size(diff));
                }
            };
            bgPanel.MouseUp += (s, e) => { dragging = false; };
        }

        /// <summary>
        /// Xử lý đăng nhập
        /// </summary>
        private void Login(string username, string password)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(username) || username == "Tên đăng nhập")
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Kiểm tra đăng nhập
                string query = "SELECT UserID, FullName, Role FROM Users WHERE Username = @Username AND Password = @Password";
                SqlParameter[] parameters = {
                    new SqlParameter("@Username", username),
                    new SqlParameter("@Password", password)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    // Đăng nhập thành công
                    string fullName = dt.Rows[0]["FullName"].ToString();
                    string role = dt.Rows[0]["Role"].ToString();

                    MessageBox.Show($"Đăng nhập thành công!\n\nXin chào, {fullName}",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở form chính
                    this.Hide();
                    FormMain mainForm = new FormMain(fullName, role);
                    mainForm.FormClosed += (s, e) => this.Close();
                    mainForm.Show();
                }
                else
                {
                    // Đăng nhập thất bại
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đăng nhập:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormLogin
            // 
            this.ClientSize = new System.Drawing.Size(450, 350);
            this.Name = "FormLogin";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.ResumeLayout(false);

        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
    }
}