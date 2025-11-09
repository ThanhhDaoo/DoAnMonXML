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
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Panel chính
            Panel mainPanel = new Panel
            {
                Size = new Size(380, 280),
                Location = new Point(35, 20),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Tiêu đề
            Label lblTitle = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Size = new Size(350, 40),
                Location = new Point(15, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Icon người dùng
            Label lblIcon = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 32),
                Size = new Size(60, 60),
                Location = new Point(160, 65),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Label Username
            Label lblUsername = new Label
            {
                Text = "Tên đăng nhập:",
                Font = new Font("Segoe UI", 10),
                Size = new Size(120, 25),
                Location = new Point(30, 140)
            };

            // TextBox Username
            TextBox txtUsername = new TextBox
            {
                Name = "txtUsername",
                Font = new Font("Segoe UI", 10),
                Size = new Size(200, 25),
                Location = new Point(150, 138)
            };

            // Label Password
            Label lblPassword = new Label
            {
                Text = "Mật khẩu:",
                Font = new Font("Segoe UI", 10),
                Size = new Size(120, 25),
                Location = new Point(30, 175)
            };

            // TextBox Password
            TextBox txtPassword = new TextBox
            {
                Name = "txtPassword",
                Font = new Font("Segoe UI", 10),
                Size = new Size(200, 25),
                Location = new Point(150, 173),
                PasswordChar = '●'
            };

            // Button Đăng nhập
            Button btnLogin = new Button
            {
                Text = "Đăng Nhập",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 35),
                Location = new Point(80, 220),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += (s, e) => Login(txtUsername.Text, txtPassword.Text);

            // Button Thoát
            Button btnExit = new Button
            {
                Text = "Thoát",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 35),
                Location = new Point(210, 220),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => Application.Exit();

            // Thêm controls vào panel
            mainPanel.Controls.AddRange(new Control[] {
                lblTitle, lblIcon, lblUsername, txtUsername,
                lblPassword, txtPassword, btnLogin, btnExit
            });

            // Thêm panel vào form
            this.Controls.Add(mainPanel);

            // Set focus vào txtUsername
            txtUsername.Select();

            // Xử lý phím Enter
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Login(txtUsername.Text, txtPassword.Text);
                }
            };
        }

        /// <summary>
        /// Xử lý đăng nhập
        /// </summary>
        private void Login(string username, string password)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(username))
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