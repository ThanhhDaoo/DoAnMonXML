using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormMainModern : Form
    {
        private string userName;
        private string userRole;
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Label lblWelcome;

        public FormMainModern(string userName, string userRole)
        {
            InitializeComponent();
            this.userName = userName;
            this.userRole = userRole;
            SetupModernUI();
        }

        private void SetupModernUI()
        {
            // Form configuration
            this.Text = "Hệ Thống Quản Lý Thư Viện";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = ModernUIHelper.Colors.Light;

            // === SIDEBAR ===
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = ModernUIHelper.Colors.Dark
            };

            // Logo section
            Panel logoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = ModernUIHelper.Colors.Dark
            };

            Label lblLogo = new Label
            {
                Text = "📚",
                Font = new Font("Segoe UI", 42),
                ForeColor = Color.White,
                Size = new Size(80, 80),
                Location = new Point(100, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblAppName = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(280, 30),
                Location = new Point(0, 85),
                TextAlign = ContentAlignment.MiddleCenter
            };

            logoPanel.Controls.AddRange(new Control[] { lblLogo, lblAppName });

            // User info section
            Panel userPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            Panel avatarCircle = ModernUIHelper.CreateCircularIcon("👤", ModernUIHelper.Colors.Primary, 60);
            avatarCircle.Location = new Point(15, 20);

            Label lblUserName = new Label
            {
                Text = userName,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(85, 25),
                Size = new Size(180, 25)
            };

            Label lblUserRole = new Label
            {
                Text = userRole,
                Font = new Font("Segoe UI", 9),
                ForeColor = ModernUIHelper.Colors.Gray,
                Location = new Point(85, 50),
                Size = new Size(180, 20)
            };

            userPanel.Controls.AddRange(new Control[] { avatarCircle, lblUserName, lblUserRole });

            // Menu items
            FlowLayoutPanel menuPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10, 20, 10, 10),
                BackColor = ModernUIHelper.Colors.Dark,
                AutoScroll = true
            };

            menuPanel.Controls.Add(CreateMenuButton("🏠", "Trang chủ", true));
            menuPanel.Controls.Add(CreateMenuButton("📚", "Quản lý Sách", false, (s, e) => OpenBooks()));
            menuPanel.Controls.Add(CreateMenuButton("👥", "Quản lý Độc giả", false, (s, e) => OpenMembers()));
            menuPanel.Controls.Add(CreateMenuButton("📝", "Mượn/Trả Sách", false, (s, e) => OpenLoans()));
            menuPanel.Controls.Add(CreateMenuButton("📊", "Báo cáo Sách", false, (s, e) => OpenReports()));
            menuPanel.Controls.Add(CreateMenuButton("📈", "Báo cáo Mượn/Trả", false, (s, e) => OpenReportLoans()));
            menuPanel.Controls.Add(CreateMenuButton("⚙️", "Cài đặt", false));
            menuPanel.Controls.Add(CreateMenuButton("❓", "Trợ giúp", false, (s, e) => ShowHelp()));
            menuPanel.Controls.Add(CreateMenuButton("🚪", "Đăng xuất", false, (s, e) => Logout()));

            sidebarPanel.Controls.Add(menuPanel);
            sidebarPanel.Controls.Add(userPanel);
            sidebarPanel.Controls.Add(logoPanel);

            // === CONTENT AREA ===
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ModernUIHelper.Colors.Light,
                Padding = new Padding(30),
                AutoScroll = true
            };

            // Header Panel
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.Transparent
            };

            lblWelcome = new Label
            {
                Text = $"Xin chào, {userName}! 👋",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(0, 10),
                AutoSize = true
            };

            Label lblDate = new Label
            {
                Text = DateTime.Now.ToString("dddd, dd MMMM yyyy"),
                Font = new Font("Segoe UI", 12),
                ForeColor = ModernUIHelper.Colors.Gray,
                Location = new Point(0, 55),
                AutoSize = true
            };

            headerPanel.Controls.AddRange(new Control[] { lblWelcome, lblDate });

            // Stats Panel (Top)
            Panel statsPanel = new Panel
            {
                Location = new Point(0, 120),
                Size = new Size(1300, 140),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // 3 PANELS - MỖI PANEL 400px VỚI KHOẢNG CÁCH 30px
            int panelWidth = 400;
            int panelHeight = 700;
            int spacing = 30;
            int startY = 280;

            // Quick Actions Panel (Middle Left)
            Panel quickActionsPanel = CreateQuickActionsPanel();
            quickActionsPanel.Location = new Point(0, startY);
            quickActionsPanel.Size = new Size(panelWidth, panelHeight);
            quickActionsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            // Recent Activities Panel (Middle Center)
            Panel recentPanel = CreateRecentActivitiesPanel();
            recentPanel.Location = new Point(panelWidth + spacing, startY);
            recentPanel.Size = new Size(panelWidth, panelHeight);
            recentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            // Statistics Panel (Middle Right)
            Panel statsChartPanel = CreateStatisticsPanel();
            statsChartPanel.Location = new Point((panelWidth + spacing) * 2, startY);
            statsChartPanel.Size = new Size(panelWidth, panelHeight);
            statsChartPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            // Thêm controls theo đúng thứ tự
            contentPanel.Controls.Add(headerPanel);
            contentPanel.Controls.Add(statsPanel);
            contentPanel.Controls.Add(quickActionsPanel);
            contentPanel.Controls.Add(recentPanel);
            contentPanel.Controls.Add(statsChartPanel);

            // Load dashboard data
            LoadDashboardData(statsPanel);

            // Add to form
            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebarPanel);
        }

        private Button CreateMenuButton(string icon, string text, bool isActive, EventHandler clickHandler = null)
        {
            Button btn = new Button
            {
                Text = $"  {icon}   {text}",
                Size = new Size(260, 50),
                BackColor = isActive ? Color.FromArgb(52, 152, 219) : Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 5, 0, 5)
            };

            btn.FlatAppearance.BorderSize = 0;

            btn.MouseEnter += (s, e) =>
            {
                if (!isActive)
                    btn.BackColor = Color.FromArgb(52, 73, 94);
            };

            btn.MouseLeave += (s, e) =>
            {
                if (!isActive)
                    btn.BackColor = Color.Transparent;
            };

            if (clickHandler != null)
                btn.Click += clickHandler;

            return btn;
        }

        private void LoadDashboardData(Panel statsPanel)
        {
            try
            {
                string query = @"SELECT 
                    (SELECT COUNT(*) FROM Books) AS TotalBooks,
                    (SELECT SUM(Quantity) FROM Books) AS TotalQuantity,
                    (SELECT COUNT(*) FROM Members WHERE Status = 'Active') AS ActiveMembers,
                    (SELECT COUNT(*) FROM Loans WHERE Status = 'Borrowed') AS CurrentLoans,
                    (SELECT COUNT(*) FROM Loans WHERE Status = 'Overdue') AS OverdueLoans";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                if (dt.Rows.Count > 0)
                {
                    statsPanel.Controls.Clear();

                    // MỖI CARD RỘNG 310px VỚI KHOẢNG CÁCH 20px
                    int cardWidth = 310;
                    int spacing = 20;

                    Panel stat1 = CreateStatCard("📚", "Tổng sách",
                        dt.Rows[0]["TotalBooks"].ToString(),
                        dt.Rows[0]["TotalQuantity"].ToString() + " cuốn",
                        ModernUIHelper.Colors.Primary, 0);
                    stat1.Size = new Size(cardWidth, 120);

                    Panel stat2 = CreateStatCard("👥", "Độc giả",
                        dt.Rows[0]["ActiveMembers"].ToString(),
                        "Đang hoạt động",
                        ModernUIHelper.Colors.Success, cardWidth + spacing);
                    stat2.Size = new Size(cardWidth, 120);

                    Panel stat3 = CreateStatCard("📖", "Đang mượn",
                        dt.Rows[0]["CurrentLoans"].ToString(),
                        "Phiếu mượn",
                        ModernUIHelper.Colors.Info, (cardWidth + spacing) * 2);
                    stat3.Size = new Size(cardWidth, 120);

                    Panel stat4 = CreateStatCard("⚠️", "Quá hạn",
                        dt.Rows[0]["OverdueLoans"].ToString(),
                        "Cần xử lý",
                        ModernUIHelper.Colors.Danger, (cardWidth + spacing) * 3);
                    stat4.Size = new Size(cardWidth, 120);

                    statsPanel.Controls.AddRange(new Control[] { stat1, stat2, stat3, stat4 });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu dashboard:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateStatCard(string icon, string title, string value, string subtitle, Color color, int x)
        {
            Panel card = new Panel
            {
                Size = new Size(240, 120),
                Location = new Point(x, 10),
                BackColor = color,
                Cursor = Cursors.Hand
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(card.ClientRectangle, 15))
                {
                    card.Region = new Region(path);
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        card.ClientRectangle, color, ControlPaint.Dark(color, 0.1f), 45f))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 32),
                ForeColor = Color.White,
                Location = new Point(15, 15),
                Size = new Size(60, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(240, 240, 240),
                Location = new Point(85, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(85, 40),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblSubtitle = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(230, 230, 230),
                Location = new Point(85, 85),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            card.Controls.AddRange(new Control[] { lblIcon, lblTitle, lblValue, lblSubtitle });

            card.MouseEnter += (s, e) => card.BackColor = ControlPaint.Dark(color, 0.1f);
            card.MouseLeave += (s, e) => card.BackColor = color;

            return card;
        }

        private Panel CreateQuickActionsPanel()
        {
            Panel panel = new Panel
            {
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(panel.ClientRectangle, 15))
                {
                    panel.Region = new Region(path);
                }
            };

            Label lblTitle = new Label
            {
                Text = "⚡ THAO TÁC NHANH",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Buttons rộng hơn: 360px
            Button btnBooks = CreateQuickActionButton("📚", "Quản lý Sách",
                ModernUIHelper.Colors.Primary, 20, 70);
            btnBooks.Size = new Size(360, 90);
            btnBooks.Click += (s, e) => OpenBooks();

            Button btnMembers = CreateQuickActionButton("👥", "Quản lý Độc giả",
                ModernUIHelper.Colors.Success, 20, 175);
            btnMembers.Size = new Size(360, 90);
            btnMembers.Click += (s, e) => OpenMembers();

            Button btnLoans = CreateQuickActionButton("📝", "Mượn/Trả Sách",
                ModernUIHelper.Colors.Info, 20, 280);
            btnLoans.Size = new Size(360, 90);
            btnLoans.Click += (s, e) => OpenLoans();

            panel.Controls.AddRange(new Control[] { lblTitle, btnBooks, btnMembers, btnLoans });

            return panel;
        }

        private Button CreateQuickActionButton(string icon, string text, Color color, int x, int y)
        {
            Button btn = new Button
            {
                Text = $"{icon}\n\n{text}",
                Size = new Size(360, 90),
                Location = new Point(x, y),
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };

            btn.FlatAppearance.BorderSize = 0;

            btn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(btn.ClientRectangle, 10))
                {
                    btn.Region = new Region(path);
                }
            };

            Color originalColor = color;
            Color hoverColor = ControlPaint.Dark(color, 0.15f);

            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = originalColor;

            return btn;
        }

        private Panel CreateStatisticsPanel()
        {
            Panel panel = new Panel
            {
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(panel.ClientRectangle, 15))
                {
                    panel.Region = new Region(path);
                }
            };

            Label lblTitle = new Label
            {
                Text = "📊 THỐNG KÊ CHI TIẾT",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, 20),
                AutoSize = true
            };

            panel.Controls.Add(lblTitle);

            // Load statistics
            LoadDetailedStats(panel);

            return panel;
        }

        private void LoadDetailedStats(Panel panel)
        {
            try
            {
                string query = @"SELECT 
                    (SELECT COUNT(DISTINCT Category) FROM Books) AS Categories,
                    (SELECT COUNT(DISTINCT Author) FROM Books) AS Authors,
                    (SELECT COUNT(*) FROM Loans WHERE MONTH(LoanDate) = MONTH(GETDATE())) AS ThisMonth,
                    (SELECT COUNT(*) FROM Loans WHERE YEAR(LoanDate) = YEAR(GETDATE())) AS ThisYear,
                    (SELECT COUNT(*) FROM Members WHERE MONTH(JoinDate) = MONTH(GETDATE())) AS NewMembers,
                    (SELECT TOP 1 Category FROM Books GROUP BY Category ORDER BY COUNT(*) DESC) AS TopCategory";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                if (dt.Rows.Count > 0)
                {
                    int yPos = 70;
                    int itemHeight = 80;  // Height của mỗi stat item
                    int spacing = 12;

                    // Categories
                    Panel cat = CreateStatItem("📚", "Thể loại sách",
                        dt.Rows[0]["Categories"].ToString(),
                        ModernUIHelper.Colors.Primary, yPos);
                    panel.Controls.Add(cat);

                    // Authors
                    yPos += itemHeight + spacing; // 80 + 12 = 92

                    Panel auth = CreateStatItem("✍️", "Tác giả",
                        dt.Rows[0]["Authors"].ToString(),
                        ModernUIHelper.Colors.Success, yPos);
                    panel.Controls.Add(auth);

                    // This month loans
                    yPos += itemHeight + spacing;

                    Panel month = CreateStatItem("📅", "Mượn tháng này",
                        dt.Rows[0]["ThisMonth"].ToString(),
                        ModernUIHelper.Colors.Info, yPos);
                    panel.Controls.Add(month);

                    // This year loans
                    yPos += itemHeight + spacing;

                    Panel year = CreateStatItem("📆", "Mượn năm nay",
                        dt.Rows[0]["ThisYear"].ToString(),
                        ModernUIHelper.Colors.Warning, yPos);
                    panel.Controls.Add(year);

                    // New members
                                        yPos += itemHeight + spacing;

                    Panel newMem = CreateStatItem("👤", "ĐG mới tháng này",
                        dt.Rows[0]["NewMembers"].ToString(),
                        ModernUIHelper.Colors.Success, yPos);
                    panel.Controls.Add(newMem);
                }
            }
            catch { }
        }

        private Panel CreateStatItem(string icon, string label, string value, Color color, int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(20, y),
                Size = new Size(360, 80),
                BackColor = Color.FromArgb(248, 249, 250)
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
                Font = new Font("Segoe UI", 24),
                ForeColor = color,
                Location = new Point(15, 15),
                Size = new Size(50, 50),
                BackColor = Color.Transparent
            };

            Label lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9),
                ForeColor = ModernUIHelper.Colors.Gray,
                Location = new Point(75, 12),
                Size = new Size(200, 20),
                BackColor = Color.Transparent
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(75, 28),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            panel.Controls.AddRange(new Control[] { lblIcon, lblLabel, lblValue });

            panel.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(240, 240, 240);
            panel.MouseLeave += (s, e) => panel.BackColor = Color.FromArgb(248, 249, 250);

            return panel;
        }

        private Panel CreateRecentActivitiesPanel()
        {
            Panel panel = new Panel
            {
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(panel.ClientRectangle, 15))
                {
                    panel.Region = new Region(path);
                }
            };

            Label lblTitle = new Label
            {
                Text = "🕐 HOẠT ĐỘNG GẦN ĐÂY",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, 20),
                AutoSize = true
            };

            panel.Controls.Add(lblTitle);

            // Load recent activities
            LoadRecentActivities(panel);

            return panel;
        }

        private void LoadRecentActivities(Panel panel)
        {
            try
            {
                string query = @"SELECT TOP 7
                    L.LoanID,
                    B.Title AS BookTitle,
                    M.FullName AS MemberName,
                    L.LoanDate,
                    L.Status
                FROM Loans L
                INNER JOIN Books B ON L.BookID = B.BookID
                INNER JOIN Members M ON L.MemberID = M.MemberID
                ORDER BY L.LoanDate DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                int yPos = 70;
                foreach (DataRow row in dt.Rows)
                {
                    Panel activity = CreateActivityItem(
                        row["BookTitle"].ToString(),
                        row["MemberName"].ToString(),
                        Convert.ToDateTime(row["LoanDate"]),
                        row["Status"].ToString(),
                        yPos
                    );
                    panel.Controls.Add(activity);
                    yPos += 42 + 8;                }
            }
            catch { }
        }

        private Panel CreateActivityItem(string bookTitle, string memberName, DateTime date, string status, int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(20, y),
                Size = new Size(360, 42),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(panel.ClientRectangle, 8))
                {
                    panel.Region = new Region(path);
                }
            };

            string icon = status == "Borrowed" ? "📖" : status == "Returned" ? "✅" : "⚠️";

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 16),
                Location = new Point(10, 8),
                Size = new Size(30, 25),
                BackColor = Color.Transparent
            };

            Label lblBook = new Label
            {
                Text = bookTitle.Length > 22 ? bookTitle.Substring(0, 22) + "..." : bookTitle,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(45, 5),
                Size = new Size(250, 15),
                BackColor = Color.Transparent
            };

            Label lblMember = new Label
            {
                Text = memberName.Length > 24 ? memberName.Substring(0, 24) + "..." : memberName,
                Font = new Font("Segoe UI", 8),
                ForeColor = ModernUIHelper.Colors.Gray,
                Location = new Point(45, 22),
                Size = new Size(180, 15),
                BackColor = Color.Transparent
            };

            Label lblDate = new Label
            {
                Text = date.ToString("dd/MM"),
                Font = new Font("Segoe UI", 8),
                ForeColor = ModernUIHelper.Colors.Gray,
                Location = new Point(290, 12),
                Size = new Size(60, 15),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };

            panel.Controls.AddRange(new Control[] { lblIcon, lblBook, lblMember, lblDate });

            panel.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(240, 240, 240);
            panel.MouseLeave += (s, e) => panel.BackColor = Color.FromArgb(248, 249, 250);

            return panel;
        }

        private void OpenBooks()
        {
            FormBooksModern form = new FormBooksModern();
            form.ShowDialog();
        }

        private void OpenMembers()
        {
            FormMembersModern form = new FormMembersModern();
            form.ShowDialog();
        }

        private void OpenLoans()
        {
            FormLoansModern form = new FormLoansModern();
            form.ShowDialog();
        }

        private void OpenReports()
        {
            FormReportBooksModern form = new FormReportBooksModern();
            form.ShowDialog();
        }

        private void OpenReportLoans()
        {
            FormReportLoansModern form = new FormReportLoansModern();
            form.ShowDialog();
        }

        private void ShowHelp()
        {
            MessageBox.Show("Hướng dẫn sử dụng hệ thống...", "Trợ giúp",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Logout()
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
                FormLoginModern loginForm = new FormLoginModern();
                loginForm.Show();
            }
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
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.Name = "FormMainModern";
            this.Load += new System.EventHandler(this.FormMainModern_Load);
            this.ResumeLayout(false);
        }

        private void FormMainModern_Load(object sender, EventArgs e)
        {
        }
    }
}