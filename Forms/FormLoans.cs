using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormLoans : Form
    {
        private DataGridView dgvLoans;
        private TextBox txtSearch;
        private Button btnAdd, btnReturn, btnDelete, btnRefresh;
        private Button btnExport, btnImport;
        private GroupBox grpInfo;
        private TextBox txtLoanID;
        private ComboBox cboBook, cboMember, cboStatus;
        private DateTimePicker dtpLoanDate, dtpDueDate, dtpReturnDate;
        private TextBox txtNotes;
        private CheckBox chkReturned;

        public FormLoans()
        {
            InitializeComponent();
            SetupUI();
            LoadComboBoxData();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "Quản Lý Mượn/Trả Sách";
            this.Size = new Size(1300, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // === PANEL TRÊN ===
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            Label lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Location = new Point(10, 17),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };

            txtSearch = new TextBox
            {
                Location = new Point(90, 15),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.TextChanged += (s, e) => SearchLoans();

            btnAdd = CreateButton("Mượn sách", 360, Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;

            btnReturn = CreateButton("Trả sách", 460, Color.FromArgb(52, 152, 219));
            btnReturn.Click += BtnReturn_Click;

            btnDelete = CreateButton("Xóa", 560, Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = CreateButton("Làm mới", 650, Color.FromArgb(149, 165, 166));
            btnRefresh.Click += (s, e) => LoadData();

            btnExport = CreateButton("📤 Export", 770, Color.FromArgb(230, 126, 34));
            btnExport.Click += BtnExport_Click;

            btnImport = CreateButton("📥 Import", 880, Color.FromArgb(155, 89, 182));
            btnImport.Click += BtnImport_Click;

            topPanel.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnAdd, btnReturn, btnDelete,
                btnRefresh, btnExport, btnImport
            });

            // === DATAGRIDVIEW ===
            dgvLoans = new DataGridView
            {
                Location = new Point(10, 70),
                Size = new Size(880, 570),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9)
            };
            dgvLoans.SelectionChanged += DgvLoans_SelectionChanged;

            // === PANEL PHẢI ===
            grpInfo = new GroupBox
            {
                Text = "Thông tin mượn/trả",
                Location = new Point(900, 70),
                Size = new Size(380, 570),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.White
            };

            int lblX = 15, ctrlX = 120, startY = 30, spacing = 50;

            CreateLabel("ID Phiếu:", lblX, startY, grpInfo);
            txtLoanID = CreateTextBox(ctrlX, startY, grpInfo, true);

            CreateLabel("Sách:", lblX, startY + spacing, grpInfo);
            cboBook = new ComboBox
            {
                Location = new Point(ctrlX, startY + spacing),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            grpInfo.Controls.Add(cboBook);

            CreateLabel("Độc giả:", lblX, startY + spacing * 2, grpInfo);
            cboMember = new ComboBox
            {
                Location = new Point(ctrlX, startY + spacing * 2),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            grpInfo.Controls.Add(cboMember);

            CreateLabel("Ngày mượn:", lblX, startY + spacing * 3, grpInfo);
            dtpLoanDate = new DateTimePicker
            {
                Location = new Point(ctrlX, startY + spacing * 3),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short
            };
            grpInfo.Controls.Add(dtpLoanDate);

            CreateLabel("Hạn trả:", lblX, startY + spacing * 4, grpInfo);
            dtpDueDate = new DateTimePicker
            {
                Location = new Point(ctrlX, startY + spacing * 4),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short
            };
            grpInfo.Controls.Add(dtpDueDate);

            CreateLabel("Ngày trả:", lblX, startY + spacing * 5, grpInfo);
            dtpReturnDate = new DateTimePicker
            {
                Location = new Point(ctrlX, startY + spacing * 5),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short
            };
            grpInfo.Controls.Add(dtpReturnDate);

            chkReturned = new CheckBox
            {
                Text = "Đã trả sách",
                Location = new Point(ctrlX, startY + spacing * 6),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96)
            };
            chkReturned.CheckedChanged += (s, e) => dtpReturnDate.Enabled = chkReturned.Checked;
            grpInfo.Controls.Add(chkReturned);

            CreateLabel("Trạng thái:", lblX, startY + spacing * 7, grpInfo);
            cboStatus = new ComboBox
            {
                Location = new Point(ctrlX, startY + spacing * 7),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new string[] { "Borrowed", "Returned", "Overdue", "Lost" });
            grpInfo.Controls.Add(cboStatus);

            CreateLabel("Ghi chú:", lblX, startY + spacing * 8, grpInfo);
            txtNotes = new TextBox
            {
                Location = new Point(ctrlX, startY + spacing * 8),
                Size = new Size(240, 60),
                Multiline = true,
                Font = new Font("Segoe UI", 9)
            };
            grpInfo.Controls.Add(txtNotes);

            this.Controls.AddRange(new Control[] { topPanel, dgvLoans, grpInfo });
        }

        private Button CreateButton(string text, int x, Color backColor)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, 12),
                Size = new Size(95, 35),
                BackColor = backColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void CreateLabel(string text, int x, int y, Control parent)
        {
            Label lbl = new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 9)
            };
            parent.Controls.Add(lbl);
        }

        private TextBox CreateTextBox(int x, int y, Control parent, bool readOnly = false)
        {
            TextBox txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 9),
                ReadOnly = readOnly
            };
            parent.Controls.Add(txt);
            return txt;
        }

        private void LoadComboBoxData()
        {
            // Load Books
            string queryBooks = "SELECT BookID, Title FROM Books ORDER BY Title";
            DataTable dtBooks = DatabaseHelper.ExecuteQuery(queryBooks);
            cboBook.DataSource = dtBooks;
            cboBook.DisplayMember = "Title";
            cboBook.ValueMember = "BookID";

            // Load Members
            string queryMembers = "SELECT MemberID, FullName FROM Members ORDER BY FullName";
            DataTable dtMembers = DatabaseHelper.ExecuteQuery(queryMembers);
            cboMember.DataSource = dtMembers;
            cboMember.DisplayMember = "FullName";
            cboMember.ValueMember = "MemberID";
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
                MessageBox.Show($"Lỗi tải dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void FormLoans_Load(object sender, EventArgs e)
        {

        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (FormLoanDialog dialog = new FormLoanDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLoanID.Text))
            {
                MessageBox.Show("Vui lòng chọn phiếu mượn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("Trả sách thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLoanID.Text))
            {
                MessageBox.Show("Vui lòng chọn phiếu mượn cần xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Bạn có chắc muốn xóa phiếu mượn này?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                string query = "DELETE FROM Loans WHERE LoanID = @LoanID";
                SqlParameter[] parameters = {
                    new SqlParameter("@LoanID", int.Parse(txtLoanID.Text))
                };

                if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show($"Lỗi Export:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Import:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormLoans
            // 
            this.ClientSize = new System.Drawing.Size(1300, 700);
            this.Name = "FormLoans";
            this.Load += new System.EventHandler(this.FormLoans_Load);
            this.ResumeLayout(false);

        }
    }

    public class XMLHelperLoans
    {
        public static bool ExportLoansToXML(DataGridView dgv, string filePath)
        {
            try
            {
                var xmlDoc = new System.Xml.Linq.XDocument(
                    new System.Xml.Linq.XDeclaration("1.0", "utf-8", "yes"),
                    new System.Xml.Linq.XElement("LoansList",
                        new System.Xml.Linq.XAttribute("ExportDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    )
                );

                var root = xmlDoc.Root;

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    var loan = new System.Xml.Linq.XElement("Loan",
                        new System.Xml.Linq.XElement("LoanID", row.Cells["LoanID"].Value),
                        new System.Xml.Linq.XElement("BookID", row.Cells["BookID"].Value),
                        new System.Xml.Linq.XElement("MemberID", row.Cells["MemberID"].Value),
                        new System.Xml.Linq.XElement("LoanDate", row.Cells["LoanDate"].Value),
                        new System.Xml.Linq.XElement("DueDate", row.Cells["DueDate"].Value),
                        new System.Xml.Linq.XElement("ReturnDate", row.Cells["ReturnDate"].Value ?? ""),
                        new System.Xml.Linq.XElement("Status", row.Cells["Status"].Value),
                        new System.Xml.Linq.XElement("Notes", row.Cells["Notes"].Value ?? "")
                    );

                    root.Add(loan);
                }

                xmlDoc.Save(filePath);
                MessageBox.Show("Export thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool ImportLoansFromXML(string filePath)
        {
            try
            {
                var xmlDoc = System.Xml.Linq.XDocument.Load(filePath);
                var loans = xmlDoc.Root.Elements("Loan");

                int success = 0;
                foreach (var loan in loans)
                {
                    string query = @"INSERT INTO Loans (BookID, MemberID, LoanDate, DueDate, ReturnDate, Status, Notes)
                                   VALUES (@BookID, @MemberID, @LoanDate, @DueDate, @ReturnDate, @Status, @Notes)";

                    SqlParameter[] parameters = {
                        new SqlParameter("@BookID", int.Parse(loan.Element("BookID").Value)),
                        new SqlParameter("@MemberID", int.Parse(loan.Element("MemberID").Value)),
                        new SqlParameter("@LoanDate", DateTime.Parse(loan.Element("LoanDate").Value)),
                        new SqlParameter("@DueDate", DateTime.Parse(loan.Element("DueDate").Value)),
                        new SqlParameter("@ReturnDate", string.IsNullOrEmpty(loan.Element("ReturnDate").Value) ? (object)DBNull.Value : DateTime.Parse(loan.Element("ReturnDate").Value)),
                        new SqlParameter("@Status", loan.Element("Status").Value),
                        new SqlParameter("@Notes", loan.Element("Notes").Value)
                    };

                    if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                        success++;
                }

                MessageBox.Show($"Import thành công {success} phiếu mượn!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}