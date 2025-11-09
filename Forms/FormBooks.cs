using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormBooks : Form
    {
        private DataGridView dgvBooks;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private Button btnExport, btnImport; // Nút Import/Export XML
        private GroupBox grpInfo;
        private TextBox txtBookID, txtTitle, txtAuthor, txtPublisher, txtPublishYear;
        private TextBox txtCategory, txtQuantity, txtISBN, txtDescription;

        public FormBooks()
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
            this.Text = "Quản Lý Sách";
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
            txtSearch.TextChanged += (s, e) => SearchBooks();

            btnAdd = CreateButton("Thêm", 360, Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;

            btnEdit = CreateButton("Sửa", 450, Color.FromArgb(52, 152, 219));
            btnEdit.Click += BtnEdit_Click;

            btnDelete = CreateButton("Xóa", 540, Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = CreateButton("Làm mới", 630, Color.FromArgb(149, 165, 166));
            btnRefresh.Click += (s, e) => LoadData();

            // NÚT EXPORT XML
            btnExport = CreateButton("📤 Export XML", 750, Color.FromArgb(230, 126, 34));
            btnExport.Click += BtnExport_Click;

            // NÚT IMPORT XML
            btnImport = CreateButton("📥 Import XML", 880, Color.FromArgb(155, 89, 182));
            btnImport.Click += BtnImport_Click;

            topPanel.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnAdd, btnEdit, btnDelete,
                btnRefresh, btnExport, btnImport
            });

            // === PANEL GIỮA: DataGridView ===
            dgvBooks = new DataGridView
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
            dgvBooks.SelectionChanged += DgvBooks_SelectionChanged;

            // === PANEL PHẢI: Thông tin chi tiết ===
            grpInfo = new GroupBox
            {
                Text = "Thông tin sách",
                Location = new Point(800, 70),
                Size = new Size(380, 570),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.White
            };

            int lblX = 15, txtX = 120, startY = 30, spacing = 45;

            CreateLabel("ID Sách:", lblX, startY, grpInfo);
            txtBookID = CreateTextBox(txtX, startY, grpInfo, true);

            CreateLabel("Tên sách:", lblX, startY + spacing, grpInfo);
            txtTitle = CreateTextBox(txtX, startY + spacing, grpInfo);

            CreateLabel("Tác giả:", lblX, startY + spacing * 2, grpInfo);
            txtAuthor = CreateTextBox(txtX, startY + spacing * 2, grpInfo);

            CreateLabel("Nhà XB:", lblX, startY + spacing * 3, grpInfo);
            txtPublisher = CreateTextBox(txtX, startY + spacing * 3, grpInfo);

            CreateLabel("Năm XB:", lblX, startY + spacing * 4, grpInfo);
            txtPublishYear = CreateTextBox(txtX, startY + spacing * 4, grpInfo);

            CreateLabel("Thể loại:", lblX, startY + spacing * 5, grpInfo);
            txtCategory = CreateTextBox(txtX, startY + spacing * 5, grpInfo);

            CreateLabel("Số lượng:", lblX, startY + spacing * 6, grpInfo);
            txtQuantity = CreateTextBox(txtX, startY + spacing * 6, grpInfo);

            CreateLabel("ISBN:", lblX, startY + spacing * 7, grpInfo);
            txtISBN = CreateTextBox(txtX, startY + spacing * 7, grpInfo);

            CreateLabel("Mô tả:", lblX, startY + spacing * 8, grpInfo);
            txtDescription = new TextBox
            {
                Location = new Point(txtX, startY + spacing * 8),
                Size = new Size(240, 60),
                Multiline = true,
                Font = new Font("Segoe UI", 9)
            };
            grpInfo.Controls.Add(txtDescription);

            // Thêm controls vào form
            this.Controls.AddRange(new Control[] { topPanel, dgvBooks, grpInfo });
        }

        /// <summary>
        /// Tạo Button với style đẹp
        /// </summary>
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

        private void FormBooks_Load(object sender, EventArgs e)
        {

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

        // ========================================
        // PHẦN 1: TẢI VÀ HIỂN THỊ DỮ LIỆU (SQL -> WINFORM)
        // ========================================

        /// <summary>
        /// Tải dữ liệu từ SQL Server lên DataGridView
        /// </summary>
        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM Books ORDER BY BookID";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvBooks.DataSource = dt;

                // Đặt tiêu đề cột
                if (dgvBooks.Columns.Count > 0)
                {
                    dgvBooks.Columns["BookID"].HeaderText = "ID";
                    dgvBooks.Columns["Title"].HeaderText = "Tên sách";
                    dgvBooks.Columns["Author"].HeaderText = "Tác giả";
                    dgvBooks.Columns["Publisher"].HeaderText = "Nhà xuất bản";
                    dgvBooks.Columns["PublishYear"].HeaderText = "Năm XB";
                    dgvBooks.Columns["Category"].HeaderText = "Thể loại";
                    dgvBooks.Columns["Quantity"].HeaderText = "SL";
                    dgvBooks.Columns["ISBN"].HeaderText = "ISBN";
                    dgvBooks.Columns["Description"].HeaderText = "Mô tả";
                }

                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hiển thị thông tin sách được chọn
        /// </summary>
        private void DgvBooks_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBooks.CurrentRow != null)
            {
                DataGridViewRow row = dgvBooks.CurrentRow;
                txtBookID.Text = row.Cells["BookID"].Value?.ToString();
                txtTitle.Text = row.Cells["Title"].Value?.ToString();
                txtAuthor.Text = row.Cells["Author"].Value?.ToString();
                txtPublisher.Text = row.Cells["Publisher"].Value?.ToString();
                txtPublishYear.Text = row.Cells["PublishYear"].Value?.ToString();
                txtCategory.Text = row.Cells["Category"].Value?.ToString();
                txtQuantity.Text = row.Cells["Quantity"].Value?.ToString();
                txtISBN.Text = row.Cells["ISBN"].Value?.ToString();
                txtDescription.Text = row.Cells["Description"].Value?.ToString();
            }
        }

        // ========================================
        // PHẦN 2: CHỨC NĂNG EXPORT (SQL/DATAGRIDVIEW -> XML)
        // ========================================

        /// <summary>
        /// Xử lý nút Export XML
        /// </summary>
        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo SaveFileDialog để chọn nơi lưu file
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml",
                    Title = "Export dữ liệu sách ra XML",
                    FileName = $"DanhSachSach_{DateTime.Now:yyyyMMdd_HHmmss}.xml"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Export từ DataGridView
                    bool success = XMLHelper.ExportBooksToXML(dgvBooks, saveDialog.FileName);

                    if (success)
                    {
                        // Hỏi có muốn mở file không
                        DialogResult result = MessageBox.Show(
                            "Bạn có muốn mở file XML vừa xuất?",
                            "Thành công",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Export XML:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================================
        // PHẦN 3: CHỨC NĂNG IMPORT (XML -> SQL SERVER)
        // ========================================

        /// <summary>
        /// Xử lý nút Import XML
        /// </summary>
        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo OpenFileDialog để chọn file XML
                OpenFileDialog openDialog = new OpenFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml",
                    Title = "Chọn file XML để Import",
                    Multiselect = false
                };

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    // Hỏi có kiểm tra trùng lặp không
                    DialogResult checkDuplicate = MessageBox.Show(
                        "Bạn có muốn kiểm tra và bỏ qua các sách đã tồn tại trong CSDL không?\n\n" +
                        "- YES: Bỏ qua sách trùng (an toàn)\n" +
                        "- NO: Thêm tất cả (có thể trùng)",
                        "Kiểm tra trùng lặp",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (checkDuplicate == DialogResult.Cancel)
                        return;

                    bool check = (checkDuplicate == DialogResult.Yes);

                    // Import từ XML vào Database
                    bool success = XMLHelper.ImportBooksFromXML(openDialog.FileName, check);

                    if (success)
                    {
                        // Refresh lại dữ liệu
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Import XML:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================================
        // CÁC CHỨC NĂNG CRUD CƠ BẢN
        // ========================================

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"INSERT INTO Books (Title, Author, Publisher, PublishYear, Category, Quantity, ISBN, Description)
                           VALUES (@Title, @Author, @Publisher, @PublishYear, @Category, @Quantity, @ISBN, @Description)";

            SqlParameter[] parameters = {
                new SqlParameter("@Title", txtTitle.Text),
                new SqlParameter("@Author", txtAuthor.Text),
                new SqlParameter("@Publisher", txtPublisher.Text),
                new SqlParameter("@PublishYear", int.TryParse(txtPublishYear.Text, out int year) ? year : 0),
                new SqlParameter("@Category", txtCategory.Text),
                new SqlParameter("@Quantity", int.TryParse(txtQuantity.Text, out int qty) ? qty : 0),
                new SqlParameter("@ISBN", txtISBN.Text),
                new SqlParameter("@Description", txtDescription.Text)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                MessageBox.Show("Thêm sách thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBookID.Text))
            {
                MessageBox.Show("Vui lòng chọn sách cần sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"UPDATE Books SET Title = @Title, Author = @Author, Publisher = @Publisher,
                           PublishYear = @PublishYear, Category = @Category, Quantity = @Quantity,
                           ISBN = @ISBN, Description = @Description WHERE BookID = @BookID";

            SqlParameter[] parameters = {
                new SqlParameter("@BookID", int.Parse(txtBookID.Text)),
                new SqlParameter("@Title", txtTitle.Text),
                new SqlParameter("@Author", txtAuthor.Text),
                new SqlParameter("@Publisher", txtPublisher.Text),
                new SqlParameter("@PublishYear", int.TryParse(txtPublishYear.Text, out int year) ? year : 0),
                new SqlParameter("@Category", txtCategory.Text),
                new SqlParameter("@Quantity", int.TryParse(txtQuantity.Text, out int qty) ? qty : 0),
                new SqlParameter("@ISBN", txtISBN.Text),
                new SqlParameter("@Description", txtDescription.Text)
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
            if (string.IsNullOrWhiteSpace(txtBookID.Text))
            {
                MessageBox.Show("Vui lòng chọn sách cần xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Bạn có chắc muốn xóa sách này?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                string query = "DELETE FROM Books WHERE BookID = @BookID";
                SqlParameter[] parameters = {
                    new SqlParameter("@BookID", int.Parse(txtBookID.Text))
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

        private void SearchBooks()
        {
            if (dgvBooks.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = string.Format(
                    "Title LIKE '%{0}%' OR Author LIKE '%{0}%' OR Category LIKE '%{0}%'",
                    txtSearch.Text);
            }
        }

        private void ClearInputs()
        {
            txtBookID.Clear();
            txtTitle.Clear();
            txtAuthor.Clear();
            txtPublisher.Clear();
            txtPublishYear.Clear();
            txtCategory.Clear();
            txtQuantity.Clear();
            txtISBN.Clear();
            txtDescription.Clear();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormBooks
            // 
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Name = "FormBooks";
            this.Load += new System.EventHandler(this.FormBooks_Load);
            this.ResumeLayout(false);

        }
    }
}