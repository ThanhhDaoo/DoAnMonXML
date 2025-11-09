using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryManagement.Forms
{
    public partial class FormHelp : Form
    {
        public FormHelp()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            // Cấu hình Form
            this.Text = "Hướng Dẫn Sử Dụng";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.MaximizeBox = false;

            // Header Panel
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 100;
            headerPanel.BackColor = Color.FromArgb(52, 152, 219);

            Label lblTitle = new Label();
            lblTitle.Text = "📖 HƯỚNG DẪN SỬ DỤNG HỆ THỐNG";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(30, 20);
            lblTitle.AutoSize = true;

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Hướng dẫn chi tiết các chức năng của Hệ Thống Quản Lý Thư Viện";
            lblSubtitle.Font = new Font("Segoe UI", 11);
            lblSubtitle.ForeColor = Color.White;
            lblSubtitle.Location = new Point(30, 60);
            lblSubtitle.AutoSize = true;

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);

            // Content Panel với ScrollBar
            Panel contentPanel = new Panel();
            contentPanel.Location = new Point(0, 100);
            contentPanel.Size = new Size(884, 510);
            contentPanel.AutoScroll = true;
            contentPanel.BackColor = Color.FromArgb(245, 245, 245);

            // Rich Text Box cho nội dung
            RichTextBox rtbContent = new RichTextBox();
            rtbContent.Location = new Point(20, 20);
            rtbContent.Size = new Size(840, 2000);
            rtbContent.ReadOnly = true;
            rtbContent.BorderStyle = BorderStyle.None;
            rtbContent.BackColor = Color.White;
            rtbContent.Font = new Font("Segoe UI", 10);

            // Nội dung hướng dẫn
            rtbContent.Text = @"
═══════════════════════════════════════════════════════════════════════════

📚 1. QUẢN LÝ SÁCH

═══════════════════════════════════════════════════════════════════════════

🔹 THÊM SÁCH MỚI:
   • Click nút ""Thêm"" trên thanh công cụ
   • Điền đầy đủ thông tin: Tên sách, Tác giả, NXB, Năm XB, Thể loại, Số lượng, ISBN
   • Click ""Thêm"" để lưu
   • Hệ thống sẽ tự động tạo ID cho sách mới

🔹 SỬA THÔNG TIN SÁCH:
   • Click chọn sách cần sửa trong bảng
   • Thông tin sách sẽ hiển thị bên phải
   • Chỉnh sửa thông tin cần thiết
   • Click nút ""Sửa"" để cập nhật

🔹 XÓA SÁCH:
   • Click chọn sách cần xóa
   • Click nút ""Xóa""
   • Xác nhận xóa trong hộp thoại
   ⚠️ Lưu ý: Không thể xóa sách đang được mượn!

🔹 TÌM KIẾM SÁCH:
   • Nhập từ khóa vào ô ""Tìm kiếm"" (tên sách, tác giả, thể loại)
   • Hệ thống tự động lọc kết quả
   • Để xem tất cả, xóa nội dung ô tìm kiếm

🔹 EXPORT SÁCH RA XML:
   • Click nút ""📤 Export XML""
   • Chọn vị trí lưu file
   • Đặt tên file (ví dụ: DanhSachSach.xml)
   • Click ""Save""
   • File XML sẽ chứa toàn bộ thông tin sách

🔹 IMPORT SÁCH TỪ XML:
   • Chuẩn bị file XML đúng cấu trúc
   • Click nút ""📥 Import XML""
   • Chọn file XML cần import
   • Chọn có kiểm tra trùng lặp hay không:
     - YES: Bỏ qua sách đã tồn tại (an toàn)
     - NO: Thêm tất cả (có thể trùng)
   • Xem kết quả import trong thông báo


═══════════════════════════════════════════════════════════════════════════

👥 2. QUẢN LÝ ĐỘC GIẢ

═══════════════════════════════════════════════════════════════════════════

🔹 THÊM ĐỘC GIẢ MỚI:
   • Click nút ""Thêm""
   • Nhập: Họ tên, Email, Số điện thoại, Địa chỉ
   • Chọn ngày tham gia
   • Chọn trạng thái: Active/Inactive/Suspended
   • Click ""Thêm"" để lưu

🔹 CẬP NHẬT THÔNG TIN:
   • Chọn độc giả cần sửa
   • Cập nhật thông tin
   • Click ""Sửa""

🔹 QUẢN LÝ TRẠNG THÁI:
   • Active: Độc giả đang hoạt động, được mượn sách
   • Inactive: Tạm ngưng hoạt động
   • Suspended: Bị đình chỉ (quá hạn trả sách nhiều)

🔹 EXPORT/IMPORT ĐỘC GIẢ:
   • Tương tự như Export/Import sách
   • File: DanhSachDocGia.xml


═══════════════════════════════════════════════════════════════════════════

📝 3. QUẢN LÝ MƯỢN/TRẢ SÁCH

═══════════════════════════════════════════════════════════════════════════

🔹 TẠO PHIẾU MƯỢN:
   • Click nút ""Mượn sách""
   • Chọn sách từ danh sách (ComboBox)
   • Chọn độc giả
   • Chọn ngày mượn (mặc định: hôm nay)
   • Chọn hạn trả (khuyến nghị: 14 ngày)
   • Nhập ghi chú (nếu có)
   • Click ""Mượn sách"" để tạo phiếu

🔹 TRẢ SÁCH:
   • Chọn phiếu mượn cần trả
   • Click nút ""Trả sách""
   • Hệ thống tự động:
     - Cập nhật ngày trả = hôm nay
     - Đổi trạng thái = ""Returned""
     - Tính số ngày mượn

🔹 KIỂM TRA QUÁ HẠN:
   • Phiếu có hạn trả < hôm nay = Quá hạn
   • Trạng thái tự động chuyển sang ""Overdue""
   • Xem danh sách quá hạn trong Báo cáo

🔹 XÓA PHIẾU MƯỢN:
   • Chỉ xóa khi nhập sai
   • Click ""Xóa"" và xác nhận


═══════════════════════════════════════════════════════════════════════════

📊 4. BÁO CÁO VÀ THỐNG KÊ

═══════════════════════════════════════════════════════════════════════════

🔹 THỐNG KÊ SÁCH:
   
   Tab ""Tổng quan"":
   • Hiển thị 3 thẻ thống kê: Tổng đầu sách, Tổng số lượng, Số thể loại
   • Bảng chi tiết theo thể loại
   
   Tab ""Theo thể loại"":
   • Số đầu sách, tổng số lượng từng thể loại
   • Năm xuất bản trung bình
   
   Tab ""Sách phổ biến"":
   • TOP 10 sách được mượn nhiều nhất
   • Xếp hạng: Vàng-Bạc-Đồng cho top 3
   
   Tab ""Chi tiết"":
   • Danh sách toàn bộ sách
   • Lọc theo thể loại

🔹 THỐNG KÊ MƯỢN/TRẢ:
   
   Bộ lọc thời gian:
   • Chọn ""Từ ngày"" và ""Đến ngày""
   • Click ""🔍 Lọc"" để xem báo cáo theo khoảng thời gian
   
   Tab ""Tổng quan"":
   • 4 thẻ: Tổng phiếu, Đang mượn, Đã trả, Quá hạn
   • Bảng chi tiết theo trạng thái
   
   Tab ""Theo độc giả"":
   • TOP 10 độc giả mượn nhiều nhất
   • Thông tin: Họ tên, Email, SĐT, Số lần mượn
   
   Tab ""Quá hạn"" ⚠️:
   • Danh sách TẤT CẢ sách quá hạn
   • Hiển thị: Tên sách, Độc giả, SĐT, Số ngày quá hạn
   • Tô màu cảnh báo:
     - Đỏ: Quá hạn > 30 ngày (nghiêm trọng)
     - Cam: Quá hạn > 14 ngày (cảnh báo)
   
   Tab ""Chi tiết"":
   • Toàn bộ phiếu mượn trong khoảng thời gian
   • Lọc theo trạng thái: Tất cả/Borrowed/Returned/Overdue


═══════════════════════════════════════════════════════════════════════════

⚙️ 5. CẤU TRÚC FILE XML

═══════════════════════════════════════════════════════════════════════════

🔹 FILE XML SÁCH (DanhSachSach.xml):

<?xml version=""1.0"" encoding=""utf-8""?>
<Library ExportDate=""2024-11-09"" TotalBooks=""10"">
  <Book>
    <BookID>1</BookID>
    <Title>Tên sách</Title>
    <Author>Tác giả</Author>
    <Publisher>Nhà xuất bản</Publisher>
    <PublishYear>2024</PublishYear>
    <Category>Thể loại</Category>
    <Quantity>10</Quantity>
    <ISBN>978-xxxx</ISBN>
    <Description>Mô tả</Description>
  </Book>
  <!-- Thêm các Book khác -->
</Library>

🔹 FILE XML ĐỘC GIẢ (DanhSachDocGia.xml):

<?xml version=""1.0"" encoding=""utf-8""?>
<MembersList ExportDate=""2024-11-09"" TotalMembers=""5"">
  <Member>
    <MemberID>1</MemberID>
    <FullName>Nguyễn Văn A</FullName>
    <Email>a@email.com</Email>
    <Phone>0901234567</Phone>
    <Address>123 Đường ABC, Đà Nẵng</Address>
    <JoinDate>2024-01-01</JoinDate>
    <Status>Active</Status>
  </Member>
  <!-- Thêm các Member khác -->
</MembersList>


═══════════════════════════════════════════════════════════════════════════

💡 6. MẸO VÀ LƯU Ý

═══════════════════════════════════════════════════════════════════════════

✅ MẸO SỬ DỤNG:
   • Sử dụng phím TAB để di chuyển nhanh giữa các ô nhập liệu
   • Double-click vào dòng trong bảng để xem chi tiết
   • Nhấn F5 hoặc nút ""Làm mới"" để cập nhật dữ liệu mới nhất
   • Export dữ liệu thường xuyên để backup

⚠️ LƯU Ý QUAN TRỌNG:
   • Không xóa sách đang có người mượn
   • Kiểm tra kỹ thông tin trước khi xóa
   • File XML import phải đúng cấu trúc
   • Backup dữ liệu định kỳ bằng Export XML
   • Kiểm tra sách quá hạn hàng ngày
   • Liên hệ độc giả khi sách quá hạn > 7 ngày

❌ TRÁNH CÁC SAI LẦM:
   • Không nhập trùng ISBN cho các sách khác nhau
   • Không để trống thông tin bắt buộc (Tên sách, Họ tên độc giả)
   • Không đặt hạn trả quá ngắn (< 7 ngày) hoặc quá dài (> 30 ngày)
   • Không import file XML từ nguồn không rõ ràng


═══════════════════════════════════════════════════════════════════════════

📞 7. HỖ TRỢ KỸ THUẬT

═══════════════════════════════════════════════════════════════════════════

Nếu gặp vấn đề, vui lòng liên hệ:
   📧 Email: support@library.com
   ☎️ Hotline: 0362-625-218
   🌐 Website: www.library.com

Hoặc xem thêm tài liệu hướng dẫn chi tiết tại menu ""Trợ giúp"" → ""Về chúng tôi""


═══════════════════════════════════════════════════════════════════════════

Cảm ơn bạn đã sử dụng Hệ Thống Quản Lý Thư Viện! 📚
";

            // Format text
            rtbContent.SelectAll();
            rtbContent.SelectionFont = new Font("Segoe UI", 10);
            rtbContent.DeselectAll();

            contentPanel.Controls.Add(rtbContent);

            // Button Panel
            Panel btnPanel = new Panel();
            btnPanel.Dock = DockStyle.Bottom;
            btnPanel.Height = 60;
            btnPanel.BackColor = Color.White;

            Button btnPrint = new Button();
            btnPrint.Text = "🖨️ In hướng dẫn";
            btnPrint.Location = new Point(300, 15);
            btnPrint.Size = new Size(130, 35);
            btnPrint.BackColor = Color.FromArgb(52, 152, 219);
            btnPrint.ForeColor = Color.White;
            btnPrint.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnPrint.FlatStyle = FlatStyle.Flat;
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Click += (s, e) => MessageBox.Show("Chức năng in đang được phát triển", "Thông báo");

            Button btnClose = new Button();
            btnClose.Text = "❌ Đóng";
            btnClose.Location = new Point(450, 15);
            btnClose.Size = new Size(130, 35);
            btnClose.BackColor = Color.FromArgb(231, 76, 60);
            btnClose.ForeColor = Color.White;
            btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            btnPanel.Controls.Add(btnPrint);
            btnPanel.Controls.Add(btnClose);

            // Add to form
            this.Controls.Add(contentPanel);
            this.Controls.Add(btnPanel);
            this.Controls.Add(headerPanel);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.Name = "FormHelp";
            this.ResumeLayout(false);
        }
    }
}