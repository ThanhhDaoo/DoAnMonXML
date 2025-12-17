using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms
{
    public partial class FormBooksModern : Form
    {
        private DataGridView dgvBooks;
        private TextBox txtSearch;
        private Panel detailsPanel;
        private TextBox txtBookID, txtTitle, txtAuthor, txtPublisher, txtPublishYear;
        private TextBox txtCategory, txtQuantity, txtISBN, txtDescription;

        public FormBooksModern()
        {
            InitializeComponent();
            SetupModernUI();
            LoadData();
        }

        private void SetupModernUI()
        {
            this.Text = "Quản Lý Sách";
            this.Size = new Size(1600, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = ModernUIHelper.Colors.Light;

            // Header
            Panel header = ModernUIHelper.CreateGradientHeader(
                "📚 QUẢN LÝ SÁCH",
                "Quản lý thông tin sách trong thư viện",
                ModernUIHelper.Colors.Primary,
                ModernUIHelper.Colors.PrimaryDark,
                100
            );

            // Toolbar
            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 20),
                AutoScroll = true
            };

            Button btnBack = ModernUIHelper.CreateIconButton("◀", "Quay lại", ModernUIHelper.Colors.Gray, 120);
            btnBack.Location = new Point(0, 20);
            btnBack.Click += (s, e) => this.Close();

            Panel searchBox = ModernUIHelper.CreateSearchBox("Tìm kiếm sách theo tên, tác giả, thể loại...", 350);
            searchBox.Location = new Point(130, 20);
            txtSearch = (TextBox)searchBox.Tag;
            txtSearch.TextChanged += (s, e) =>
            {
                if (txtSearch.Text != "Tìm kiếm sách theo tên, tác giả, thể loại...")
                    SearchBooks();
            };

            Button btnAdd = ModernUIHelper.CreateIconButton("➕", "Thêm", ModernUIHelper.Colors.Success, 110);
            btnAdd.Location = new Point(490, 20);
            btnAdd.Click += BtnAdd_Click;

            Button btnEdit = ModernUIHelper.CreateIconButton("✏️", "Sửa", ModernUIHelper.Colors.Primary, 110);
            btnEdit.Location = new Point(610, 20);
            btnEdit.Click += BtnEdit_Click;

            Button btnDelete = ModernUIHelper.CreateIconButton("🗑️", "Xóa", ModernUIHelper.Colors.Danger, 110);
            btnDelete.Location = new Point(730, 20);
            btnDelete.Click += BtnDelete_Click;

            Button btnRefresh = ModernUIHelper.CreateIconButton("🔄", "Làm mới", ModernUIHelper.Colors.Gray, 120);
            btnRefresh.Location = new Point(850, 20);
            btnRefresh.Click += (s, e) => LoadData();

            Button btnExport = ModernUIHelper.CreateIconButton("📤", "Export", ModernUIHelper.Colors.Warning, 120);
            btnExport.Location = new Point(980, 20);
            btnExport.Click += BtnExport_Click;

            Button btnImport = ModernUIHelper.CreateIconButton("📥", "Import", ModernUIHelper.Colors.Info, 120);
            btnImport.Location = new Point(1110, 20);
            btnImport.Click += BtnImport_Click;

            toolbar.Controls.AddRange(new Control[] {
                btnBack, searchBox, btnAdd, btnEdit, btnDelete, btnRefresh, btnExport, btnImport
            });

            // Content area
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 20, 30, 30),
                BackColor = ModernUIHelper.Colors.Light,
                AutoScroll = true
            };

            // DataGridView container - RESPONSIVE WIDTH, FIXED HEIGHT for scrolling
            int detailsPanelWidth = 450;
            int spacing = 20;
            
            Panel dgvContainer = new Panel
            {
                Location = new Point(30, 20),
                Size = new Size(900, 700), // Fixed height for scrolling
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            
            dgvContainer.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(dgvContainer.ClientRectangle, 15))
                {
                    dgvContainer.Region = new Region(path);
                }
            };

            dgvBooks = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(870, 670),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            
            ModernUIHelper.StyleDataGridView(dgvBooks, ModernUIHelper.Colors.Primary);
            dgvBooks.SelectionChanged += DgvBooks_SelectionChanged;

            dgvContainer.Controls.Add(dgvBooks);

            // Details panel - RESPONSIVE WIDTH, FIXED HEIGHT for scrolling
            detailsPanel = new Panel
            {
                Location = new Point(950, 20),
                Size = new Size(detailsPanelWidth, 700),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoScroll = true
            };
            
            // Event resize để điều chỉnh vị trí
            contentPanel.Resize += (s, e) =>
            {
                int availableWidth = contentPanel.Width - 60;
                dgvContainer.Width = availableWidth - detailsPanelWidth - spacing;
                detailsPanel.Left = dgvContainer.Right + spacing;
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
                Text = "📝 THÔNG TIN CHI TIẾT",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, 20),
                Size = new Size(440, 30)
            };

            int yPos = 70;
            int fieldSpacing = 75;

            txtBookID = CreateDetailField("ID Sách:", yPos, true);
            txtTitle = CreateDetailField("Tên sách:", yPos += fieldSpacing);
            txtAuthor = CreateDetailField("Tác giả:", yPos += fieldSpacing);
            txtPublisher = CreateDetailField("Nhà xuất bản:", yPos += fieldSpacing);
            txtPublishYear = CreateDetailField("Năm xuất bản:", yPos += fieldSpacing);
            txtCategory = CreateDetailField("Thể loại:", yPos += fieldSpacing);
            txtQuantity = CreateDetailField("Số lượng:", yPos += fieldSpacing);
            txtISBN = CreateDetailField("ISBN:", yPos += fieldSpacing);

            Label lblDesc = new Label
            {
                Text = "Mô tả:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ModernUIHelper.Colors.Dark,
                Location = new Point(20, yPos += fieldSpacing),
                AutoSize = true
            };

            Panel descPanel = new Panel
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(440, 80),
                BackColor = ModernUIHelper.Colors.Light
            };
            descPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(descPanel.ClientRectangle, 8))
                {
                    descPanel.Region = new Region(path);
                }
            };

            txtDescription = new TextBox
            {
                Location = new Point(10, 10),
                Size = new Size(420, 60),
                Multiline = true,
                BorderStyle = BorderStyle.None,
                BackColor = ModernUIHelper.Colors.Light,
                Font = new Font("Segoe UI", 10)
            };
            descPanel.Controls.Add(txtDescription);

            detailsPanel.Controls.Add(lblDetailsTitle);
            detailsPanel.Controls.Add(lblDesc);
            detailsPanel.Controls.Add(descPanel);

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
                string query = "SELECT * FROM Books ORDER BY BookID";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvBooks.DataSource = dt;

                if (dgvBooks.Columns.Count > 0)
                {
                    dgvBooks.Columns["BookID"].HeaderText = "ID";
                    dgvBooks.Columns["Title"].HeaderText = "Tên sách";
                    dgvBooks.Columns["Author"].HeaderText = "Tác giả";
                    dgvBooks.Columns["Publisher"].HeaderText = "Nhà xuất bản";
                    dgvBooks.Columns["PublishYear"].HeaderText = "Năm XB";
                    dgvBooks.Columns["Category"].HeaderText = "Thể loại";
                    dgvBooks.Columns["Quantity"].HeaderText = "Số lượng";
                    dgvBooks.Columns["ISBN"].HeaderText = "ISBN";
                    dgvBooks.Columns["Description"].HeaderText = "Mô tả";
                }

                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
                MessageBox.Show($"Lỗi mở form thêm sách:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBookID.Text))
            {
                MessageBox.Show("Vui lòng chọn sách cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int bookID = int.Parse(txtBookID.Text);
                using (FormBookDialog dialog = new FormBookDialog(bookID))
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form sửa sách:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormBooksModern_Load(object sender, EventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBookID.Text))
            {
                MessageBox.Show("Vui lòng chọn sách cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Bạn có chắc muốn xóa sách này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                string query = "DELETE FROM Books WHERE BookID = @BookID";
                SqlParameter[] parameters = { new SqlParameter("@BookID", int.Parse(txtBookID.Text)) };

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
                    FileName = $"DanhSachSach_{DateTime.Now:yyyyMMdd_HHmmss}.xml"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    XMLHelper.ExportBooksToXML(dgvBooks, saveDialog.FileName);
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
                        if (XMLHelper.ImportBooksFromXML(openDialog.FileName, check))
                            LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Import:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // 
            // FormBooksModern
            // 
            this.ClientSize = new System.Drawing.Size(1600, 900);
            this.Name = "FormBooksModern";
            this.Load += new System.EventHandler(this.FormBooksModern_Load);
            this.ResumeLayout(false);

        }
    }
}
