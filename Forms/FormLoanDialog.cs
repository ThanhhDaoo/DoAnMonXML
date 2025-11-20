using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormLoanDialog : Form
    {
        public string LoanID { get; set; }
        public int BookID { get; set; }
        public int MemberID { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }

        private ComboBox cboBook, cboMember, cboStatus;
        private DateTimePicker dtpLoanDate, dtpDueDate, dtpReturnDate;
        private TextBox txtNotes;
        private CheckBox chkReturned;
        private Button btnSave, btnCancel;
        private bool isEditMode;
        private int? editLoanID;

        public FormLoanDialog(int? loanID = null)
        {
            editLoanID = loanID;
            isEditMode = loanID.HasValue;
            LoanDate = DateTime.Now;
            DueDate = DateTime.Now.AddDays(14);
            Status = "Borrowed";
            
            this.SuspendLayout();
            SetupUI();
            LoadComboBoxData();
            this.ResumeLayout(false);
            
            if (isEditMode)
            {
                LoadLoanData(loanID.Value);
            }
        }

        private void SetupUI()
        {
            this.Text = isEditMode ? "Sửa phiếu mượn" : "Tạo phiếu mượn mới";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Header
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(46, 204, 113)
            };

            Label lblHeader = new Label
            {
                Text = isEditMode ? "✏️ SỬA PHIẾU MƯỢN" : "➕ TẠO PHIẾU MƯỢN MỚI",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(480, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            headerPanel.Controls.Add(lblHeader);

            // Content Panel
            Panel contentPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(440, 420),
                AutoScroll = true
            };

            int y = 10;
            int spacing = 60;

            CreateLabel("Chọn sách: *", 10, y, contentPanel);
            cboBook = new ComboBox
            {
                Location = new Point(10, y + 25),
                Size = new Size(410, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            contentPanel.Controls.Add(cboBook);

            y += spacing;
            CreateLabel("Chọn độc giả: *", 10, y, contentPanel);
            cboMember = new ComboBox
            {
                Location = new Point(10, y + 25),
                Size = new Size(410, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            contentPanel.Controls.Add(cboMember);

            y += spacing;
            CreateLabel("Ngày mượn:", 10, y, contentPanel);
            dtpLoanDate = new DateTimePicker
            {
                Location = new Point(10, y + 25),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            contentPanel.Controls.Add(dtpLoanDate);

            y += spacing;
            CreateLabel("Hạn trả:", 10, y, contentPanel);
            dtpDueDate = new DateTimePicker
            {
                Location = new Point(10, y + 25),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(14)
            };
            contentPanel.Controls.Add(dtpDueDate);

            y += spacing;
            chkReturned = new CheckBox
            {
                Text = "✓ Đã trả sách",
                Location = new Point(10, y),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96)
            };
            chkReturned.CheckedChanged += (s, e) => dtpReturnDate.Enabled = chkReturned.Checked;
            contentPanel.Controls.Add(chkReturned);

            CreateLabel("Ngày trả:", 10, y + 30, contentPanel);
            dtpReturnDate = new DateTimePicker
            {
                Location = new Point(10, y + 55),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now,
                Enabled = false
            };
            contentPanel.Controls.Add(dtpReturnDate);

            y += 90;
            CreateLabel("Trạng thái:", 10, y, contentPanel);
            cboStatus = new ComboBox
            {
                Location = new Point(10, y + 25),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new string[] { "Borrowed", "Returned", "Overdue", "Lost" });
            cboStatus.SelectedIndex = 0;
            contentPanel.Controls.Add(cboStatus);

            y += spacing;
            CreateLabel("Ghi chú:", 10, y, contentPanel);
            txtNotes = new TextBox
            {
                Location = new Point(10, y + 25),
                Size = new Size(410, 60),
                Multiline = true,
                Font = new Font("Segoe UI", 10),
                ScrollBars = ScrollBars.Vertical
            };
            contentPanel.Controls.Add(txtNotes);

            // Buttons
            btnSave = new Button
            {
                Text = "💾 Lưu",
                Location = new Point(150, 515),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "❌ Hủy",
                Location = new Point(280, 515),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { headerPanel, contentPanel, btnSave, btnCancel });
        }

        private void CreateLabel(string text, int x, int y, Control parent)
        {
            Label lbl = new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            parent.Controls.Add(lbl);
        }

        private void LoadComboBoxData()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLoanData(int loanID)
        {
            try
            {
                string query = "SELECT * FROM Loans WHERE LoanID = @LoanID";
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new System.Data.SqlClient.SqlParameter("@LoanID", loanID)
                };
                
                System.Data.DataTable dt = LibraryManagement.Helpers.DatabaseHelper.ExecuteQuery(query, parameters);
                
                if (dt.Rows.Count > 0)
                {
                    System.Data.DataRow row = dt.Rows[0];
                    LoanID = row["LoanID"].ToString();
                    cboBook.SelectedValue = Convert.ToInt32(row["BookID"]);
                    cboMember.SelectedValue = Convert.ToInt32(row["MemberID"]);
                    dtpLoanDate.Value = Convert.ToDateTime(row["LoanDate"]);
                    dtpDueDate.Value = Convert.ToDateTime(row["DueDate"]);
                    
                    if (row["ReturnDate"] != DBNull.Value)
                    {
                        chkReturned.Checked = true;
                        dtpReturnDate.Value = Convert.ToDateTime(row["ReturnDate"]);
                    }
                    
                    cboStatus.SelectedItem = row["Status"].ToString();
                    txtNotes.Text = row["Notes"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadData()
        {
            if (BookID > 0)
                cboBook.SelectedValue = BookID;
            
            if (MemberID > 0)
                cboMember.SelectedValue = MemberID;

            dtpLoanDate.Value = LoanDate;
            dtpDueDate.Value = DueDate;

            if (ReturnDate.HasValue)
            {
                chkReturned.Checked = true;
                dtpReturnDate.Value = ReturnDate.Value;
            }

            cboStatus.SelectedItem = Status;
            txtNotes.Text = Notes;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cboBook.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboBook.Focus();
                return;
            }

            if (cboMember.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn độc giả!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMember.Focus();
                return;
            }

            try
            {
                string query;
                System.Data.SqlClient.SqlParameter[] parameters;

                if (isEditMode)
                {
                    query = @"UPDATE Loans SET BookID = @BookID, MemberID = @MemberID, 
                             LoanDate = @LoanDate, DueDate = @DueDate, ReturnDate = @ReturnDate, 
                             Status = @Status, Notes = @Notes WHERE LoanID = @LoanID";

                    parameters = new System.Data.SqlClient.SqlParameter[] {
                        new System.Data.SqlClient.SqlParameter("@LoanID", editLoanID.Value),
                        new System.Data.SqlClient.SqlParameter("@BookID", Convert.ToInt32(cboBook.SelectedValue)),
                        new System.Data.SqlClient.SqlParameter("@MemberID", Convert.ToInt32(cboMember.SelectedValue)),
                        new System.Data.SqlClient.SqlParameter("@LoanDate", dtpLoanDate.Value.Date),
                        new System.Data.SqlClient.SqlParameter("@DueDate", dtpDueDate.Value.Date),
                        new System.Data.SqlClient.SqlParameter("@ReturnDate", chkReturned.Checked ? (object)dtpReturnDate.Value.Date : DBNull.Value),
                        new System.Data.SqlClient.SqlParameter("@Status", cboStatus.SelectedItem.ToString()),
                        new System.Data.SqlClient.SqlParameter("@Notes", txtNotes.Text.Trim())
                    };
                }
                else
                {
                    query = @"INSERT INTO Loans (BookID, MemberID, LoanDate, DueDate, ReturnDate, Status, Notes)
                             VALUES (@BookID, @MemberID, @LoanDate, @DueDate, @ReturnDate, @Status, @Notes)";

                    parameters = new System.Data.SqlClient.SqlParameter[] {
                        new System.Data.SqlClient.SqlParameter("@BookID", Convert.ToInt32(cboBook.SelectedValue)),
                        new System.Data.SqlClient.SqlParameter("@MemberID", Convert.ToInt32(cboMember.SelectedValue)),
                        new System.Data.SqlClient.SqlParameter("@LoanDate", dtpLoanDate.Value.Date),
                        new System.Data.SqlClient.SqlParameter("@DueDate", dtpDueDate.Value.Date),
                        new System.Data.SqlClient.SqlParameter("@ReturnDate", chkReturned.Checked ? (object)dtpReturnDate.Value.Date : DBNull.Value),
                        new System.Data.SqlClient.SqlParameter("@Status", cboStatus.SelectedItem.ToString()),
                        new System.Data.SqlClient.SqlParameter("@Notes", txtNotes.Text.Trim())
                    };
                }

                int result = LibraryManagement.Helpers.DatabaseHelper.ExecuteNonQuery(query, parameters);
                
                if (result > 0)
                {
                    MessageBox.Show(isEditMode ? "Cập nhật phiếu mượn thành công!" : "Thêm phiếu mượn thành công!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Không thể lưu dữ liệu!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            // This method is required for the designer support
            // Actual initialization is done in SetupUI()
        }
    }
}
