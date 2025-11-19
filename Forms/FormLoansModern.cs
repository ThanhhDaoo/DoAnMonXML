using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormLoansModern : Form
    {
        private DataGridView dgvLoans;
        private TextBox txtSearch;
        private Panel detailsPanel;
        private TextBox txtLoanID;
        private ComboBox cboBook, cboMember, cboStatus;
        private DateTimePicker dtpLoanDate, dtpDueDate, dtpReturnDate;
        private TextBox txtNotes;
        private CheckBox chkReturned;

        public FormLoansModern()
        {
            InitializeComponent();
            SetupModernUI();
            LoadComboBoxData();
            LoadData();
        }

        private void SetupModernUI()
        {
            this.Text = "Quản Lý Mượn/Trả Sách";
            this.Size = new Size(1700, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = ModernUIHelper.Colors.Light;

            // Header
            Panel header = ModernUIHelper.CreateGradientHeader(
                "📝 QUẢN LÝ MƯỢN/TRẢ SÁCH",
                "Quản lý phiếu mượn và trả sách",
                ModernUIHelper.Colors.Info,
                ModernUIHelper.Colors.InfoDark,
                100
            );

            // Toolbar
            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 20)
            };

            Panel searchBox = ModernUIHelper.CreateSearchBox("Tìm kiếm theo sách, độc giả, trạng thái...", 400);
            searchBox.Location = new Point(30, 20);
            txtSearch = (TextBox)searchBox.Tag;
            txtSearch.TextChanged += (s, e) =>
            {
                if (txtSearch.Text != "Tìm kiếm theo sách, độc giả, trạng thái...")
                    SearchLoans();
            };

            Button btnAdd = ModernUIHelper.CreateIconButton("➕", "Mượn sách", ModernUIHelper.Colors.Success, 130);
            btnAdd.Location = new Point(460, 20);
            btnAdd.Click += BtnAdd_Click;

            Button btnReturn = ModernUIHelper.CreateIconButton("✅", "Trả sách", ModernUIHelper.Colors.Primary, 130);
            btnReturn.Location = new Point(610, 20);
            btnReturn.Click += BtnReturn_Click;

            Button btnDelete = ModernUIHelper.CreateIconButton("🗑️", "Xóa", ModernUIHelper.Colors.Danger, 120);
            btnDelete.Location = new Point(760, 20);
            btnDelete.Click += BtnDelete_Click;

            Button btnRefresh = ModernUIHelper.CreateIconButton("🔄", "Làm mới", ModernUIHelper.Colors.Gray, 130);
            btnRefresh.Location = new Point(900, 20);
            btnRefresh.Click += (s, e) => LoadData();

            Button btnExport = ModernUIHelper.CreateIconButton("📤", "Export", ModernUIHelper.Colors.Warning, 130);
            btnExport.Location = new Point(1050, 20);
            btnExport.Click += BtnExport_Click;

            Button btnImport = ModernUIHelper.CreateIconButton("📥", "Import", ModernUIHelper.Colors.Info, 130);
            btnImport.Location = new Point(1200, 20);
            btnImport.Click += BtnImport_Click;

            toolbar.Controls.AddRange(new Control[] {
                searchBox, btnAdd, btnReturn, btnDelete, btnRefresh, btnExport, btnImport
            });

            // Content area
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 20, 30, 30),
                BackColor = ModernUIHelper.Colors.Light
            };

            // DataGridView container
            Panel dgvContainer = new Panel
            {
                Location = new Point(30, 20),
                Size = new Size(1100, 700),
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

            dgvLoans = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(1070, 670),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            ModernUIHelper.StyleDataGridView(dgvLoans, ModernUIHelper.Colors.Info);
            dgvLoans.SelectionChanged += DgvLoans_SelectionChanged;

            dgvContainer.Controls.Add(dgvLoans);

            // Details panel
            detailsPanel = new Panel
            {
                Location = new Point(1150, 20),
                Size = new Size(500, 700),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };
            detailsPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(detailsPanel.ClientRectangle, 15))
                {
                    detailsPanel.Region = new Region(path);
                }
            };

            Label lblDetailsTitle = new Label
            {
                Text = "📋 THÔNG TIN PHIẾU MƯỢN",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, 20),
                Size = new Size(460, 30)
            };

            int yPos = 70;
            int spacing = 75;

            txtLoanID = CreateDetailField("ID Phiếu:", yPos, true);

            // ComboBox Sách
            Label lblBook = new Label
            {
                Text = "Sách:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, yPos += spacing),
                AutoSize = true
            };

            Panel cboBookPanel = CreateComboPanel(yPos + 25);
            cboBook = new ComboBox
            {
                Location = new Point(12, 8),
                Size = new Size(436, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cboBookPanel.Controls.Add(cboBook);
            detailsPanel.Controls.Add(lblBook);
            detailsPanel.Controls.Add(cboBookPanel);

            // ComboBox Độc giả
            Label lblMember = new Label
            {
                Text = "Độc giả:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, yPos += spacing),
                AutoSize = true
            };

            Panel cboMemberPanel = CreateComboPanel(yPos + 25);
            cboMember = new ComboBox
            {
                Location = new Point(12, 8),
                Size = new Size(436, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cboMemberPanel.Controls.Add(cboMember);
            detailsPanel.Controls.Add(lblMember);
            detailsPanel.Controls.Add(cboMemberPanel);

            // DateTimePickers
            dtpLoanDate = CreateDateField("Ngày mượn:", yPos += spacing);
            dtpDueDate = CreateDateField("Hạn trả:", yPos += spacing);
            dtpReturnDate = CreateDateField("Ngày trả:", yPos += spacing);

            // Checkbox Đã trả
            chkReturned = new CheckBox
            {
                Text = "✅ Đã trả sách",
                Location = new Point(20, yPos += spacing),
                Size = new Size(460, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Success
            };
            chkReturned.CheckedChanged += (s, e) => dtpReturnDate.Enabled = chkReturned.Checked;
            detailsPanel.Controls.Add(chkReturned);

            // ComboBox Status
            Label lblStatus = new Label
            {
                Text = "Trạng thái:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, yPos += 50),
                AutoSize = true
            };

            Panel cboStatusPanel = CreateComboPanel(yPos + 25);
            cboStatus = new ComboBox
            {
                Location = new Point(12, 8),
                Size = new Size(436, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cboStatus.Items.AddRange(new string[] { "Borrowed", "Returned", "Overdue", "Lost" });
            cboStatusPanel.Controls.Add(cboStatus);
            detailsPanel.Controls.Add(lblStatus);
            detailsPanel.Controls.Add(cboStatusPanel);

            // Notes
            Label lblNotes = new Label
            {
                Text = "Ghi chú:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, yPos += 75),
                AutoSize = true
            };

            Panel notesPanel = new Panel
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(460, 80),
                BackColor = ModernUIHelper.Colors.Light
            };
            notesPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(notesPanel.ClientRectangle, 8))
                {
                    notesPanel.Region = new Region(path);
                }
            };

            txtNotes = new TextBox
            {
                Location = new Point(10, 10),
                Size = new Size(440, 60),
                Multiline = true,
                BorderStyle = BorderStyle.None,
                BackColor = ModernUIHelper.Colors.Light,
                Font = new Font("Segoe UI", 10)
            };
            notesPanel.Controls.Add(txtNotes);

            detailsPanel.Controls.Add(lblDetailsTitle);
            detailsPanel.Controls.Add(lblNotes);
            detailsPanel.Controls.Add(notesPanel);

            contentPanel.Controls.AddRange(new Control[] { dgvContainer, detailsPanel });

            this.Controls.Add(contentPanel);
            this.Controls.Add(toolbar);
            this.Controls.Add(header);
        }

        private TextBox CreateDetailField(string label, int yPos, bool readOnly = false)
        {
            Label lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, yPos),
                AutoSize = true
            };

            Panel txtPanel = new Panel
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(460, 40),
                BackColor = ModernUIHelper.Colors.Light
            };
            txtPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(txtPanel.ClientRectangle, 8))
                {
                    txtPanel.Region = new Region(path);
                }
            };

            TextBox txt = new TextBox
            {
                Location = new Point(12, 8),
                Size = new Size(436, 25),
                BorderStyle = BorderStyle.None,
                BackColor = ModernUIHelper.Colors.Light,
                Font = new Font("Segoe UI", 11),
                ReadOnly = readOnly
            };

            txtPanel.Controls.Add(txt);
            detailsPanel.Controls.Add(lbl);
            detailsPanel.Controls.Add(txtPanel);

            return txt;
        }

        private DateTimePicker CreateDateField(string label, int yPos)
        {
            Label lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, yPos),
                AutoSize = true
            };

            Panel dtpPanel = new Panel
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(460, 40),
                BackColor = ModernUIHelper.Colors.Light
            };
            dtpPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(dtpPanel.ClientRectangle, 8))
                {
                    dtpPanel.Region = new Region(path);
                }
            };

            DateTimePicker dtp = new DateTimePicker
            {
                Location = new Point(12, 8),
                Size = new Size(436, 25),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };

            dtpPanel.Controls.Add(dtp);
            detailsPanel.Controls.Add(lbl);
            detailsPanel.Controls.Add(dtpPanel);

            return dtp;
        }

        private Panel CreateComboPanel(int yPos)
        {
            Panel panel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(460, 40),
                BackColor = ModernUIHelper.Colors.Light
            };
            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(panel.ClientRectangle, 8))
                {
                    panel.Region = new Region(path);
                }
            };
            return panel;
        }

        private void LoadComboBoxData()
        {
            try
            {
                string queryBooks = "SELECT BookID, Title FROM Books ORDER BY Title";
                DataTable dtBooks = DatabaseHelper.ExecuteQuery(queryBooks);
                cboBook.DataSource = dtBooks;
                cboBook.DisplayMember = "Title";
                cboBook.ValueMember = "BookID";

                string queryMembers = "SELECT MemberID, FullName FROM Members ORDER BY FullName";
                DataTable dtMembers = DatabaseHelper.ExecuteQuery(queryMembers);
                cboMember.DataSource = dtMembers;
                cboMember.DisplayMember = "FullName";
                cboMember.ValueMember = "MemberID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu ComboBox:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                string query = @"SELECT L.LoanID, B.Title AS BookTitle, M.FullName AS MemberName,
                               L.LoanDate, L.DueDate, L.ReturnDate, L.Status, L.Notes,
                               L.BookID, L.MemberID
                               FROM Loans L
                               INNER JOIN Books B ON L.BookID = B.BookID
                               INNER JOIN Members M ON L.MemberID = M.MemberID
                               ORDER BY L.LoanID DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvLoans.DataSource = dt;

                if (dgvLoans.Columns.Count > 0)
                {
                    dgvLoans.Columns["LoanID"].HeaderText = "ID";
                    dgvLoans.Columns["BookTitle"].HeaderText = "Tên sách";
                    dgvLoans.Columns["MemberName"].HeaderText = "Độc giả";
                    dgvLoans.Columns["LoanDate"].HeaderText = "Ngày mượn";
                    dgvLoans.Columns["DueDate"].HeaderText = "Hạn trả";
                    dgvLoans.Columns["ReturnDate"].HeaderText = "Ngày trả";
                    dgvLoans.Columns["Status"].HeaderText = "Trạng thái";
                    dgvLoans.Columns["Notes"].HeaderText = "Ghi chú";

                    dgvLoans.Columns["BookID"].Visible = false;
                    dgvLoans.Columns["MemberID"].Visible = false;
                }

                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvLoans_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLoans.CurrentRow != null)
            {
                DataGridViewRow row = dgvLoans.CurrentRow;
                txtLoanID.Text = row.Cells["LoanID"].Value?.ToString();
                cboBook.SelectedValue = row.Cells["BookID"].Value;
                cboMember.SelectedValue = row.Cells["MemberID"].Value;

                if (row.Cells["LoanDate"].Value != DBNull.Value)
                    dtpLoanDate.Value = Convert.ToDateTime(row.Cells["LoanDate"].Value);

                if (row.Cells["DueDate"].Value != DBNull.Value)
                    dtpDueDate.Value = Convert.ToDateTime(row.Cells["DueDate"].Value);

                if (row.Cells["ReturnDate"].Value != DBNull.Value)
                {
                    dtpReturnDate.Value = Convert.ToDateTime(row.Cells["ReturnDate"].Value);
                    chkReturned.Checked = true;
                }
                else
                {
                    chkReturned.Checked = false;
                    dtpReturnDate.Value = DateTime.Now;
                }

                cboStatus.SelectedItem = row.Cells["Status"].Value?.ToString() ?? "Borrowed";
                txtNotes.Text = row.Cells["Notes"].Value?.ToString();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (cboBook.SelectedValue == null || cboMember.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn sách và độc giả!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"INSERT INTO Loans (BookID, MemberID, LoanDate, DueDate, Status, Notes)
                           VALUES (@BookID, @MemberID, @LoanDate, @DueDate, @Status, @Notes)";

            SqlParameter[] parameters = {
                new SqlParameter("@BookID", cboBook.SelectedValue),
                new SqlParameter("@MemberID", cboMember.SelectedValue),
                new SqlParameter("@LoanDate", dtpLoanDate.Value.Date),
                new SqlParameter("@DueDate", dtpDueDate.Value.Date),
                new SqlParameter("@Status", "Borrowed"),
                new SqlParameter("@Notes", txtNotes.Text)
            };

            if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Thêm phiếu mượn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLoanID.Text))
            {
                MessageBox.Show("Vui lòng chọn phiếu mượn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"UPDATE Loans SET ReturnDate = @ReturnDate, Status = 'Returned'
                           WHERE LoanID = @LoanID";

            SqlParameter[] parameters = {
                new SqlParameter("@LoanID", int.Parse(txtLoanID.Text)),
                new SqlParameter("@ReturnDate", DateTime.Now.Date)
            };

            if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Trả sách thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLoanID.Text))
            {
                MessageBox.Show("Vui lòng chọn phiếu mượn cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Bạn có chắc muốn xóa phiếu mượn này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                string query = "DELETE FROM Loans WHERE LoanID = @LoanID";
                SqlParameter[] parameters = { new SqlParameter("@LoanID", int.Parse(txtLoanID.Text)) };

                if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                {
                    MessageBox.Show("Xóa thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
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
                    FileName = $"PhieuMuon_{DateTime.Now:yyyyMMdd_HHmmss}.xml"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    XMLHelperLoans.ExportLoansToXML(dgvLoans, saveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Export:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml"
                };

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    if (XMLHelperLoans.ImportLoansFromXML(openDialog.FileName))
                        LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Import:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchLoans()
        {
            if (dgvLoans.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = string.Format(
                    "BookTitle LIKE '%{0}%' OR MemberName LIKE '%{0}%' OR Status LIKE '%{0}%'",
                    txtSearch.Text);
            }
        }

        private void ClearInputs()
        {
            txtLoanID.Clear();
            dtpLoanDate.Value = DateTime.Now;
            dtpDueDate.Value = DateTime.Now.AddDays(14);
            dtpReturnDate.Value = DateTime.Now;
            chkReturned.Checked = false;
            txtNotes.Clear();
            if (cboStatus.Items.Count > 0) cboStatus.SelectedIndex = 0;
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
            this.ClientSize = new System.Drawing.Size(1700, 900);
            this.Name = "FormLoansModern";
            this.ResumeLayout(false);
        }
    }
}
