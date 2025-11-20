using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryManagement.Forms
{
    public partial class FormBookDialog : Form
    {
        public string BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int PublishYear { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }

        private TextBox txtTitle, txtAuthor, txtPublisher, txtPublishYear;
        private TextBox txtCategory, txtQuantity, txtISBN, txtDescription;
        private Button btnSave, btnCancel;
        private bool isEditMode;
        private int? editBookID;

        public FormBookDialog(int? bookID = null)
        {
            editBookID = bookID;
            isEditMode = bookID.HasValue;
            
            this.SuspendLayout();
            SetupUI();
            this.ResumeLayout(false);
            
            if (isEditMode)
            {
                LoadBookData(bookID.Value);
            }
        }

        private void FormBookDialog_Load(object sender, EventArgs e)
        {

        }

        private void SetupUI()
        {
            this.Text = isEditMode ? "Sửa thông tin sách" : "Thêm sách mới";
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
                BackColor = Color.FromArgb(52, 152, 219)
            };

            Label lblHeader = new Label
            {
                Text = isEditMode ? "✏️ SỬA THÔNG TIN SÁCH" : "➕ THÊM SÁCH MỚI",
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

            CreateLabel("Tên sách: *", 10, y, contentPanel);
            txtTitle = CreateTextBox(10, y + 25, contentPanel);

            y += spacing;
            CreateLabel("Tác giả:", 10, y, contentPanel);
            txtAuthor = CreateTextBox(10, y + 25, contentPanel);

            y += spacing;
            CreateLabel("Nhà xuất bản:", 10, y, contentPanel);
            txtPublisher = CreateTextBox(10, y + 25, contentPanel);

            y += spacing;
            CreateLabel("Năm xuất bản:", 10, y, contentPanel);
            txtPublishYear = CreateTextBox(10, y + 25, contentPanel, 200);

            y += spacing;
            CreateLabel("Thể loại:", 10, y, contentPanel);
            txtCategory = CreateTextBox(10, y + 25, contentPanel);

            y += spacing;
            CreateLabel("Số lượng:", 10, y, contentPanel);
            txtQuantity = CreateTextBox(10, y + 25, contentPanel, 200);

            y += spacing;
            CreateLabel("ISBN:", 10, y, contentPanel);
            txtISBN = CreateTextBox(10, y + 25, contentPanel);

            y += spacing;
            CreateLabel("Mô tả:", 10, y, contentPanel);
            txtDescription = new TextBox
            {
                Location = new Point(10, y + 25),
                Size = new Size(410, 80),
                Multiline = true,
                Font = new Font("Segoe UI", 10),
                ScrollBars = ScrollBars.Vertical
            };
            contentPanel.Controls.Add(txtDescription);

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

        private TextBox CreateTextBox(int x, int y, Control parent, int width = 410)
        {
            TextBox txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 30),
                Font = new Font("Segoe UI", 10)
            };
            parent.Controls.Add(txt);
            return txt;
        }

        private void LoadBookData(int bookID)
        {
            try
            {
                string query = "SELECT * FROM Books WHERE BookID = @BookID";
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new System.Data.SqlClient.SqlParameter("@BookID", bookID)
                };
                
                System.Data.DataTable dt = LibraryManagement.Helpers.DatabaseHelper.ExecuteQuery(query, parameters);
                
                if (dt.Rows.Count > 0)
                {
                    System.Data.DataRow row = dt.Rows[0];
                    BookID = row["BookID"].ToString();
                    txtTitle.Text = row["Title"].ToString();
                    txtAuthor.Text = row["Author"].ToString();
                    txtPublisher.Text = row["Publisher"].ToString();
                    txtPublishYear.Text = row["PublishYear"].ToString();
                    txtCategory.Text = row["Category"].ToString();
                    txtQuantity.Text = row["Quantity"].ToString();
                    txtISBN.Text = row["ISBN"].ToString();
                    txtDescription.Text = row["Description"].ToString();
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
            txtTitle.Text = Title;
            txtAuthor.Text = Author;
            txtPublisher.Text = Publisher;
            txtPublishYear.Text = PublishYear > 0 ? PublishYear.ToString() : "";
            txtCategory.Text = Category;
            txtQuantity.Text = Quantity > 0 ? Quantity.ToString() : "";
            txtISBN.Text = ISBN;
            txtDescription.Text = Description;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            try
            {
                string query;
                System.Data.SqlClient.SqlParameter[] parameters;

                if (isEditMode)
                {
                    query = @"UPDATE Books SET Title = @Title, Author = @Author, Publisher = @Publisher,
                             PublishYear = @PublishYear, Category = @Category, Quantity = @Quantity,
                             ISBN = @ISBN, Description = @Description WHERE BookID = @BookID";

                    parameters = new System.Data.SqlClient.SqlParameter[] {
                        new System.Data.SqlClient.SqlParameter("@BookID", editBookID.Value),
                        new System.Data.SqlClient.SqlParameter("@Title", txtTitle.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Author", txtAuthor.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Publisher", txtPublisher.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@PublishYear", int.TryParse(txtPublishYear.Text, out int year) ? year : 0),
                        new System.Data.SqlClient.SqlParameter("@Category", txtCategory.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Quantity", int.TryParse(txtQuantity.Text, out int qty) ? qty : 0),
                        new System.Data.SqlClient.SqlParameter("@ISBN", txtISBN.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Description", txtDescription.Text.Trim())
                    };
                }
                else
                {
                    query = @"INSERT INTO Books (Title, Author, Publisher, PublishYear, Category, Quantity, ISBN, Description)
                             VALUES (@Title, @Author, @Publisher, @PublishYear, @Category, @Quantity, @ISBN, @Description)";

                    parameters = new System.Data.SqlClient.SqlParameter[] {
                        new System.Data.SqlClient.SqlParameter("@Title", txtTitle.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Author", txtAuthor.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Publisher", txtPublisher.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@PublishYear", int.TryParse(txtPublishYear.Text, out int year) ? year : 0),
                        new System.Data.SqlClient.SqlParameter("@Category", txtCategory.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Quantity", int.TryParse(txtQuantity.Text, out int qty) ? qty : 0),
                        new System.Data.SqlClient.SqlParameter("@ISBN", txtISBN.Text.Trim()),
                        new System.Data.SqlClient.SqlParameter("@Description", txtDescription.Text.Trim())
                    };
                }

                int result = LibraryManagement.Helpers.DatabaseHelper.ExecuteNonQuery(query, parameters);
                
                if (result > 0)
                {
                    MessageBox.Show(isEditMode ? "Cập nhật sách thành công!" : "Thêm sách thành công!", 
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
            this.SuspendLayout();
            // 
            // FormBookDialog
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "FormBookDialog";
            this.Load += new System.EventHandler(this.FormBookDialog_Load);
            this.ResumeLayout(false);

        }
    }
}
