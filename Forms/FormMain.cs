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
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.IsMdiContainer = true;
            this.WindowState = FormWindowState.Maximized;

            // Menu Strip với gradient
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.FromArgb(44, 62, 80);
            menuStrip.ForeColor = Color.White;
            menuStrip.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            menuStrip.Padding = new Padding(15, 8, 0, 8);

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

            // Panel chính với gradient background
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.White;
            mainPanel.Paint += (s, e) =>
            {
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    mainPanel.ClientRectangle,
                    Color.FromArgb(236, 240, 241),
                    Color.FromArgb(255, 255, 255),
                    90f);
                e.Graphics.FillRectangle(brush, mainPanel.ClientRectangle);
            };

            // Header Panel
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 180,
                BackColor = Color.Transparent
            };

            // Logo và tiêu đề
            Label lblTitle = new Label();
            lblTitle.Text = "📚 HỆ THỐNG QUẢN LÝ THƯ VIỆN";
            lblTitle.Font = new Font("Segoe UI", 32, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.AutoSize = false;
            lblTitle.Size = new Size(900, 70);
            lblTitle.Location = new Point((this.Width - 900) / 2, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Anchor = AnchorStyles.Top;

            Label lblWelcome = new Label();
            lblWelcome.Text = "Xin chào, " + userName + "! 👋 Chào mừng bạn đến với hệ thống";
            lblWelcome.Font = new Font("Segoe UI", 14);
            lblWelcome.ForeColor = Color.FromArgb(127, 140, 141);
            lblWelcome.AutoSize = false;
            lblWelcome.Size = new Size(900, 40);
            lblWelcome.Location = new Point((this.Width - 900) / 2, 100);
            lblWelcome.TextAlign = ContentAlignment.MiddleCenter;
            lblWelcome.Anchor = AnchorStyles.Top;

            headerPanel.Controls.AddRange(new Control[] { lblTitle, lblWelcome });

            // Cards Container
            FlowLayoutPanel cardsPanel = new FlowLayoutPanel
            {
                Location = new Point(50, 200),
                Size = new Size(1100, 350),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent
            };

            // Tạo các card với hiệu ứng đẹp
            Panel cardBooks = CreateModernCard("📚", "Quản lý Sách", "Thêm, sửa, xóa thông tin sách", 
                Color.FromArgb(52, 152, 219), Color.FromArgb(41, 128, 185), MenuBooks_Click);

            Panel cardMembers = CreateModernCard("👥", "Quản lý Độc giả", "Quản lý thông tin độc giả", 
                Color.FromArgb(46, 204, 113), Color.FromArgb(39, 174, 96), MenuMembers_Click);

            Panel cardLoans = CreateModernCard("📝", "Mượn/Trả Sách", "Xử lý mượn và trả sách", 
                Color.FromArgb(155, 89, 182), Color.FromArgb(142, 68, 173), MenuLoans_Click);

            Panel cardReport = CreateModernCard("📊", "Báo cáo", "Thống kê và báo cáo", 
                Color.FromArgb(230, 126, 34), Color.FromArgb(211, 84, 0), MenuReportBooks_Click);

            cardsPanel.Controls.AddRange(new Control[] { cardBooks, cardMembers, cardLoans, cardReport });

            // Info Panel
            Panel infoPanel = new Panel
            {
                Location = new Point(50, 560),
                Size = new Size(1100, 100),
                BackColor = Color.White,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            infoPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, 15, 15, 180, 90);
                path.AddArc(infoPanel.Width - 15, 0, 15, 15, 270, 90);
                path.AddArc(infoPanel.Width - 15, infoPanel.Height - 15, 15, 15, 0, 90);
                path.AddArc(0, infoPanel.Height - 15, 15, 15, 90, 90);
                path.CloseFigure();
                infoPanel.Region = new Region(path);
            };

            Label lblInfo = new Label();
            lblInfo.Text = "💡 Tính năng: Import/Export XML • Quản lý toàn diện • Báo cáo chi tiết • Giao diện hiện đại";
            lblInfo.Font = new Font("Segoe UI", 11);
            lblInfo.ForeColor = Color.FromArgb(149, 165, 166);
            lblInfo.AutoSize = false;
            lblInfo.Size = new Size(1080, 80);
            lblInfo.Location = new Point(10, 10);
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            infoPanel.Controls.Add(lblInfo);

            mainPanel.Controls.Add(headerPanel);
            mainPanel.Controls.Add(cardsPanel);
            mainPanel.Controls.Add(infoPanel);

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

        private Panel CreateModernCard(string icon, string title, string description, Color color1, Color color2, EventHandler clickHandler = null)
        {
            Panel card = new Panel
            {
                Size = new Size(250, 300),
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                Margin = new Padding(15)
            };

            // Attach click handler to card if provided
            if (clickHandler != null)
            {
                card.Click += clickHandler;
            }

            // Rounded corners và shadow effect
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, 20, 20, 180, 90);
                path.AddArc(card.Width - 20, 0, 20, 20, 270, 90);
                path.AddArc(card.Width - 20, card.Height - 20, 20, 20, 0, 90);
                path.AddArc(0, card.Height - 20, 20, 20, 90, 90);
                path.CloseFigure();
                card.Region = new Region(path);

                // Draw shadow
                using (System.Drawing.Drawing2D.GraphicsPath shadowPath = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    shadowPath.AddArc(3, 3, 20, 20, 180, 90);
                    shadowPath.AddArc(card.Width - 17, 3, 20, 20, 270, 90);
                    shadowPath.AddArc(card.Width - 17, card.Height - 17, 20, 20, 0, 90);
                    shadowPath.AddArc(3, card.Height - 17, 20, 20, 90, 90);
                    shadowPath.CloseFigure();
                }
            };

            // Header với gradient
            Panel header = new Panel
            {
                Size = new Size(250, 120),
                Location = new Point(0, 0),
                BackColor = color1
            };
            header.Paint += (s, e) =>
            {
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    header.ClientRectangle, color1, color2, 45f);
                e.Graphics.FillRectangle(brush, header.ClientRectangle);
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 48),
                Size = new Size(250, 120),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };
            header.Controls.Add(lblIcon);

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Size = new Size(230, 40),
                Location = new Point(10, 135),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(44, 62, 80)
            };

            Label lblDesc = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 10),
                Size = new Size(230, 60),
                Location = new Point(10, 180),
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.FromArgb(149, 165, 166)
            };

            Panel btnPanel = new Panel
            {
                Size = new Size(200, 45),
                Location = new Point(25, 245),
                BackColor = color1
            };
            btnPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, 10, 10, 180, 90);
                path.AddArc(btnPanel.Width - 10, 0, 10, 10, 270, 90);
                path.AddArc(btnPanel.Width - 10, btnPanel.Height - 10, 10, 10, 0, 90);
                path.AddArc(0, btnPanel.Height - 10, 10, 10, 90, 90);
                path.CloseFigure();
                btnPanel.Region = new Region(path);
            };

            Label lblButton = new Label
            {
                Text = "Mở →",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 45),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnPanel.Controls.Add(lblButton);

            card.Controls.AddRange(new Control[] { header, lblTitle, lblDesc, btnPanel });

            // Hover effects
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(245, 245, 245);
                btnPanel.BackColor = color2;
            };
            card.MouseLeave += (s, e) =>
            {
                card.BackColor = Color.White;
                btnPanel.BackColor = color1;
            };

            // Click event propagation - Make child controls trigger card's click event
            if (clickHandler != null)
            {
                foreach (Control ctrl in card.Controls)
                {
                    ctrl.Click += clickHandler;
                    if (ctrl.HasChildren)
                    {
                        foreach (Control child in ctrl.Controls)
                        {
                            child.Click += clickHandler;
                        }
                    }
                }
            }

            return card;
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