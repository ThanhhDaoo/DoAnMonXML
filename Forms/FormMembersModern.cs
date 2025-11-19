using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormMembersModern : Form
    {
        private DataGridView dgvMembers;
        private TextBox txtSearch;
        private Panel detailsPanel;
        private TextBox txtMemberID, txtFullName, txtEmail, txtPhone, txtAddress;
        private DateTimePicker dtpJoinDate;
        private ComboBox cboStatus;

        public FormMembersModern()
        {
            InitializeComponent();
            SetupModernUI();
            LoadData();
        }

        private void SetupModernUI()
        {
            this.Text = "Quản Lý Độc Giả";
            this.Size = new Size(1600, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = ModernUIHelper.Colors.Light;

            // Header
            Panel header = ModernUIHelper.CreateGradientHeader(
                "👥 QUẢN LÝ ĐỘC GIẢ",
                "Quản lý thông tin độc giả thư viện",
                ModernUIHelper.Colors.Success,
                ModernUIHelper.Colors.SuccessDark,
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

            Panel searchBox = ModernUIHelper.CreateSearchBox("Tìm kiếm theo tên, email, số điện thoại...", 400);
            searchBox.Location = new Point(30, 20);
            txtSearch = (TextBox)searchBox.Tag;
            txtSearch.TextChanged += (s, e) =>
            {
                if (txtSearch.Text != "Tìm kiếm theo tên, email, số điện thoại...")
                    SearchMembers();
            };

            Button btnAdd = ModernUIHelper.CreateIconButton("➕", "Thêm", ModernUIHelper.Colors.Success, 120);
            btnAdd.Location = new Point(460, 20);
            btnAdd.Click += BtnAdd_Click;

            Button btnEdit = ModernUIHelper.CreateIconButton("✏️", "Sửa", ModernUIHelper.Colors.Primary, 120);
            btnEdit.Location = new Point(600, 20);
            btnEdit.Click += BtnEdit_Click;

            Button btnDelete = ModernUIHelper.CreateIconButton("🗑️", "Xóa", ModernUIHelper.Colors.Danger, 120);
            btnDelete.Location = new Point(740, 20);
            btnDelete.Click += BtnDelete_Click;

            Button btnRefresh = ModernUIHelper.CreateIconButton("🔄", "Làm mới", ModernUIHelper.Colors.Gray, 130);
            btnRefresh.Location = new Point(880, 20);
            btnRefresh.Click += (s, e) => LoadData();

            Button btnExport = ModernUIHelper.CreateIconButton("📤", "Export", ModernUIHelper.Colors.Warning, 130);
            btnExport.Location = new Point(1030, 20);
            btnExport.Click += BtnExport_Click;

            Button btnImport = ModernUIHelper.CreateIconButton("📥", "Import", ModernUIHelper.Colors.Info, 130);
            btnImport.Location = new Point(1180, 20);
            btnImport.Click += BtnImport_Click;

            toolbar.Controls.AddRange(new Control[] {
                searchBox, btnAdd, btnEdit, btnDelete, btnRefresh, btnExport, btnImport
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
                Size = new Size(1000, 700),
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

            dgvMembers = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(970, 670),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            ModernUIHelper.StyleDataGridView(dgvMembers, ModernUIHelper.Colors.Success);
            dgvMembers.SelectionChanged += DgvMembers_SelectionChanged;

            dgvContainer.Controls.Add(dgvMembers);

            // Details panel
            detailsPanel = new Panel
            {
                Location = new Point(1050, 20),
                Size = new Size(480, 700),
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
                Text = "👤 THÔNG TIN ĐỘC GIẢ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, 20),
                Size = new Size(440, 30)
            };

            int yPos = 70;
            int spacing = 75;

            txtMemberID = CreateDetailField("ID Độc giả:", yPos, true);
            txtFullName = CreateDetailField("Họ và tên:", yPos += spacing);
            txtEmail = CreateDetailField("Email:", yPos += spacing);
            txtPhone = CreateDetailField("Số điện thoại:", yPos += spacing);

            Label lblAddress = new Label
            {
                Text = "Địa chỉ:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, yPos += spacing),
                AutoSize = true
            };

            Panel addressPanel = new Panel
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(440, 80),
                BackColor = ModernUIHelper.Colors.Light
            };
            addressPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(addressPanel.ClientRectangle, 8))
                {
                    addressPanel.Region = new Region(path);
                }
            };

            txtAddress = new TextBox
            {
                Location = new Point(10, 10),
                Size = new Size(420, 60),
                Multiline = true,
                BorderStyle = BorderStyle.None,
                BackColor = ModernUIHelper.Colors.Light,
                Font = new Font("Segoe UI", 10)
            };
            addressPanel.Controls.Add(txtAddress);

            Label lblJoinDate = new Label
            {
                Text = "Ngày tham gia:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, yPos += 120),
                AutoSize = true
            };

            Panel dtpPanel = new Panel
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(440, 40),
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

            dtpJoinDate = new DateTimePicker
            {
                Location = new Point(12, 8),
                Size = new Size(416, 25),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };
            dtpPanel.Controls.Add(dtpJoinDate);

            Label lblStatus = new Label
            {
                Text = "Trạng thái:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, yPos += 75),
                AutoSize = true
            };

            Panel cboPanel = new Panel
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(440, 40),
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
                Location = new Point(12, 8),
                Size = new Size(416, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cboStatus.Items.AddRange(new string[] { "Active", "Inactive", "Suspended" });
            cboStatus.SelectedIndex = 0;
            cboPanel.Controls.Add(cboStatus);

            detailsPanel.Controls.Add(lblDetailsTitle);
            detailsPanel.Controls.Add(lblAddress);
            detailsPanel.Controls.Add(addressPanel);
            detailsPanel.Controls.Add(lblJoinDate);
            detailsPanel.Controls.Add(dtpPanel);
            detailsPanel.Controls.Add(lblStatus);
            detailsPanel.Controls.Add(cboPanel);

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
                Size = new Size(440, 40),
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
                Size = new Size(416, 25),
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

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM Members ORDER BY MemberID";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvMembers.DataSource = dt;

                if (dgvMembers.Columns.Count > 0)
                {
                    dgvMembers.Columns["MemberID"].HeaderText = "ID";
                    dgvMembers.Columns["FullName"].HeaderText = "Họ tên";
                    dgvMembers.Columns["Email"].HeaderText = "Email";
                    dgvMembers.Columns["Phone"].HeaderText = "Điện thoại";
                    dgvMembers.Columns["Address"].HeaderText = "Địa chỉ";
                    dgvMembers.Columns["JoinDate"].HeaderText = "Ngày tham gia";
                    dgvMembers.Columns["Status"].HeaderText = "Trạng thái";
                }

                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvMembers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMembers.CurrentRow != null)
            {
                DataGridViewRow row = dgvMembers.CurrentRow;
                txtMemberID.Text = row.Cells["MemberID"].Value?.ToString();
                txtFullName.Text = row.Cells["FullName"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtPhone.Text = row.Cells["Phone"].Value?.ToString();
                txtAddress.Text = row.Cells["Address"].Value?.ToString();

                if (row.Cells["JoinDate"].Value != null && row.Cells["JoinDate"].Value != DBNull.Value)
                {
                    dtpJoinDate.Value = Convert.ToDateTime(row.Cells["JoinDate"].Value);
                }

                string status = row.Cells["Status"].Value?.ToString() ?? "Active";
                cboStatus.SelectedItem = status;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"INSERT INTO Members (FullName, Email, Phone, Address, JoinDate, Status)
                           VALUES (@FullName, @Email, @Phone, @Address, @JoinDate, @Status)";

            SqlParameter[] parameters = {
                new SqlParameter("@FullName", txtFullName.Text),
                new SqlParameter("@Email", txtEmail.Text),
                new SqlParameter("@Phone", txtPhone.Text),
                new SqlParameter("@Address", txtAddress.Text),
                new SqlParameter("@JoinDate", dtpJoinDate.Value.Date),
                new SqlParameter("@Status", cboStatus.SelectedItem.ToString())
            };

            if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Thêm độc giả thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMemberID.Text))
            {
                MessageBox.Show("Vui lòng chọn độc giả cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"UPDATE Members SET FullName = @FullName, Email = @Email, 
                           Phone = @Phone, Address = @Address, JoinDate = @JoinDate, Status = @Status
                           WHERE MemberID = @MemberID";

            SqlParameter[] parameters = {
                new SqlParameter("@MemberID", int.Parse(txtMemberID.Text)),
                new SqlParameter("@FullName", txtFullName.Text),
                new SqlParameter("@Email", txtEmail.Text),
                new SqlParameter("@Phone", txtPhone.Text),
                new SqlParameter("@Address", txtAddress.Text),
                new SqlParameter("@JoinDate", dtpJoinDate.Value.Date),
                new SqlParameter("@Status", cboStatus.SelectedItem.ToString())
            };

            if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Cập nhật thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMemberID.Text))
            {
                MessageBox.Show("Vui lòng chọn độc giả cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Bạn có chắc muốn xóa độc giả này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                string query = "DELETE FROM Members WHERE MemberID = @MemberID";
                SqlParameter[] parameters = { new SqlParameter("@MemberID", int.Parse(txtMemberID.Text)) };

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
                    FileName = $"DanhSachDocGia_{DateTime.Now:yyyyMMdd_HHmmss}.xml"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    XMLHelperMembers.ExportMembersToXML(dgvMembers, saveDialog.FileName);
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
                    DialogResult checkDuplicate = MessageBox.Show(
                        "Kiểm tra trùng lặp?\n\nYES: Bỏ qua trùng\nNO: Thêm tất cả",
                        "Kiểm tra trùng lặp", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (checkDuplicate != DialogResult.Cancel)
                    {
                        bool check = (checkDuplicate == DialogResult.Yes);
                        if (XMLHelperMembers.ImportMembersFromXML(openDialog.FileName, check))
                            LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Import:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchMembers()
        {
            if (dgvMembers.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = string.Format(
                    "FullName LIKE '%{0}%' OR Email LIKE '%{0}%' OR Phone LIKE '%{0}%'",
                    txtSearch.Text);
            }
        }

        private void ClearInputs()
        {
            txtMemberID.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            dtpJoinDate.Value = DateTime.Now;
            cboStatus.SelectedIndex = 0;
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
            this.ClientSize = new System.Drawing.Size(1600, 900);
            this.Name = "FormMembersModern";
            this.ResumeLayout(false);
        }
    }
}
