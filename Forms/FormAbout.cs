using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryManagement.Forms
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            // Cấu hình Form
            this.Text = "Về Chúng Tôi";
            this.Size = new Size(700, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Main Panel
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.White;

            // Logo/Icon Panel
            Panel logoPanel = new Panel();
            logoPanel.Location = new Point(0, 0);
            logoPanel.Size = new Size(700, 150);
            logoPanel.BackColor = Color.FromArgb(41, 128, 185);

            Label lblIcon = new Label();
            lblIcon.Text = "📚";
            lblIcon.Font = new Font("Segoe UI", 48);
            lblIcon.ForeColor = Color.White;
            lblIcon.Location = new Point(310, 20);
            lblIcon.AutoSize = true;

            Label lblSystemName = new Label();
            lblSystemName.Text = "HỆ THỐNG QUẢN LÝ THƯ VIỆN";
            lblSystemName.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblSystemName.ForeColor = Color.White;
            lblSystemName.Location = new Point(160, 95);
            lblSystemName.AutoSize = true;

            logoPanel.Controls.Add(lblIcon);
            logoPanel.Controls.Add(lblSystemName);

            // Content Panel
            Panel contentPanel = new Panel();
            contentPanel.Location = new Point(50, 170);
            contentPanel.Size = new Size(600, 380);
            contentPanel.BackColor = Color.White;

            int y = 0;
            int spacing = 50;

            // Version
            Label lblVersion = CreateInfoLabel("📦 Phiên bản: 1.0.0", y, true);
            y += spacing;

            // Release Date
            Label lblRelease = CreateInfoLabel("📅 Ngày phát hành: 09/11/2024", y);
            y += spacing;

            // Technology
            Label lblTech = CreateInfoLabel("💻 Công nghệ:", y, true);
            y += 30;
            Label lblTechDetail = CreateInfoLabel("   • Ngôn ngữ: C# (.NET Framework 4.7.2)", y);
            y += 25;
            Label lblTechDetail2 = CreateInfoLabel("   • Giao diện: Windows Forms", y);
            y += 25;
            Label lblTechDetail3 = CreateInfoLabel("   • Cơ sở dữ liệu: SQL Server", y);
            y += 25;
            Label lblTechDetail4 = CreateInfoLabel("   • Định dạng dữ liệu: XML", y);
            y += spacing;

            // Features
            Label lblFeatures = CreateInfoLabel("✨ Tính năng chính:", y, true);
            y += 30;
            Label lblFeature1 = CreateInfoLabel("   • Quản lý sách, độc giả và phiếu mượn/trả", y);
            y += 25;
            Label lblFeature2 = CreateInfoLabel("   • Import/Export dữ liệu với XML", y);
            y += 25;
            Label lblFeature3 = CreateInfoLabel("   • Báo cáo và thống kê chi tiết", y);
            y += 25;
            Label lblFeature4 = CreateInfoLabel("   • Giao diện thân thiện và hiện đại", y);
            y += 50;

            // Developer
            Label lblDev = CreateInfoLabel("👨‍💻 Phát triển bởi:", y, true);
            y += 30;
            Label lblDevName = CreateInfoLabel("   Sinh viên Khoa Công Nghệ Thông Tin", y);
            y += 25;
            Label lblDevSchool = CreateInfoLabel("   Trường Đại Học [Tên Trường]", y);
            y += 25;
            Label lblDevProject = CreateInfoLabel("   Đồ án môn học: Lập trình XML", y);
            y += 50;

            // Copyright
            Label lblCopyright = CreateInfoLabel("© 2024 Library Management System. All rights reserved.", y);
            lblCopyright.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblCopyright.ForeColor = Color.Gray;

            contentPanel.Controls.Add(lblVersion);
            contentPanel.Controls.Add(lblRelease);
            contentPanel.Controls.Add(lblTech);
            contentPanel.Controls.Add(lblTechDetail);
            contentPanel.Controls.Add(lblTechDetail2);
            contentPanel.Controls.Add(lblTechDetail3);
            contentPanel.Controls.Add(lblTechDetail4);
            contentPanel.Controls.Add(lblFeatures);
            contentPanel.Controls.Add(lblFeature1);
            contentPanel.Controls.Add(lblFeature2);
            contentPanel.Controls.Add(lblFeature3);
            contentPanel.Controls.Add(lblFeature4);
            contentPanel.Controls.Add(lblDev);
            contentPanel.Controls.Add(lblDevName);
            contentPanel.Controls.Add(lblDevSchool);
            contentPanel.Controls.Add(lblDevProject);
            contentPanel.Controls.Add(lblCopyright);

            // Button Panel
            Panel btnPanel = new Panel();
            btnPanel.Dock = DockStyle.Bottom;
            btnPanel.Height = 70;
            btnPanel.BackColor = Color.FromArgb(245, 245, 245);

            Button btnWebsite = new Button();
            btnWebsite.Text = "🌐 Website";
            btnWebsite.Location = new Point(180, 18);
            btnWebsite.Size = new Size(120, 35);
            btnWebsite.BackColor = Color.FromArgb(52, 152, 219);
            btnWebsite.ForeColor = Color.White;
            btnWebsite.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnWebsite.FlatStyle = FlatStyle.Flat;
            btnWebsite.FlatAppearance.BorderSize = 0;
            btnWebsite.Cursor = Cursors.Hand;
            btnWebsite.Click += (s, e) => MessageBox.Show("Website: https://library-system.com", "Website", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Button btnContact = new Button();
            btnContact.Text = "📧 Liên hệ";
            btnContact.Location = new Point(310, 18);
            btnContact.Size = new Size(120, 35);
            btnContact.BackColor = Color.FromArgb(46, 204, 113);
            btnContact.ForeColor = Color.White;
            btnContact.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnContact.FlatStyle = FlatStyle.Flat;
            btnContact.FlatAppearance.BorderSize = 0;
            btnContact.Cursor = Cursors.Hand;
            btnContact.Click += BtnContact_Click;

            Button btnClose = new Button();
            btnClose.Text = "❌ Đóng";
            btnClose.Location = new Point(440, 18);
            btnClose.Size = new Size(120, 35);
            btnClose.BackColor = Color.FromArgb(231, 76, 60);
            btnClose.ForeColor = Color.White;
            btnClose.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();

            btnPanel.Controls.Add(btnWebsite);
            btnPanel.Controls.Add(btnContact);
            btnPanel.Controls.Add(btnClose);

            mainPanel.Controls.Add(logoPanel);
            mainPanel.Controls.Add(contentPanel);
            mainPanel.Controls.Add(btnPanel);

            this.Controls.Add(mainPanel);
        }

        private Label CreateInfoLabel(string text, int y, bool isBold = false)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Location = new Point(0, y);
            lbl.AutoSize = true;
            lbl.Font = new Font("Segoe UI", 10, isBold ? FontStyle.Bold : FontStyle.Regular);
            lbl.ForeColor = Color.FromArgb(52, 73, 94);
            return lbl;
        }

        private void BtnContact_Click(object sender, EventArgs e)
        {
            string contactInfo = "📧 THÔNG TIN LIÊN HỆ\n\n" +
                "Email: support@library.com\n" +
                "Hotline: 0123-456-789\n" +
                "Địa chỉ: 123 Đường ABC, Quận XYZ, TP. Đà Nẵng\n\n" +
                "Giờ làm việc:\n" +
                "Thứ 2 - Thứ 6: 8:00 - 17:00\n" +
                "Thứ 7: 8:00 - 12:00\n" +
                "Chủ nhật: Nghỉ";

            MessageBox.Show(contactInfo, "Thông tin liên hệ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormAbout
            // 
            this.ClientSize = new System.Drawing.Size(700, 650);
            this.Name = "FormAbout";
            this.Load += new System.EventHandler(this.FormAbout_Load);
            this.ResumeLayout(false);

        }

        private void FormAbout_Load(object sender, EventArgs e)
        {

        }
    }
}