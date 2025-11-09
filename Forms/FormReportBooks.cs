using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormReportBooks : Form
    {
        private TabControl tabControl;
        private Panel pnlStats;

        public FormReportBooks()
        {
            InitializeComponent();
            SetupUI();
            LoadAllReports();
        }

        private void SetupUI()
        {
            // Cấu hình Form
            this.Text = "Báo Cáo Thống Kê Sách";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Header Panel
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            headerPanel.BackColor = Color.FromArgb(41, 128, 185);

            Label lblTitle = new Label();
            lblTitle.Text = "📊 BÁO CÁO THỐNG KÊ SÁCH";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;

            Label lblDate = new Label();
            lblDate.Text = "Ngày: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            lblDate.Font = new Font("Segoe UI", 10);
            lblDate.ForeColor = Color.White;
            lblDate.Location = new Point(20, 50);
            lblDate.AutoSize = true;

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblDate);

            // Button Panel
            Panel btnPanel = new Panel();
            btnPanel.Dock = DockStyle.Top;
            btnPanel.Height = 50;
            btnPanel.BackColor = Color.White;

            Button btnRefresh = new Button();
            btnRefresh.Text = "🔄 Làm mới";
            btnRefresh.Location = new Point(20, 10);
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.BackColor = Color.FromArgb(52, 152, 219);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Click += (s, e) => LoadAllReports();

            Button btnClose = new Button();
            btnClose.Text = "❌ Đóng";
            btnClose.Location = new Point(130, 10);
            btnClose.Size = new Size(100, 30);
            btnClose.BackColor = Color.FromArgb(231, 76, 60);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Click += (s, e) => this.Close();

            btnPanel.Controls.Add(btnRefresh);
            btnPanel.Controls.Add(btnClose);

            // Tab Control
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10);

            // Tab 1: Tổng quan
            TabPage tab1 = new TabPage("Tổng quan");
            SetupTab1(tab1);

            // Tab 2: Theo thể loại
            TabPage tab2 = new TabPage("Theo thể loại");
            SetupTab2(tab2);

            // Tab 3: Sách phổ biến
            TabPage tab3 = new TabPage("Sách phổ biến");
            SetupTab3(tab3);

            // Tab 4: Chi tiết
            TabPage tab4 = new TabPage("Chi tiết");
            SetupTab4(tab4);

            tabControl.TabPages.Add(tab1);
            tabControl.TabPages.Add(tab2);
            tabControl.TabPages.Add(tab3);
            tabControl.TabPages.Add(tab4);

            // Add controls
            this.Controls.Add(tabControl);
            this.Controls.Add(btnPanel);
            this.Controls.Add(headerPanel);
        }

        // TAB 1: TỔNG QUAN
        private void SetupTab1(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 244, 248);

            // Panel thống kê
            pnlStats = new Panel();
            pnlStats.Location = new Point(20, 20);
            pnlStats.Size = new Size(1040, 120);
            pnlStats.Name = "pnlStats";

            // DataGridView
            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvTab1";
            dgv.Location = new Point(20, 160);
            dgv.Size = new Size(1040, 400);
            dgv.BackgroundColor = Color.White;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tab.Controls.Add(pnlStats);
            tab.Controls.Add(dgv);
        }

        // TAB 2: THEO THỂ LOẠI
        private void SetupTab2(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 244, 248);

            Label lbl = new Label();
            lbl.Text = "📚 THỐNG KÊ THEO THỂ LOẠI";
            lbl.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(41, 128, 185);
            lbl.Location = new Point(20, 20);
            lbl.AutoSize = true;

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvTab2";
            dgv.Location = new Point(20, 60);
            dgv.Size = new Size(1040, 500);
            dgv.BackgroundColor = Color.White;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tab.Controls.Add(lbl);
            tab.Controls.Add(dgv);
        }

        // TAB 3: SÁCH PHỔ BIẾN
        private void SetupTab3(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 244, 248);

            Panel pnl = new Panel();
            pnl.Location = new Point(20, 20);
            pnl.Size = new Size(1040, 60);
            pnl.BackColor = Color.FromArgb(231, 76, 60);

            Label lbl = new Label();
            lbl.Text = "🔥 TOP 10 SÁCH ĐƯỢC MƯỢN NHIỀU NHẤT";
            lbl.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(15, 20);
            lbl.AutoSize = true;

            pnl.Controls.Add(lbl);

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvTab3";
            dgv.Location = new Point(20, 100);
            dgv.Size = new Size(1040, 460);
            dgv.BackgroundColor = Color.White;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tab.Controls.Add(pnl);
            tab.Controls.Add(dgv);
        }

        // TAB 4: CHI TIẾT
        private void SetupTab4(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 244, 248);

            Panel filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 60;
            filterPanel.BackColor = Color.White;

            Label lbl = new Label();
            lbl.Text = "Thể loại:";
            lbl.Location = new Point(20, 20);
            lbl.Size = new Size(80, 25);

            ComboBox cbo = new ComboBox();
            cbo.Name = "cboFilter";
            cbo.Location = new Point(100, 18);
            cbo.Size = new Size(200, 25);
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbo.SelectedIndexChanged += CboFilter_Changed;

            Button btn = new Button();
            btn.Text = "Tất cả";
            btn.Location = new Point(320, 15);
            btn.Size = new Size(100, 30);
            btn.BackColor = Color.FromArgb(52, 152, 219);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Click += (s, e) => LoadTab4();

            filterPanel.Controls.Add(lbl);
            filterPanel.Controls.Add(cbo);
            filterPanel.Controls.Add(btn);

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvTab4";
            dgv.Dock = DockStyle.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tab.Controls.Add(dgv);
            tab.Controls.Add(filterPanel);
        }

        // LOAD DỮ LIỆU
        private void LoadAllReports()
        {
            LoadTab1();
            LoadTab2();
            LoadTab3();
            LoadTab4();
        }

        private void LoadTab1()
        {
            try
            {
                // Thống kê
                string query = @"SELECT 
                    (SELECT COUNT(*) FROM Books) AS Total,
                    (SELECT SUM(Quantity) FROM Books) AS Quantity,
                    (SELECT COUNT(DISTINCT Category) FROM Books) AS Categories";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                if (dt.Rows.Count > 0)
                {
                    pnlStats.Controls.Clear();

                    Panel p1 = CreateStatBox("Tổng đầu sách", dt.Rows[0]["Total"].ToString(), 0);
                    Panel p2 = CreateStatBox("Tổng số lượng", dt.Rows[0]["Quantity"].ToString(), 260);
                    Panel p3 = CreateStatBox("Số thể loại", dt.Rows[0]["Categories"].ToString(), 520);

                    pnlStats.Controls.Add(p1);
                    pnlStats.Controls.Add(p2);
                    pnlStats.Controls.Add(p3);
                }

                // Chi tiết
                string query2 = @"SELECT Category AS [Thể loại], COUNT(*) AS [Số sách], SUM(Quantity) AS [Tổng SL]
                    FROM Books GROUP BY Category ORDER BY COUNT(*) DESC";

                DataTable dt2 = DatabaseHelper.ExecuteQuery(query2);
                DataGridView dgv = GetDGV(0, "dgvTab1");
                dgv.DataSource = dt2;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void LoadTab2()
        {
            string query = @"SELECT Category AS [Thể loại], COUNT(*) AS [Số đầu sách], 
                SUM(Quantity) AS [Tổng số lượng], AVG(PublishYear) AS [Năm XB TB]
                FROM Books GROUP BY Category ORDER BY SUM(Quantity) DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            DataGridView dgv = GetDGV(1, "dgvTab2");
            dgv.DataSource = dt;
        }

        private void LoadTab3()
        {
            string query = @"SELECT TOP 10 B.Title AS [Tên sách], B.Author AS [Tác giả], 
                B.Category AS [Thể loại], COUNT(L.LoanID) AS [Số lần mượn]
                FROM Books B LEFT JOIN Loans L ON B.BookID = L.BookID
                GROUP BY B.BookID, B.Title, B.Author, B.Category
                ORDER BY COUNT(L.LoanID) DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            DataGridView dgv = GetDGV(2, "dgvTab3");
            dgv.DataSource = dt;

            // Tô màu top 3
            if (dgv.Rows.Count > 0)
                dgv.Rows[0].DefaultCellStyle.BackColor = Color.Gold;
            if (dgv.Rows.Count > 1)
                dgv.Rows[1].DefaultCellStyle.BackColor = Color.Silver;
            if (dgv.Rows.Count > 2)
                dgv.Rows[2].DefaultCellStyle.BackColor = Color.FromArgb(205, 127, 50);
        }

        private void LoadTab4()
        {
            string query = @"SELECT BookID AS [ID], Title AS [Tên sách], Author AS [Tác giả], 
                Publisher AS [NXB], PublishYear AS [Năm XB], Category AS [Thể loại], 
                Quantity AS [SL], ISBN FROM Books ORDER BY BookID";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            DataGridView dgv = GetDGV(3, "dgvTab4");
            dgv.DataSource = dt;

            // Load combo
            ComboBox cbo = GetComboBox(3, "cboFilter");
            if (cbo != null && cbo.Items.Count == 0)
            {
                string qCat = "SELECT DISTINCT Category FROM Books ORDER BY Category";
                DataTable dtCat = DatabaseHelper.ExecuteQuery(qCat);
                foreach (DataRow row in dtCat.Rows)
                {
                    cbo.Items.Add(row["Category"].ToString());
                }
            }
        }

        private void CboFilter_Changed(object sender, EventArgs e)
        {
            ComboBox cbo = sender as ComboBox;
            DataGridView dgv = GetDGV(3, "dgvTab4");

            if (cbo != null && dgv != null && cbo.SelectedItem != null)
            {
                if (dgv.DataSource is DataTable dt)
                {
                    dt.DefaultView.RowFilter = string.Format("[Thể loại] = '{0}'", cbo.SelectedItem);
                }
            }
        }

        // Helper methods
        private Panel CreateStatBox(string title, string value, int x)
        {
            Panel p = new Panel();
            p.Size = new Size(240, 100);
            p.Location = new Point(x, 10);
            p.BackColor = Color.FromArgb(52, 152, 219);

            Label lbl1 = new Label();
            lbl1.Text = title;
            lbl1.ForeColor = Color.White;
            lbl1.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl1.Location = new Point(10, 10);
            lbl1.AutoSize = true;

            Label lbl2 = new Label();
            lbl2.Text = value;
            lbl2.ForeColor = Color.White;
            lbl2.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lbl2.Location = new Point(10, 40);
            lbl2.AutoSize = true;

            p.Controls.Add(lbl1);
            p.Controls.Add(lbl2);
            return p;
        }

        private DataGridView GetDGV(int tabIndex, string name)
        {
            foreach (Control ctrl in tabControl.TabPages[tabIndex].Controls)
            {
                if (ctrl is DataGridView && ctrl.Name == name)
                    return (DataGridView)ctrl;
            }
            return null;
        }

        private ComboBox GetComboBox(int tabIndex, string name)
        {
            foreach (Control ctrl in tabControl.TabPages[tabIndex].Controls)
            {
                if (ctrl is Panel)
                {
                    foreach (Control c in ctrl.Controls)
                    {
                        if (c is ComboBox && c.Name == name)
                            return (ComboBox)c;
                    }
                }
            }
            return null;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Name = "FormReportBooks";
            this.ResumeLayout(false);
        }
    }
}