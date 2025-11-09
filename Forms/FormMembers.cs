using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormMembers : Form
    {
        private DataGridView dgvMembers;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private Button btnExport, btnImport;
        private GroupBox grpInfo;
        private TextBox txtMemberID, txtFullName, txtEmail, txtPhone;
        private TextBox txtAddress;
        private DateTimePicker dtpJoinDate;
        private ComboBox cboStatus;

        public FormMembers()
        {
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        /// <summary>
        /// Thiết lập giao diện form
        /// </summary>
        private void SetupUI()
        {
            // Cấu hình Form
            this.Text = "Quản Lý Độc Giả";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // === PANEL TRÊN: Tìm kiếm và Buttons ===
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
            txtSearch.TextChanged += (s, e) => SearchMembers();

            btnAdd = CreateButton("Thêm", 360, Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;

            btnEdit = CreateButton("Sửa", 450, Color.FromArgb(52, 152, 219));
            btnEdit.Click += BtnEdit_Click;

            btnDelete = CreateButton("Xóa", 540, Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = CreateButton("Làm mới", 630, Color.FromArgb(149, 165, 166));
            btnRefresh.Click += (s, e) => LoadData();

            btnExport = CreateButton("📤 Export", 750, Color.FromArgb(230, 126, 34));
            btnExport.Click += BtnExport_Click;

            btnImport = CreateButton("📥 Import", 850, Color.FromArgb(155, 89, 182));
            btnImport.Click += BtnImport_Click;

            topPanel.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnAdd, btnEdit, btnDelete,
                btnRefresh, btnExport, btnImport
            });

            // === PANEL GIỮA: DataGridView ===
            dgvMembers = new DataGridView
            {
                Location = new Point(10, 70),
                Size = new Size(780, 570),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9)
            };
            dgvMembers.SelectionChanged += DgvMembers_SelectionChanged;

            // === PANEL PHẢI: Thông tin chi tiết ===
            grpInfo = new GroupBox
            {
                Text = "Thông tin độc giả",
                Location = new Point(800, 70),
                Size = new Size(380, 570),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.White
            };

            int lblX = 15, txtX = 120, startY = 30, spacing = 50;

            CreateLabel("ID Độc giả:", lblX, startY, grpInfo);
            txtMemberID = CreateTextBox(txtX, startY, grpInfo, true);

            CreateLabel("Họ tên:", lblX, startY + spacing, grpInfo);
            txtFullName = CreateTextBox(txtX, startY + spacing, grpInfo);

            CreateLabel("Email:", lblX, startY + spacing * 2, grpInfo);
            txtEmail = CreateTextBox(txtX, startY + spacing * 2, grpInfo);

            CreateLabel("Số ĐT:", lblX, startY + spacing * 3, grpInfo);
            txtPhone = CreateTextBox(txtX, startY + spacing * 3, grpInfo);

            CreateLabel("Địa chỉ:", lblX, startY + spacing * 4, grpInfo);
            txtAddress = new TextBox
            {
                Location = new Point(txtX, startY + spacing * 4),
                Size = new Size(240, 60),
                Multiline = true,
                Font = new Font("Segoe UI", 9)
            };
            grpInfo.Controls.Add(txtAddress);

            CreateLabel("Ngày tham gia:", lblX, startY + spacing * 5 + 20, grpInfo);
            dtpJoinDate = new DateTimePicker
            {
                Location = new Point(txtX, startY + spacing * 5 + 20),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short
            };
            grpInfo.Controls.Add(dtpJoinDate);

            CreateLabel("Trạng thái:", lblX, startY + spacing * 6 + 20, grpInfo);
            cboStatus = new ComboBox
            {
                Location = new Point(txtX, startY + spacing * 6 + 20),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new string[] { "Active", "Inactive", "Suspended" });
            cboStatus.SelectedIndex = 0;
            grpInfo.Controls.Add(cboStatus);

            // Thêm controls vào form
            this.Controls.AddRange(new Control[] { topPanel, dgvMembers, grpInfo });
        }

        private Button CreateButton(string text, int x, Color backColor)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, 12),
                Size = new Size(85, 35),
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

        /// <summary>
        /// Tải dữ liệu từ SQL Server
        /// </summary>
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
                MessageBox.Show($"Lỗi tải dữ liệu:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // ========================================
        // CRUD OPERATIONS
        // ========================================

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm độc giả thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private void FormMembers_Load(object sender, EventArgs e)
        {

        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMemberID.Text))
            {
                MessageBox.Show("Vui lòng chọn độc giả cần sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMemberID.Text))
            {
                MessageBox.Show("Vui lòng chọn độc giả cần xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Bạn có chắc muốn xóa độc giả này?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                string query = "DELETE FROM Members WHERE MemberID = @MemberID";
                SqlParameter[] parameters = {
                    new SqlParameter("@MemberID", int.Parse(txtMemberID.Text))
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
        }

        // ========================================
        // XML IMPORT/EXPORT
        // ========================================

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml",
                    Title = "Export danh sách độc giả",
                    FileName = $"DanhSachDocGia_{DateTime.Now:yyyyMMdd_HHmmss}.xml"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    bool success = XMLHelperMembers.ExportMembersToXML(dgvMembers, saveDialog.FileName);

                    if (success)
                    {
                        DialogResult result = MessageBox.Show("Bạn có muốn mở file XML?",
                            "Thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveDialog.FileName);
                        }
                    }
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
                    Filter = "XML Files (*.xml)|*.xml",
                    Title = "Chọn file XML để Import"
                };

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    DialogResult checkDuplicate = MessageBox.Show(
                        "Kiểm tra trùng lặp?\n\nYES: Bỏ qua trùng\nNO: Thêm tất cả",
                        "Kiểm tra trùng lặp",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (checkDuplicate != DialogResult.Cancel)
                    {
                        bool check = (checkDuplicate == DialogResult.Yes);
                        bool success = XMLHelperMembers.ImportMembersFromXML(openDialog.FileName, check);

                        if (success)
                        {
                            LoadData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Import:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormMembers
            // 
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Name = "FormMembers";
            this.Load += new System.EventHandler(this.FormMembers_Load);
            this.ResumeLayout(false);

        }
    }

    // ========================================
    // XML HELPER CHO MEMBERS
    // ========================================
    public class XMLHelperMembers
    {
        public static bool ExportMembersToXML(DataGridView dgv, string filePath)
        {
            try
            {
                System.Xml.Linq.XDocument xmlDoc = new System.Xml.Linq.XDocument(
                    new System.Xml.Linq.XDeclaration("1.0", "utf-8", "yes"),
                    new System.Xml.Linq.XElement("MembersList",
                        new System.Xml.Linq.XAttribute("ExportDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                        new System.Xml.Linq.XAttribute("TotalMembers", dgv.Rows.Count)
                    )
                );

                var root = xmlDoc.Root;

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    var member = new System.Xml.Linq.XElement("Member",
                        new System.Xml.Linq.XElement("MemberID", row.Cells["MemberID"].Value?.ToString() ?? ""),
                        new System.Xml.Linq.XElement("FullName", row.Cells["FullName"].Value?.ToString() ?? ""),
                        new System.Xml.Linq.XElement("Email", row.Cells["Email"].Value?.ToString() ?? ""),
                        new System.Xml.Linq.XElement("Phone", row.Cells["Phone"].Value?.ToString() ?? ""),
                        new System.Xml.Linq.XElement("Address", row.Cells["Address"].Value?.ToString() ?? ""),
                        new System.Xml.Linq.XElement("JoinDate", row.Cells["JoinDate"].Value?.ToString() ?? ""),
                        new System.Xml.Linq.XElement("Status", row.Cells["Status"].Value?.ToString() ?? "Active")
                    );

                    root.Add(member);
                }

                xmlDoc.Save(filePath);
                MessageBox.Show($"Export thành công {dgv.Rows.Count} độc giả!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Export:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool ImportMembersFromXML(string filePath, bool checkDuplicate)
        {
            try
            {
                var xmlDoc = System.Xml.Linq.XDocument.Load(filePath);
                var members = xmlDoc.Root.Elements("Member");

                int total = 0, success = 0, skip = 0, error = 0;

                foreach (var member in members)
                {
                    total++;
                    try
                    {
                        string memberIDStr = member.Element("MemberID")?.Value;

                        if (checkDuplicate && !string.IsNullOrEmpty(memberIDStr))
                        {
                            if (int.TryParse(memberIDStr, out int memberID))
                            {
                                if (DatabaseHelper.IsIDExists("Members", "MemberID", memberID))
                                {
                                    skip++;
                                    continue;
                                }
                            }
                        }

                        string query = @"INSERT INTO Members (FullName, Email, Phone, Address, JoinDate, Status)
                                       VALUES (@FullName, @Email, @Phone, @Address, @JoinDate, @Status)";

                        SqlParameter[] parameters = {
                            new SqlParameter("@FullName", member.Element("FullName")?.Value ?? ""),
                            new SqlParameter("@Email", member.Element("Email")?.Value ?? ""),
                            new SqlParameter("@Phone", member.Element("Phone")?.Value ?? ""),
                            new SqlParameter("@Address", member.Element("Address")?.Value ?? ""),
                            new SqlParameter("@JoinDate", DateTime.TryParse(member.Element("JoinDate")?.Value, out DateTime jd) ? jd : DateTime.Now),
                            new SqlParameter("@Status", member.Element("Status")?.Value ?? "Active")
                        };

                        if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                            success++;
                        else
                            error++;
                    }
                    catch
                    {
                        error++;
                    }
                }

                MessageBox.Show($"Kết quả Import:\n\nTổng: {total}\nThành công: {success}\nBỏ qua: {skip}\nLỗi: {error}",
                    "Hoàn thành", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return success > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Import:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}