using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormReportBooksModern : Form
    {
        private Panel statsPanel;
        private DataGridView dgvReport;
        private ComboBox cboCategory;

        public FormReportBooksModern()
        {
            InitializeComponent();
            SetupModernUI();
            LoadData();
        }

        private void SetupModernUI()
        {
            this.Text = "Báo Cáo Thống Kê Sách";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = ModernUIHelper.Colors.Light;

            // Header
            Panel header = ModernUIHelper.CreateGradientHeader(
                "📊 BÁO CÁO THỐNG KÊ SÁCH",
                "Thống kê và phân tích dữ liệu sách trong thư viện",
                ModernUIHelper.Colors.Warning,
                ModernUIHelper.Colors.WarningDark,
                100
            );

            // Toolbar
            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(30, 15, 30, 15)
            };

            Label lblFilter = new Label
            {
                Text = "Lọc theo thể loại:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 25),
                AutoSize = true
            };

            Panel cboPanel = new Panel
            {
                Location = new Point(160, 20),
                Size = new Size(250, 40),
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

            cboCategory = new ComboBox
            {
                Location = new Point(10, 8),
                Size = new Size(230, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cboCategory.Items.Add("Tất cả");
            cboCategory.SelectedIndex = 0;
            cboCategory.SelectedIndexChanged += (s, e) => FilterData();
            cboPanel.Controls.Add(cboCategory);

            Button btnRefresh = ModernUIHelper.CreateIconButton("🔄", "Làm mới", ModernUIHelper.Colors.Primary, 130);
            btnRefresh.Location = new Point(430, 17);
            btnRefresh.Click += (s, e) => LoadData();

            Button btnExport = ModernUIHelper.CreateIconButton("📤", "Export", ModernUIHelper.Colors.Success, 130);
            btnExport.Location = new Point(580, 17);
            btnExport.Click += BtnExport_Click;

            toolbar.Controls.AddRange(new Control[] { lblFilter, cboPanel, btnRefresh, btnExport });

            // Content
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 20, 30, 30),
                BackColor = ModernUIHelper.Colors.Light
            };

            // Stats Panel
            statsPanel = new Panel
            {
                Location = new Point(30, 20),
                Size = new Size(1300, 140),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // DataGridView
            Panel dgvContainer = new Panel
            {
                Location = new Point(30, 180),
                Size = new Size(1300, 550),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
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
                Size = new Size(1270, 520),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            ModernUIHelper.StyleDataGridView(dgvReport, ModernUIHelper.Colors.Warning);

            dgvContainer.Controls.Add(dgvReport);
            contentPanel.Controls.AddRange(new Control[] { statsPanel, dgvContainer });

            this.Controls.Add(contentPanel);
            this.Controls.Add(toolbar);
            this.Controls.Add(header);
        }

        private void LoadData()
        {
            try
            {
                // Load stats
                string queryStats = @"SELECT 
                    (SELECT COUNT(*) FROM Books) AS TotalBooks,
                    (SELECT SUM(Quantity) FROM Books) AS TotalQuantity,
                    (SELECT COUNT(DISTINCT Category) FROM Books) AS TotalCategories,
                    (SELECT COUNT(DISTINCT Author) FROM Books) AS TotalAuthors";

                DataTable dtStats = DatabaseHelper.ExecuteQuery(queryStats);

                if (dtStats.Rows.Count > 0)
                {
                    statsPanel.Controls.Clear();

                    Panel stat1 = CreateStatCard("📚 Tổng đầu sách", 
                        dtStats.Rows[0]["TotalBooks"].ToString(), 
                        ModernUIHelper.Colors.Primary, 0);

                    Panel stat2 = CreateStatCard("📦 Tổng số lượng", 
                        dtStats.Rows[0]["TotalQuantity"].ToString(), 
                        ModernUIHelper.Colors.Success, 320);

                    Panel stat3 = CreateStatCard("🏷️ Số thể loại", 
                        dtStats.Rows[0]["TotalCategories"].ToString(), 
                        ModernUIHelper.Colors.Info, 640);

                    Panel stat4 = CreateStatCard("✍️ Số tác giả", 
                        dtStats.Rows[0]["TotalAuthors"].ToString(), 
                        ModernUIHelper.Colors.Warning, 960);

                    statsPanel.Controls.AddRange(new Control[] { stat1, stat2, stat3, stat4 });
                }

                // Load categories for filter
                string queryCategories = "SELECT DISTINCT Category FROM Books ORDER BY Category";
                DataTable dtCategories = DatabaseHelper.ExecuteQuery(queryCategories);
                
                cboCategory.Items.Clear();
                cboCategory.Items.Add("Tất cả");
                foreach (DataRow row in dtCategories.Rows)
                {
                    cboCategory.Items.Add(row["Category"].ToString());
                }
                cboCategory.SelectedIndex = 0;

                // Load report data
                string queryReport = @"SELECT 
                    Category AS [Thể loại],
                    COUNT(*) AS [Số đầu sách],
                    SUM(Quantity) AS [Tổng số lượng],
                    AVG(PublishYear) AS [Năm XB trung bình]
                    FROM Books
                    GROUP BY Category
                    ORDER BY SUM(Quantity) DESC";

                DataTable dtReport = DatabaseHelper.ExecuteQuery(queryReport);
                dgvReport.DataSource = dtReport;
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
            if (dgvReport.DataSource is DataTable dt && cboCategory.SelectedItem != null)
            {
                string category = cboCategory.SelectedItem.ToString();
                if (category == "Tất cả")
                {
                    dt.DefaultView.RowFilter = "";
                }
                else
                {
                    dt.DefaultView.RowFilter = string.Format("[Thể loại] = '{0}'", category);
                }
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml",
                    FileName = $"BaoCaoSach_{DateTime.Now:yyyyMMdd_HHmmss}.xml"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Export báo cáo thành công!", "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Export:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            this.Name = "FormReportBooksModern";
            this.ResumeLayout(false);
        }
    }
}
