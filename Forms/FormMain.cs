using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryManagement.Forms
{
    public partial class FormMain : Form
    {
        private string userName;
        private string userRole;

        public FormMain(string userName, string userRole)
        {
            InitializeComponent();
            this.userName = userName;
            this.userRole = userRole;
            SetupUI();
        }

        private void SetupUI()
        {
            // Cấu hình Form
            this.Text = "Hệ Thống Quản Lý Thư Viện";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.IsMdiContainer = true;

            // Menu Strip
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.FromArgb(52, 73, 94);
            menuStrip.ForeColor = Color.White;
            menuStrip.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            menuStrip.Padding = new Padding(10, 5, 0, 5);

            // Menu Hệ thống
            ToolStripMenuItem menuSystem = new ToolStripMenuItem("⚙️ Hệ thống");
            menuSystem.DropDownItems.Add(CreateMenuItem("Đăng xuất", MenuLogout_Click));
            menuSystem.DropDownItems.Add(new ToolStripSeparator());
            menuSystem.DropDownItems.Add(CreateMenuItem("Thoát", (s, e) => Application.Exit()));

            // Menu Quản lý
            ToolStripMenuItem menuManage = new ToolStripMenuItem("📚 Quản lý");
            menuManage.DropDownItems.Add(CreateMenuItem("Quản lý Sách", MenuBooks_Click));
            menuManage.DropDownItems.Add(CreateMenuItem("Quản lý Độc giả", MenuMembers_Click));
            menuManage.DropDownItems.Add(CreateMenuItem("Quản lý Mượn/Trả", MenuLoans_Click));

            // Menu Báo cáo
            ToolStripMenuItem menuReport = new ToolStripMenuItem("📊 Báo cáo");
            menuReport.DropDownItems.Add(CreateMenuItem("Thống kê Sách", MenuReportBooks_Click));
            menuReport.DropDownItems.Add(CreateMenuItem("Thống kê Mượn/Trả", MenuReportLoans_Click));

            // Menu Trợ giúp
            ToolStripMenuItem menuHelp = new ToolStripMenuItem("❓ Trợ giúp");
            menuHelp.DropDownItems.Add(CreateMenuItem("Hướng dẫn sử dụng", (s, e) => ShowHelp()));
            menuHelp.DropDownItems.Add(CreateMenuItem("Về chúng tôi", MenuAbout_Click));

            menuStrip.Items.Add(menuSystem);
            menuStrip.Items.Add(menuManage);
            menuStrip.Items.Add(menuReport);
            menuStrip.Items.Add(menuHelp);

            // Status Strip
            StatusStrip statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.FromArgb(52, 73, 94);
            statusStrip.ForeColor = Color.White;
            statusStrip.Font = new Font("Segoe UI", 9);

            ToolStripStatusLabel lblUser = new ToolStripStatusLabel("👤 Người dùng: " + userName);
            ToolStripStatusLabel lblRole = new ToolStripStatusLabel("| Vai trò: " + userRole);
            ToolStripStatusLabel lblTime = new ToolStripStatusLabel();

            // Timer để cập nhật giờ
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) => lblTime.Text = "| 🕐 " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            timer.Start();

            statusStrip.Items.Add(lblUser);
            statusStrip.Items.Add(lblRole);
            statusStrip.Items.Add(lblTime);

            // Panel chính
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.White;

            // Logo và tiêu đề
            Label lblTitle = new Label();
            lblTitle.Text = "📚 HỆ THỐNG QUẢN LÝ THƯ VIỆN";
            lblTitle.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(41, 128, 185);
            lblTitle.Size = new Size(800, 60);
            lblTitle.Location = new Point(100, 80);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Label lblWelcome = new Label();
            lblWelcome.Text = "Xin chào, " + userName + "! Chào mừng bạn đến với hệ thống.";
            lblWelcome.Font = new Font("Segoe UI", 14);
            lblWelcome.ForeColor = Color.FromArgb(127, 140, 141);
            lblWelcome.Size = new Size(800, 40);
            lblWelcome.Location = new Point(100, 150);
            lblWelcome.TextAlign = ContentAlignment.MiddleCenter;

            // Các nút chức năng chính
            int btnY = 220;
            int btnSpacing = 110;

            Button btnBooks = CreateDashboardButton("📚\n\nQuản lý\nSách", 200, btnY, Color.FromArgb(52, 152, 219));
            btnBooks.Click += MenuBooks_Click;

            Button btnMembers = CreateDashboardButton("👥\n\nQuản lý\nĐộc giả", 200 + btnSpacing, btnY, Color.FromArgb(46, 204, 113));
            btnMembers.Click += MenuMembers_Click;

            Button btnLoans = CreateDashboardButton("📝\n\nMượn/Trả\nSách", 200 + btnSpacing * 2, btnY, Color.FromArgb(155, 89, 182));
            btnLoans.Click += MenuLoans_Click;

            Button btnReport = CreateDashboardButton("📊\n\nBáo cáo\nThống kê", 200 + btnSpacing * 3, btnY, Color.FromArgb(230, 126, 34));
            btnReport.Click += MenuReportBooks_Click;

            // Thông tin hướng dẫn
            Label lblInfo = new Label();
            lblInfo.Text = "💡 Sử dụng menu ở trên hoặc click vào các nút bên dưới để bắt đầu làm việc.\n\n" +
                           "✨ Tính năng nổi bật:\n" +
                           "   • Import/Export dữ liệu với XML\n" +
                           "   • Quản lý Sách, Độc giả và Mượn/Trả\n" +
                           "   • Báo cáo và thống kê chi tiết\n" +
                           "   • Giao diện hiện đại và thân thiện";
            lblInfo.Font = new Font("Segoe UI", 10);
            lblInfo.ForeColor = Color.FromArgb(149, 165, 166);
            lblInfo.Size = new Size(800, 120);
            lblInfo.Location = new Point(100, 350);
            lblInfo.TextAlign = ContentAlignment.TopCenter;

            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(lblWelcome);
            mainPanel.Controls.Add(btnBooks);
            mainPanel.Controls.Add(btnMembers);
            mainPanel.Controls.Add(btnLoans);
            mainPanel.Controls.Add(btnReport);
            mainPanel.Controls.Add(lblInfo);

            // Thêm controls vào form
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(mainPanel);
            this.Controls.Add(statusStrip);
            this.Controls.Add(menuStrip);
        }

        private ToolStripMenuItem CreateMenuItem(string text, EventHandler clickHandler)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            item.BackColor = Color.White;
            item.ForeColor = Color.Black;
            item.Font = new Font("Segoe UI", 9);
            item.Click += clickHandler;
            return item;
        }

        private Button CreateDashboardButton(string text, int x, int y, Color backColor)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(100, 100);
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // XỬ LÝ SỰ KIỆN MENU
        private void MenuBooks_Click(object sender, EventArgs e)
        {
            FormBooks formBooks = new FormBooks();
            formBooks.ShowDialog();
        }

        private void MenuMembers_Click(object sender, EventArgs e)
        {
            FormMembers formMembers = new FormMembers();
            formMembers.ShowDialog();
        }

        private void MenuLoans_Click(object sender, EventArgs e)
        {
            FormLoans formLoans = new FormLoans();
            formLoans.ShowDialog();
        }

        private void MenuReportBooks_Click(object sender, EventArgs e)
        {
            FormReportBooks formReport = new FormReportBooks();
            formReport.ShowDialog();
        }

        private void MenuReportLoans_Click(object sender, EventArgs e)
        {
            FormReportLoans formReport = new FormReportLoans();
            formReport.ShowDialog();
        }

        private void MenuLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn đăng xuất?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
                FormLogin loginForm = new FormLogin();
                loginForm.Show();
            }
        }

        private void MenuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("HỆ THỐNG QUẢN LÝ THƯ VIỆN\n\n" +
                "Phiên bản: 1.0\n" +
                "Công nghệ: C# Winform + SQL Server + XML\n\n" +
                "Tính năng:\n" +
                "✓ Quản lý Sách, Độc giả, Mượn/Trả\n" +
                "✓ Import/Export dữ liệu XML\n" +
                "✓ Báo cáo và thống kê chi tiết\n\n" +
                "Phát triển bởi: Sinh viên CNTT\n" +
                "Đồ án môn học: Lập trình XML\n\n" +
                "© 2024 - Library Management System",
                "Về chúng tôi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowHelp()
        {
            string helpText = "HƯỚNG DẪN SỬ DỤNG HỆ THỐNG\n\n" +
                "📚 QUẢN LÝ SÁCH:\n" +
                "- Thêm/Sửa/Xóa thông tin sách\n" +
                "- Export danh sách sách ra file XML\n" +
                "- Import sách từ file XML vào hệ thống\n\n" +
                "👥 QUẢN LÝ ĐỘC GIẢ:\n" +
                "- Quản lý thông tin độc giả\n" +
                "- Export/Import dữ liệu độc giả\n\n" +
                "📝 MƯỢN/TRẢ SÁCH:\n" +
                "- Tạo phiếu mượn sách\n" +
                "- Xử lý trả sách\n" +
                "- Theo dõi sách quá hạn\n\n" +
                "📊 BÁO CÁO:\n" +
                "- Thống kê sách theo thể loại\n" +
                "- Báo cáo sách phổ biến\n" +
                "- Thống kê mượn/trả theo thời gian\n" +
                "- Danh sách sách quá hạn";

            MessageBox.Show(helpText, "Hướng dẫn sử dụng",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Name = "FormMain";
            this.ResumeLayout(false);
        }
    }
}