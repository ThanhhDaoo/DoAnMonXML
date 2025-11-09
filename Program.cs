using System;
using System.Windows.Forms;
using LibraryManagement.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement
{
    /// <summary>
    /// Class chính - Entry Point của ứng dụng Quản lý Thư viện
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Cấu hình hiển thị cho Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ====================================================
            // BƯỚC 1: KIỂM TRA KẾT NỐI DATABASE
            // ====================================================
            try
            {
                // Test kết nối database trước khi chạy ứng dụng
                bool isConnected = DatabaseHelper.TestConnection();

                if (!isConnected)
                {
                    // Nếu kết nối thất bại, hiển thị hướng dẫn
                    DialogResult result = MessageBox.Show(
                        "❌ Không thể kết nối đến Database!\n\n" +
                        "Vui lòng kiểm tra:\n" +
                        "1. SQL Server đã được cài đặt và đang chạy\n" +
                        "2. Connection String trong DatabaseHelper.cs đã đúng\n" +
                        "3. Database 'LibraryManagement' đã được tạo (chạy file .sql)\n\n" +
                        "Bạn có muốn tiếp tục chạy ứng dụng không?\n" +
                        "(Chọn NO để thoát và kiểm tra lại cấu hình)",
                        "Lỗi Kết Nối Database",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error
                    );

                    if (result == DialogResult.No)
                    {
                        // Người dùng chọn thoát để fix database
                        return;
                    }
                }
                else
                {
                    // Kết nối thành công - Có thể bỏ comment dòng dưới nếu muốn thông báo
                    // MessageBox.Show("✅ Kết nối Database thành công!", "Thông báo", 
                    //     MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi không xác định khi kiểm tra kết nối:\n\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            // ====================================================
            // BƯỚC 2: KHỞI CHẠY ỨNG DỤNG
            // ====================================================
            try
            {
                // Khởi chạy Form Login (Form đăng nhập)
                Application.Run(new FormLogin());

                // HOẶC nếu muốn bỏ qua đăng nhập (cho debug), dùng dòng dưới:
                // Application.Run(new FormMain("Admin", "Admin"));
            }
            catch (Exception ex)
            {
                // Bắt lỗi toàn cục của ứng dụng
                MessageBox.Show(
                    $"❌ Lỗi nghiêm trọng:\n\n{ex.Message}\n\n" +
                    $"Stack Trace:\n{ex.StackTrace}",
                    "Lỗi Ứng Dụng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Xử lý các exception chưa được bắt trong toàn ứng dụng
        /// </summary>
        static Program()
        {
            // Đăng ký handler cho exception chưa được xử lý
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            // Đăng ký handler cho exception trong AppDomain
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        /// <summary>
        /// Xử lý exception trong UI thread
        /// </summary>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(
                $"❌ Lỗi ứng dụng:\n\n{e.Exception.Message}\n\n" +
                $"Vui lòng liên hệ nhà phát triển nếu lỗi tiếp tục xảy ra.",
                "Lỗi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        /// <summary>
        /// Xử lý exception không được xử lý trong AppDomain
        /// </summary>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                MessageBox.Show(
                    $"❌ Lỗi nghiêm trọng:\n\n{ex.Message}\n\n" +
                    $"Ứng dụng sẽ đóng để tránh mất dữ liệu.",
                    "Lỗi Nghiêm Trọng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}