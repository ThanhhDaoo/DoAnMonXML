using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormReportLoans : Form
    {
        private TabControl tabControl;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Panel pnlStats;

        public FormReportLoans()
        {
            InitializeComponent();
            SetupUI();
            LoadAllReports();
        }

        private void SetupUI()
        {
            // Cấu hình Form
            this.Text = "Báo Cáo Thống Kê Mượn/Trả Sách";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Header Panel
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            headerPanel.BackColor = Color.FromArgb(142, 68, 173);

            Label lblTitle = new Label();
            lblTitle.Text = "📊 BÁO CÁO THỐNG KÊ MƯỢN/TRẢ SÁCH";
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

            // Filter Panel
            Panel filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 60;
            filterPanel.BackColor = Color.White;

            Label lblFrom = new Label();
            lblFrom.Text = "Từ ngày:";
            lblFrom.Location = new Point(20, 20);
            lblFrom.Size = new Size(70, 25);

            dtpFrom = new DateTimePicker();
            dtpFrom.Location = new Point(90, 18);
            dtpFrom.Size = new Size(150, 25);
            dtpFrom.Format = DateTimePickerFormat.Short;
            dtpFrom.Value = DateTime.Now.AddMonths(-1);

            Label lblTo = new Label();
            lblTo.Text = "Đến ngày:";
            lblTo.Location = new Point(260, 20);
            lblTo.Size = new Size(80, 25);

            dtpTo = new DateTimePicker();
            dtpTo.Location = new Point(340, 18);
            dtpTo.Size = new Size(150, 25);
            dtpTo.Format = DateTimePickerFormat.Short;
            dtpTo.Value = DateTime.Now;

            Button btnFilter = new Button();
            btnFilter.Text = "🔍 Lọc";
            btnFilter.Location = new Point(510, 15);
            btnFilter.Size = new Size(90, 30);
            btnFilter.BackColor = Color.FromArgb(52, 152, 219);
            btnFilter.ForeColor = Color.White;
            btnFilter.FlatStyle = FlatStyle.Flat;
            btnFilter.Click += (s, e) => LoadAllReports();

            Button btnRefresh = new Button();
            btnRefresh.Text = "🔄 Làm mới";
            btnRefresh.Location = new Point(610, 15);
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.BackColor = Color.FromArgb(46, 204, 113);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Click += (s, e) => { dtpFrom.Value = DateTime.Now.AddMonths(-1); dtpTo.Value = DateTime.Now; LoadAllReports(); };

            Button btnClose = new Button();
            btnClose.Text = "❌ Đóng";
            btnClose.Location = new Point(720, 15);
            btnClose.Size = new Size(90, 30);
            btnClose.BackColor = Color.FromArgb(231, 76, 60);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Click += (s, e) => this.Close();

            filterPanel.Controls.Add(lblFrom);
            filterPanel.Controls.Add(dtpFrom);
            filterPanel.Controls.Add(lblTo);
            filterPanel.Controls.Add(dtpTo);
            filterPanel.Controls.Add(btnFilter);
            filterPanel.Controls.Add(btnRefresh);
            filterPanel.Controls.Add(btnClose);

            // Tab Control
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10);

            // Tabs
            TabPage tab1 = new TabPage("Tổng quan");
            SetupTab1(tab1);

            TabPage tab2 = new TabPage("Theo trạng thái");
            SetupTab2(tab2);

            TabPage tab3 = new TabPage("Theo độc giả");
            SetupTab3(tab3);

            TabPage tab4 = new TabPage("Quá hạn");
            SetupTab4(tab4);

            TabPage tab5 = new TabPage("Chi tiết");
            SetupTab5(tab5);

            tabControl.TabPages.Add(tab1);
            tabControl.TabPages.Add(tab2);
            tabControl.TabPages.Add(tab3);
            tabControl.TabPages.Add(tab4);
            tabControl.TabPages.Add(tab5);

            // Add controls
            this.Controls.Add(tabControl);
            this.Controls.Add(filterPanel);
            this.Controls.Add(headerPanel);
        }

        // TAB 1: TỔNG QUAN
        private void SetupTab1(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 244, 248);

            pnlStats = new Panel();
            pnlStats.Location = new Point(20, 20);
            pnlStats.Size = new Size(1040, 120);
            pnlStats.Name = "pnlStats";

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvTab1";
            dgv.Location = new Point(20, 160);
            dgv.Size = new Size(1040, 380);
            dgv.BackgroundColor = Color.White;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tab.Controls.Add(pnlStats);
            tab.Controls.Add(dgv);
        }

        // TAB 2: THEO TRẠNG THÁI
        private void SetupTab2(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 244, 248);

            Label lbl = new Label();
            lbl.Text = "📊 THỐNG KÊ THEO TRẠNG THÁI";
            lbl.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(41, 128, 185);
            lbl.Location = new Point(20, 20);
            lbl.AutoSize = true;

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvTab2";
            dgv.Location = new Point(20, 60);
            dgv.Size = new Size(1040, 480);
            dgv.BackgroundColor = Color.White;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tab.Controls.Add(lbl);
            tab.Controls.Add(dgv);
        }

        // TAB 3: THEO ĐỘC GIẢ
        private void SetupTab3(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 244, 248);

            Panel pnl = new Panel();
            pnl.Location = new Point(20, 20);
            pnl.Size = new Size(1040, 60);
            pnl.BackColor = Color.FromArgb(46, 204, 113);

            Label lbl = new Label();
            lbl.Text = "👥 TOP 10 ĐỘC GIẢ MƯỢN NHIỀU NHẤT";
            lbl.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(15, 20);
            lbl.AutoSize = true;

            pnl.Controls.Add(lbl);

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvTab3";
            dgv.Location = new Point(20, 100);
            dgv.Size = new Size(1040, 440);
            dgv.BackgroundColor = Color.White;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tab.Controls.Add(pnl);
            tab.Controls.Add(dgv);
        }

        // TAB 4: QUÁ HẠN
        private void SetupTab4(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 244, 248);

            Panel pnl = new Panel();
            pnl.Location = new Point(20, 20);
            pnl.Size = new Size(1040, 80);
            pnl.BackColor = Color.FromArgb(231, 76, 60);

            Label lbl1 = new Label();
            lbl1.Text = "⚠️ DANH SÁCH SÁCH QUÁ HẠN";
            lbl1.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lbl1.ForeColor = Color.White;
            lbl1.Location = new Point(15, 15);
            lbl1.AutoSize = true;

            Label lbl2 = new Label();
            lbl2.Name = "lblCount";
            lbl2.Text = "Số phiếu quá hạn: 0";
            lbl2.Font = new Font("Segoe UI", 11);
            lbl2.ForeColor = Color.White;
            lbl2.Location = new Point(15, 45);
            lbl2.AutoSize = true;

            pnl.Controls.Add(lbl1);
            pnl.Controls.Add(lbl2);

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvTab4";
            dgv.Location = new Point(20, 120);
            dgv.Size = new Size(1040, 420);
            dgv.BackgroundColor = Color.White;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tab.Controls.Add(pnl);
            tab.Controls.Add(dgv);
        }

        // TAB 5: CHI TIẾT
        private void SetupTab5(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 244, 248);

            Panel filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 60;
            filterPanel.BackColor = Color.White;

            Label lbl = new Label();
            lbl.Text = "Trạng thái:";
            lbl.Location = new Point(20, 20);
            lbl.Size = new Size(80, 25);

            ComboBox cbo = new ComboBox();
            cbo.Name = "cboStatus";
            cbo.Location = new Point(100, 18);
            cbo.Size = new Size(150, 25);
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbo.Items.AddRange(new string[] { "Tất cả", "Borrowed", "Returned", "Overdue" });
            cbo.SelectedIndex = 0;
            cbo.SelectedIndexChanged += CboStatus_Changed;

            Button btn = new Button();
            btn.Text = "Tất cả";
            btn.Location = new Point(270, 15);
            btn.Size = new Size(100, 30);
            btn.BackColor = Color.FromArgb(52, 152, 219);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Click += (s, e) => { cbo.SelectedIndex = 0; LoadTab5(); };

            filterPanel.Controls.Add(lbl);
            filterPanel.Controls.Add(cbo);
            filterPanel.Controls.Add(btn);

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvTab5";
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
            LoadTab5();
        }

        private void LoadTab1()
        {
            try
            {
                string dateFrom = dtpFrom.Value.ToString("yyyy-MM-dd");
                string dateTo = dtpTo.Value.ToString("yyyy-MM-dd");

                string query = string.Format(@"SELECT 
                    (SELECT COUNT(*) FROM Loans WHERE LoanDate BETWEEN '{0}' AND '{1}') AS Total,
                    (SELECT COUNT(*) FROM Loans WHERE Status = 'Borrowed') AS Borrowed,
                    (SELECT COUNT(*) FROM Loans WHERE Status = 'Returned' AND ReturnDate BETWEEN '{0}' AND '{1}') AS Returned,
                    (SELECT COUNT(*) FROM Loans WHERE Status = 'Overdue') AS Overdue", dateFrom, dateTo);

                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                if (dt.Rows.Count > 0)
                {
                    pnlStats.Controls.Clear();

                    Panel p1 = CreateStatBox("Tổng phiếu", dt.Rows[0]["Total"].ToString(), 0, Color.FromArgb(52, 152, 219));
                    Panel p2 = CreateStatBox("Đang mượn", dt.Rows[0]["Borrowed"].ToString(), 260, Color.FromArgb(46, 204, 113));
                    Panel p3 = CreateStatBox("Đã trả", dt.Rows[0]["Returned"].ToString(), 520, Color.FromArgb(26, 188, 156));
                    Panel p4 = CreateStatBox("Quá hạn", dt.Rows[0]["Overdue"].ToString(), 780, Color.FromArgb(231, 76, 60));

                    pnlStats.Controls.Add(p1);
                    pnlStats.Controls.Add(p2);
                    pnlStats.Controls.Add(p3);
                    pnlStats.Controls.Add(p4);
                }

                // Chi tiết
                string query2 = string.Format(@"SELECT Status AS [Trạng thái], COUNT(*) AS [Số lượng]
                    FROM Loans WHERE LoanDate BETWEEN '{0}' AND '{1}'
                    GROUP BY Status ORDER BY COUNT(*) DESC", dateFrom, dateTo);

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
            string dateFrom = dtpFrom.Value.ToString("yyyy-MM-dd");
            string dateTo = dtpTo.Value.ToString("yyyy-MM-dd");

            string query = string.Format(@"SELECT Status AS [Trạng thái], COUNT(*) AS [Số lượng]
                FROM Loans WHERE LoanDate BETWEEN '{0}' AND '{1}'
                GROUP BY Status ORDER BY COUNT(*) DESC", dateFrom, dateTo);

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            DataGridView dgv = GetDGV(1, "dgvTab2");
            dgv.DataSource = dt;
        }

        private void LoadTab3()
        {
            string dateFrom = dtpFrom.Value.ToString("yyyy-MM-dd");
            string dateTo = dtpTo.Value.ToString("yyyy-MM-dd");

            string query = string.Format(@"SELECT TOP 10 M.FullName AS [Họ tên], M.Email AS [Email], 
                M.Phone AS [Điện thoại], COUNT(L.LoanID) AS [Số lần mượn]
                FROM Members M INNER JOIN Loans L ON M.MemberID = L.MemberID
                WHERE L.LoanDate BETWEEN '{0}' AND '{1}'
                GROUP BY M.MemberID, M.FullName, M.Email, M.Phone
                ORDER BY COUNT(L.LoanID) DESC", dateFrom, dateTo);

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
            string query = @"SELECT L.LoanID AS [ID], B.Title AS [Tên sách], M.FullName AS [Độc giả],
                M.Phone AS [Điện thoại], L.LoanDate AS [Ngày mượn], L.DueDate AS [Hạn trả],
                DATEDIFF(day, L.DueDate, GETDATE()) AS [Số ngày quá hạn]
                FROM Loans L
                INNER JOIN Books B ON L.BookID = B.BookID
                INNER JOIN Members M ON L.MemberID = M.MemberID
                WHERE L.Status = 'Overdue' OR (L.DueDate < GETDATE() AND L.Status = 'Borrowed')
                ORDER BY DATEDIFF(day, L.DueDate, GETDATE()) DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            DataGridView dgv = GetDGV(3, "dgvTab4");
            dgv.DataSource = dt;

            // Cập nhật số lượng
            Label lblCount = GetLabel(3, "lblCount");
            if (lblCount != null)
                lblCount.Text = "Số phiếu quá hạn: " + dt.Rows.Count.ToString();

            // Tô màu cảnh báo
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells["Số ngày quá hạn"].Value != null)
                {
                    int days = Convert.ToInt32(row.Cells["Số ngày quá hạn"].Value);
                    if (days > 30)
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 100, 100);
                    else if (days > 14)
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 100);
                }
            }
        }

        private void LoadTab5()
        {
            string dateFrom = dtpFrom.Value.ToString("yyyy-MM-dd");
            string dateTo = dtpTo.Value.ToString("yyyy-MM-dd");

            string query = string.Format(@"SELECT L.LoanID AS [ID], B.Title AS [Tên sách], 
                M.FullName AS [Độc giả], L.LoanDate AS [Ngày mượn], L.DueDate AS [Hạn trả],
                L.ReturnDate AS [Ngày trả], L.Status AS [Trạng thái], L.Notes AS [Ghi chú]
                FROM Loans L
                INNER JOIN Books B ON L.BookID = B.BookID
                INNER JOIN Members M ON L.MemberID = M.MemberID
                WHERE L.LoanDate BETWEEN '{0}' AND '{1}'
                ORDER BY L.LoanID DESC", dateFrom, dateTo);

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            DataGridView dgv = GetDGV(4, "dgvTab5");
            dgv.DataSource = dt;
        }

        private void CboStatus_Changed(object sender, EventArgs e)
        {
            ComboBox cbo = sender as ComboBox;
            DataGridView dgv = GetDGV(4, "dgvTab5");

            if (cbo != null && dgv != null && dgv.DataSource is DataTable dt)
            {
                if (cbo.SelectedItem.ToString() == "Tất cả")
                {
                    dt.DefaultView.RowFilter = "";
                }
                else
                {
                    dt.DefaultView.RowFilter = string.Format("[Trạng thái] = '{0}'", cbo.SelectedItem);
                }
            }
        }

        // Helper methods
        private Panel CreateStatBox(string title, string value, int x, Color color)
        {
            Panel p = new Panel();
            p.Size = new Size(240, 100);
            p.Location = new Point(x, 10);
            p.BackColor = color;

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

        private Label GetLabel(int tabIndex, string name)
        {
            foreach (Control ctrl in tabControl.TabPages[tabIndex].Controls)
            {
                if (ctrl is Panel)
                {
                    foreach (Control c in ctrl.Controls)
                    {
                        if (c is Label && c.Name == name)
                            return (Label)c;
                    }
                }
            }
            return null;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Name = "FormReportLoans";
            this.ResumeLayout(false);
        }
    }
}