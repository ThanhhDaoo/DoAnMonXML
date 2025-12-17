using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormReportLoansModern : Form
    {
        private Panel statsPanel;
        private DataGridView dgvReport;
        private DateTimePicker dtpFrom, dtpTo;
        private ComboBox cboStatus;

        public FormReportLoansModern()
        {
            InitializeComponent();
            SetupModernUI();
            LoadData();
        }

        private void SetupModernUI()
        {
            this.Text = "Báo Cáo Thống Kê Mượn/Trả";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = ModernUIHelper.Colors.Light;

            // Header
            Panel header = ModernUIHelper.CreateGradientHeader(
                "📊 BÁO CÁO MƯỢN/TRẢ SÁCH",
                "Thống kê và phân tích hoạt động mượn trả sách",
                ModernUIHelper.Colors.Info,
                ModernUIHelper.Colors.InfoDark,
                100
            );

            // Toolbar
            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(30, 15, 30, 15),
                AutoScroll = true
            };

            Button btnBack = ModernUIHelper.CreateIconButton("◀", "Quay lại", ModernUIHelper.Colors.Gray, 120);
            btnBack.Location = new Point(0, 17);
            btnBack.Click += (s, e) => this.Close();

            Label lblFrom = new Label
            {
                Text = "Từ ngày:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(130, 25),
                AutoSize = true
            };

            Panel dtpFromPanel = new Panel
            {
                Location = new Point(200, 20),
                Size = new Size(160, 40),
                BackColor = ModernUIHelper.Colors.Light
            };
            dtpFromPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(dtpFromPanel.ClientRectangle, 8))
                {
                    dtpFromPanel.Region = new Region(path);
                }
            };

            dtpFrom = new DateTimePicker
            {
                Location = new Point(10, 8),
                Size = new Size(140, 25),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-1)
            };
            dtpFromPanel.Controls.Add(dtpFrom);

            Label lblTo = new Label
            {
                Text = "Đến ngày:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(370, 25),
                AutoSize = true
            };

            Panel dtpToPanel = new Panel
            {
                Location = new Point(450, 20),
                Size = new Size(160, 40),
                BackColor = ModernUIHelper.Colors.Light
            };
            dtpToPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(dtpToPanel.ClientRectangle, 8))
                {
                    dtpToPanel.Region = new Region(path);
                }
            };

            dtpTo = new DateTimePicker
            {
                Location = new Point(10, 8),
                Size = new Size(140, 25),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            dtpToPanel.Controls.Add(dtpTo);

            Label lblStatus = new Label
            {
                Text = "Trạng thái:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(620, 25),
                AutoSize = true
            };

            Panel cboPanel = new Panel
            {
                Location = new Point(710, 20),
                Size = new Size(140, 40),
                BackColor = ModernUIHelper.Colors.Light
            };
            cboPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(cboPanel.ClientRectangle, 8))
                {
                    cboPanel.Region = new Region(path);
                }
            };

            cboStatus = new ComboBox
            {
                Location = new Point(10, 8),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cboStatus.Items.AddRange(new string[] { "Tất cả", "Borrowed", "Returned", "Overdue" });
            cboStatus.SelectedIndex = 0;
            cboStatus.SelectedIndexChanged += (s, e) => FilterData();
            cboPanel.Controls.Add(cboStatus);

            Button btnFilter = ModernUIHelper.CreateIconButton("🔍", "Lọc", ModernUIHelper.Colors.Primary, 110);
            btnFilter.Location = new Point(860, 17);
            btnFilter.Click += (s, e) => LoadData();

            Button btnRefresh = ModernUIHelper.CreateIconButton("🔄", "Làm mới", ModernUIHelper.Colors.Success, 120);
            btnRefresh.Location = new Point(980, 17);
            btnRefresh.Click += (s, e) => { dtpFrom.Value = DateTime.Now.AddMonths(-1); dtpTo.Value = DateTime.Now; LoadData(); };

            toolbar.Controls.AddRange(new Control[] { 
                btnBack, lblFrom, dtpFromPanel, lblTo, dtpToPanel, 
                lblStatus, cboPanel, btnFilter, btnRefresh 
            });

            // Content
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 20, 30, 30),
                BackColor = ModernUIHelper.Colors.Light,
                AutoScroll = true
            };

            // Stats Panel - RESPONSIVE
            statsPanel = new Panel
            {
                Location = new Point(30, 20),
                Size = new Size(1200, 140),
                Height = 140,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // DataGridView - RESPONSIVE WIDTH, FIXED HEIGHT for scrolling
            Panel dgvContainer = new Panel
            {
                Location = new Point(30, 180),
                Size = new Size(1200, 500),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            
            dgvContainer.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(dgvContainer.ClientRectangle, 15))
                {
                    dgvContainer.Region = new Region(path);
                }
            };

            dgvReport = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(1170, 470),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            
            ModernUIHelper.StyleDataGridView(dgvReport, ModernUIHelper.Colors.Info);

            dgvContainer.Controls.Add(dgvReport);
            
            // Event resize
            contentPanel.Resize += (s, e) =>
            {
                statsPanel.Width = contentPanel.Width - 60;
                dgvContainer.Width = contentPanel.Width - 60;
            };
            contentPanel.Controls.AddRange(new Control[] { statsPanel, dgvContainer });

            this.Controls.Add(contentPanel);
            this.Controls.Add(toolbar);
            this.Controls.Add(header);
        }

        private void LoadData()
        {
            try
            {
                string dateFrom = dtpFrom.Value.ToString("yyyy-MM-dd");
                string dateTo = dtpTo.Value.ToString("yyyy-MM-dd");

                // Load stats
                string queryStats = string.Format(@"SELECT 
                    (SELECT COUNT(*) FROM Loans WHERE LoanDate BETWEEN '{0}' AND '{1}') AS TotalLoans,
                    (SELECT COUNT(*) FROM Loans WHERE Status = 'Borrowed') AS Borrowed,
                    (SELECT COUNT(*) FROM Loans WHERE Status = 'Returned' AND ReturnDate BETWEEN '{0}' AND '{1}') AS Returned,
                    (SELECT COUNT(*) FROM Loans WHERE Status = 'Overdue') AS Overdue", 
                    dateFrom, dateTo);

                DataTable dtStats = DatabaseHelper.ExecuteQuery(queryStats);

                if (dtStats.Rows.Count > 0)
                {
                    statsPanel.Controls.Clear();

                    // RESPONSIVE STAT CARDS
                    int cardWidth = (statsPanel.Width - 60) / 4;
                    int cardSpacing = 20;

                    Panel stat1 = CreateStatCard("📚 Tổng phiếu mượn", 
                        dtStats.Rows[0]["TotalLoans"].ToString(), 
                        ModernUIHelper.Colors.Primary, 0);
                    stat1.Width = cardWidth;

                    Panel stat2 = CreateStatCard("📖 Đang mượn", 
                        dtStats.Rows[0]["Borrowed"].ToString(), 
                        ModernUIHelper.Colors.Success, cardWidth + cardSpacing);
                    stat2.Width = cardWidth;

                    Panel stat3 = CreateStatCard("✅ Đã trả", 
                        dtStats.Rows[0]["Returned"].ToString(), 
                        ModernUIHelper.Colors.Info, (cardWidth + cardSpacing) * 2);
                    stat3.Width = cardWidth;

                    Panel stat4 = CreateStatCard("⚠️ Quá hạn", 
                        dtStats.Rows[0]["Overdue"].ToString(), 
                        ModernUIHelper.Colors.Danger, (cardWidth + cardSpacing) * 3);
                    stat4.Width = cardWidth;

                    statsPanel.Controls.AddRange(new Control[] { stat1, stat2, stat3, stat4 });
                }

                // Load report data
                string queryReport = string.Format(@"SELECT 
                    L.LoanID AS [ID],
                    B.Title AS [Tên sách],
                    M.FullName AS [Độc giả],
                    L.LoanDate AS [Ngày mượn],
                    L.DueDate AS [Hạn trả],
                    L.ReturnDate AS [Ngày trả],
                    L.Status AS [Trạng thái],
                    CASE 
                        WHEN L.Status = 'Overdue' OR (L.DueDate < GETDATE() AND L.Status = 'Borrowed') 
                        THEN DATEDIFF(day, L.DueDate, GETDATE())
                        ELSE 0
                    END AS [Số ngày quá hạn]
                    FROM Loans L
                    INNER JOIN Books B ON L.BookID = B.BookID
                    INNER JOIN Members M ON L.MemberID = M.MemberID
                    WHERE L.LoanDate BETWEEN '{0}' AND '{1}'
                    ORDER BY L.LoanID DESC", dateFrom, dateTo);

                DataTable dtReport = DatabaseHelper.ExecuteQuery(queryReport);
                dgvReport.DataSource = dtReport;

                // Highlight overdue rows
                foreach (DataGridViewRow row in dgvReport.Rows)
                {
                    if (row.Cells["Số ngày quá hạn"].Value != null)
                    {
                        int days = Convert.ToInt32(row.Cells["Số ngày quá hạn"].Value);
                        if (days > 30)
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                        else if (days > 14)
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 200);
                        else if (days > 0)
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateStatCard(string title, string value, Color color, int x)
        {
            Panel card = new Panel
            {
                Size = new Size(300, 120),
                Location = new Point(x, 10),
                BackColor = color
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

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 50),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            card.Controls.AddRange(new Control[] { lblTitle, lblValue });
            return card;
        }

        private void FilterData()
        {
            if (dgvReport.DataSource is DataTable dt && cboStatus.SelectedItem != null)
            {
                string status = cboStatus.SelectedItem.ToString();
                if (status == "Tất cả")
                {
                    dt.DefaultView.RowFilter = "";
                }
                else
                {
                    dt.DefaultView.RowFilter = string.Format("[Trạng thái] = '{0}'", status);
                }
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
            this.Name = "FormReportLoansModern";
            this.ResumeLayout(false);
        }
    }
}
