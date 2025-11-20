using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryManagement.Forms
{
    public partial class FormMemberDialog : Form
    {
        public string MemberID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime JoinDate { get; set; }
        public string Status { get; set; }

        private TextBox txtFullName, txtEmail, txtPhone, txtAddress;
        private DateTimePicker dtpJoinDate;
        private ComboBox cboStatus;
        private Button btnSave, btnCancel;
        private bool isEditMode;
        private int? editMemberID;

        public FormMemberDialog(int? memberID = null)
        {
            editMemberID = memberID;
            isEditMode = memberID.HasValue;
            JoinDate = DateTime.Now;
            Status = "Active";
            
            this.SuspendLayout();
            SetupUI();
            this.ResumeLayout(false);
            
            if (isEditMode)
            {
                LoadMemberData(memberID.Value);
            }
        }

        private void SetupUI()
        {
            this.Text = isEditMode ? "Sửa thông tin độc giả" : "Thêm độc giả mới";
            this.Size = new Size(500, 550);
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
                BackColor = Color.FromArgb(155, 89, 182)
            };

            Label lblHeader = new Label
            {
                Text = isEditMode ? "✏️ SỬA THÔNG TIN ĐỘC GIẢ" : "➕ THÊM ĐỘC GIẢ MỚI",
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
                Size = new Size(440, 380),
                AutoScroll = true
            };

            int y = 10;
            int spacing = 60;

            CreateLabel("Họ và tên: *", 10, y, contentPanel);
            txtFullName = CreateTextBox(10, y + 25, contentPanel);

            y += spacing;
            CreateLabel("Email:", 10, y, contentPanel);
            txtEmail = CreateTextBox(10, y + 25, contentPanel);

            y += spacing;
            CreateLabel("Số điện thoại:", 10, y, contentPanel);
            txtPhone = CreateTextBox(10, y + 25, contentPanel);

            y += spacing;
            CreateLabel("Địa chỉ:", 10, y, contentPanel);
            txtAddress = new TextBox
            {
                Location = new Point(10, y + 25),
                Size = new Size(410, 60),
                Multiline = true,
                Font = new Font("Segoe UI", 10),
                ScrollBars = ScrollBars.Vertical
            };
            contentPanel.Controls.Add(txtAddress);

            y += 90;
            CreateLabel("Ngày tham gia:", 10, y, contentPanel);
            dtpJoinDate = new DateTimePicker
            {
                Location = new Point(10, y + 25),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            contentPanel.Controls.Add(dtpJoinDate);

            y += spacing;
            CreateLabel("Trạng thái:", 10, y, contentPanel);
            cboStatus = new ComboBox
            {
                Location = new Point(10, y + 25),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new string[] { "Active", "Inactive", "Suspended" });
            cboStatus.SelectedIndex = 0;
            contentPanel.Controls.Add(cboStatus);

            // Buttons
            btnSave = new Button
            {
                Text = "💾 Lưu",
                Location = new Point(150, 475),
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
                Location = new Point(280, 475),
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

        private TextBox CreateTextBox(int x, int y, Control parent)
        {
            TextBox txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(410, 30),
                Font = new Font("Segoe UI", 10)
            };
            parent.Controls.Add(txt);
            return txt;
        }

        private void LoadMemberData(int memberID)
        {
            try
            {
                string query = "SELECT * FROM Members WHERE MemberID = @MemberID";
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new System.Data.SqlClient.SqlParameter("@MemberID", memberID)
                };
                
                System.Data.DataTable dt = LibraryManagement.Helpers.DatabaseHelper.ExecuteQuery(query, parameters);
                
                if (dt.Rows.Count > 0)
                {
                    System.Data.DataRow row = dt.Rows[0];
                    MemberID = row["MemberID"].ToString();
                    txtFullName.Text = row["FullName"].ToString();
                    txtEmail.Text = row["Email"].ToString();
                    txtPhone.Text = row["Phone"].ToString();
                    txtAddress.Text = row["Address"].ToString();
                    dtpJoinDate.Value = Convert.ToDateTime(row["JoinDate"]);
                    cboStatus.SelectedItem = row["Status"].ToString();
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
            txtFullName.Text = FullName;
            txtEmail.Text = Email;
            txtPhone.Text = Phone;
            txtAddress.Text = Address;
            dtpJoinDate.Value = JoinDate;
            cboStatus.SelectedItem = Status;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return;
            }

            try
            {
                string query;
                System.Data.SqlClient.SqlParameter[] parameters;

                if (isEditMode)
                {
                    query = @"UPDATE Members SET FullName = @FullName, Email = @Email, 
                             Phone = @Phone, Address = @Address, JoinDate = @JoinDate, Status = @Status
                             WHERE MemberID = @MemberID";

                    parameters = new System.Data.SqlClient.SqlParameter[] {
                        new System.Data.SqlClient.SqlParameter("@MemberID", editMemberID.Value),
                        new System.Data.SqlClient.SqlParameter("@FullName", txtFullName.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Email", txtEmail.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Phone", txtPhone.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Address", txtAddress.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@JoinDate", dtpJoinDate.Value.Date),
                        new System.Data.SqlClient.SqlParameter("@Status", cboStatus.SelectedItem.ToString())
                    };
                }
                else
                {
                    query = @"INSERT INTO Members (FullName, Email, Phone, Address, JoinDate, Status)
                             VALUES (@FullName, @Email, @Phone, @Address, @JoinDate, @Status)";

                    parameters = new System.Data.SqlClient.SqlParameter[] {
                        new System.Data.SqlClient.SqlParameter("@FullName", txtFullName.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Email", txtEmail.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Phone", txtPhone.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Address", txtAddress.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@JoinDate", dtpJoinDate.Value.Date),
                        new System.Data.SqlClient.SqlParameter("@Status", cboStatus.SelectedItem.ToString())
                    };
                }

                int result = LibraryManagement.Helpers.DatabaseHelper.ExecuteNonQuery(query, parameters);
                
                if (result > 0)
                {
                    MessageBox.Show(isEditMode ? "Cập nhật độc giả thành công!" : "Thêm độc giả thành công!", 
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
