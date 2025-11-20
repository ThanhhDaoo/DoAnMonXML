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
            // Cấu hình Form - Responsive
            this.Text = "Quản Lý Sách";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1000, 600);

            // === HEADER PANEL ===
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.White
            };
            headerPanel.Paint += (s, e) =>
            {
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    headerPanel.ClientRectangle,
                    Color.FromArgb(52, 152, 219),
                    Color.FromArgb(41, 128, 185),
                    System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(brush, headerPanel.ClientRectangle);
            };

            Label lblHeaderTitle = new Label
            {
                Text = "📚 QUẢN LÝ SÁCH",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 25),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblHeaderDesc = new Label
            {
                Text = "Quản lý thông tin sách trong thư viện",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(236, 240, 241),
                Location = new Point(30, 60),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            headerPanel.Controls.AddRange(new Control[] { lblHeaderTitle, lblHeaderDesc });

            // === TOOLBAR PANEL ===
            Panel toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            // Search box với icon
            Panel searchPanel = new Panel
            {
                Location = new Point(20, 15),
                Size = new Size(350, 45),
                BackColor = Color.FromArgb(236, 240, 241)
            };
            searchPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, 10, 10, 180, 90);
                path.AddArc(searchPanel.Width - 10, 0, 10, 10, 270, 90);
                path.AddArc(searchPanel.Width - 10, searchPanel.Height - 10, 10, 10, 0, 90);
                path.AddArc(0, searchPanel.Height - 10, 10, 10, 90, 90);
                path.CloseFigure();
                searchPanel.Region = new Region(path);
            };

            Label lblSearchIcon = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI", 14),
                Location = new Point(10, 10),
                Size = new Size(30, 25),
                BackColor = Color.Transparent
            };

            txtSearch = new TextBox
            {
                Location = new Point(45, 12),
                Size = new Size(295, 25),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(236, 240, 241),
                Text = "Tìm kiếm sách..."
            };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Tìm kiếm sách...") txtSearch.Text = ""; };
            txtSearch.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) txtSearch.Text = "Tìm kiếm sách..."; };
            txtSearch.TextChanged += (s, e) => { if (txtSearch.Text != "Tìm kiếm sách...") SearchBooks(); };

            searchPanel.Controls.AddRange(new Control[] { lblSearchIcon, txtSearch });

            btnAdd = CreateModernButton("➕ Thêm", 400, Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;

            btnEdit = CreateModernButton("✏️ Sửa", 520, Color.FromArgb(52, 152, 219));
            btnEdit.Click += BtnEdit_Click;

            btnDelete = CreateModernButton("🗑️ Xóa", 640, Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = CreateModernButton("🔄 Làm mới", 760, Color.FromArgb(149, 165, 166));
            btnRefresh.Click += (s, e) => LoadData();

            btnExport = CreateModernButton("📤 Export", 900, Color.FromArgb(230, 126, 34));
            btnExport.Click += BtnExport_Click;

            btnImport = CreateModernButton("📥 Import", 1020, Color.FromArgb(155, 89, 182));
            btnImport.Click += BtnImport_Click;

            toolbarPanel.Controls.AddRange(new Control[] {
                searchPanel, btnAdd, btnEdit, btnDelete,
                btnRefresh, btnExport, btnImport
            });

            // === CONTENT PANEL ===
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(236, 240, 241)
            };

            // DataGridView với style hiện đại
            dgvBooks = new DataGridView
            {
                Location = new Point(20, 10),
                Size = new Size(900, 650),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 10),
                RowTemplate = { Height = 35 },
                EnableHeadersVisualStyles = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            dgvBooks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvBooks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBooks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvBooks.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvBooks.ColumnHeadersHeight = 40;
            dgvBooks.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvBooks.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgvBooks.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvBooks.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvBooks.GridColor = Color.FromArgb(224, 224, 224);
            dgvBooks.SelectionChanged += DgvBooks_SelectionChanged;

            contentPanel.Controls.Add(dgvBooks);

            // === PANEL PHẢI: Thông tin chi tiết ===
            grpInfo = new GroupBox
            {
                Text = "  📝 Thông tin chi tiết",
                Location = new Point(940, 10),
                Size = new Size(420, 650),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(44, 62, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };
            grpInfo.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, 15, 15, 180, 90);
                path.AddArc(grpInfo.Width - 15, 0, 15, 15, 270, 90);
                path.AddArc(grpInfo.Width - 15, grpInfo.Height - 15, 15, 15, 0, 90);
                path.AddArc(0, grpInfo.Height - 15, 15, 15, 90, 90);
                path.CloseFigure();
                grpInfo.Region = new Region(path);
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

            contentPanel.Controls.Add(grpInfo);

            // Thêm controls vào form
            this.Controls.Add(contentPanel);
            this.Controls.Add(toolbarPanel);
            this.Controls.Add(headerPanel);
        }

        /// <summary>
        /// Tạo Button hiện đại với hiệu ứng
        /// </summary>
        private Button CreateModernButton(string text, int x, Color backColor)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, 15),
                Size = new Size(110, 45),
                BackColor = backColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, 10, 10, 180, 90);
                path.AddArc(btn.Width - 10, 0, 10, 10, 270, 90);
                path.AddArc(btn.Width - 10, btn.Height - 10, 10, 10, 0, 90);
                path.AddArc(0, btn.Height - 10, 10, 10, 90, 90);
                path.CloseFigure();
                btn.Region = new Region(path);
            };

            Color originalColor = backColor;
            Color hoverColor = ControlPaint.Dark(backColor, 0.1f);

            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = originalColor;

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
            Panel txtPanel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(280, 35),
                BackColor = Color.FromArgb(236, 240, 241)
            };
            txtPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, 8, 8, 180, 90);
                path.AddArc(txtPanel.Width - 8, 0, 8, 8, 270, 90);
                path.AddArc(txtPanel.Width - 8, txtPanel.Height - 8, 8, 8, 0, 90);
                path.AddArc(0, txtPanel.Height - 8, 8, 8, 90, 90);
                path.CloseFigure();
                txtPanel.Region = new Region(path);
            };

            TextBox txt = new TextBox
            {
                Location = new Point(8, 7),
                Size = new Size(264, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(236, 240, 241),
                ReadOnly = readOnly
            };

            txtPanel.Controls.Add(txt);
            parent.Controls.Add(txtPanel);
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
            try
            {
                using (FormBookDialog dialog = new FormBookDialog())
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form thêm sách:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            int bookID = int.Parse(txtBookID.Text);
            using (FormBookDialog dialog = new FormBookDialog(bookID))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
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