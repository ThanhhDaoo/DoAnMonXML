using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LibraryManagement.Helpers
{
    /// <summary>
    /// Class quản lý Import/Export dữ liệu XML
    /// </summary>
    public class XMLHelper
    {
        // ========================================
        // PHẦN 1: EXPORT DỮ LIỆU TỪ SQL/DATAGRIDVIEW RA XML
        // ========================================

        /// <summary>
        /// Export dữ liệu Sách từ DataGridView ra file XML
        /// </summary>
        public static bool ExportBooksToXML(DataGridView dgv, string filePath)
        {
            try
            {
                // Tạo XML document với LINQ to XML
                XDocument xmlDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("Library",
                        new XAttribute("ExportDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                        new XAttribute("TotalBooks", dgv.Rows.Count)
                    )
                );

                // Lấy element gốc
                XElement library = xmlDoc.Root;

                // Duyệt qua từng dòng trong DataGridView
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    // Bỏ qua dòng trống
                    if (row.IsNewRow) continue;

                    // Tạo element Book cho mỗi cuốn sách
                    XElement book = new XElement("Book",
                        new XElement("BookID", row.Cells["BookID"].Value?.ToString() ?? ""),
                        new XElement("Title", row.Cells["Title"].Value?.ToString() ?? ""),
                        new XElement("Author", row.Cells["Author"].Value?.ToString() ?? ""),
                        new XElement("Publisher", row.Cells["Publisher"].Value?.ToString() ?? ""),
                        new XElement("PublishYear", row.Cells["PublishYear"].Value?.ToString() ?? ""),
                        new XElement("Category", row.Cells["Category"].Value?.ToString() ?? ""),
                        new XElement("Quantity", row.Cells["Quantity"].Value?.ToString() ?? "0"),
                        new XElement("ISBN", row.Cells["ISBN"].Value?.ToString() ?? ""),
                        new XElement("Description", row.Cells["Description"].Value?.ToString() ?? "")
                    );

                    library.Add(book);
                }

                // Lưu file XML
                xmlDoc.Save(filePath);

                MessageBox.Show($"Export thành công {dgv.Rows.Count} cuốn sách ra file:\n{filePath}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi export XML:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Export dữ liệu Sách trực tiếp từ Database ra XML
        /// </summary>
        public static bool ExportBooksFromDatabaseToXML(string filePath)
        {
            try
            {
                // Lấy dữ liệu từ database
                string query = "SELECT * FROM Books ORDER BY BookID";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu sách để export!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tạo XML document
                XDocument xmlDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("Library",
                        new XAttribute("ExportDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                        new XAttribute("TotalBooks", dt.Rows.Count)
                    )
                );

                XElement library = xmlDoc.Root;

                // Duyệt qua từng dòng trong DataTable
                foreach (DataRow row in dt.Rows)
                {
                    XElement book = new XElement("Book",
                        new XElement("BookID", row["BookID"]),
                        new XElement("Title", row["Title"]),
                        new XElement("Author", row["Author"]),
                        new XElement("Publisher", row["Publisher"]),
                        new XElement("PublishYear", row["PublishYear"]),
                        new XElement("Category", row["Category"]),
                        new XElement("Quantity", row["Quantity"]),
                        new XElement("ISBN", row["ISBN"]),
                        new XElement("Description", row["Description"])
                    );

                    library.Add(book);
                }

                // Lưu file
                xmlDoc.Save(filePath);

                MessageBox.Show($"Export thành công {dt.Rows.Count} cuốn sách ra file:\n{filePath}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi export XML:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ========================================
        // PHẦN 2: IMPORT DỮ LIỆU TỪ XML VÀO SQL SERVER
        // ========================================

        /// <summary>
        /// Import dữ liệu Sách từ file XML vào Database
        /// </summary>
        public static bool ImportBooksFromXML(string filePath, bool checkDuplicate = true)
        {
            try
            {
                // Kiểm tra file có tồn tại không
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File XML không tồn tại!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Đọc file XML
                XDocument xmlDoc = XDocument.Load(filePath);
                XElement library = xmlDoc.Root;

                if (library == null || library.Name != "Library")
                {
                    MessageBox.Show("Định dạng file XML không hợp lệ! Root element phải là 'Library'.",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Lấy danh sách các sách
                var books = library.Elements("Book");

                int totalBooks = books.Count();
                int successCount = 0;
                int skipCount = 0;
                int errorCount = 0;

                // Duyệt qua từng sách và thêm vào database
                foreach (XElement book in books)
                {
                    try
                    {
                        // Lấy thông tin từ XML
                        string bookIDStr = book.Element("BookID")?.Value;
                        string title = book.Element("Title")?.Value ?? "";
                        string author = book.Element("Author")?.Value ?? "";
                        string publisher = book.Element("Publisher")?.Value ?? "";
                        string publishYearStr = book.Element("PublishYear")?.Value ?? "0";
                        string category = book.Element("Category")?.Value ?? "";
                        string quantityStr = book.Element("Quantity")?.Value ?? "0";
                        string isbn = book.Element("ISBN")?.Value ?? "";
                        string description = book.Element("Description")?.Value ?? "";

                        // Chuyển đổi kiểu dữ liệu
                        int publishYear = 0;
                        int.TryParse(publishYearStr, out publishYear);

                        int quantity = 0;
                        int.TryParse(quantityStr, out quantity);

                        // Kiểm tra trùng lặp nếu cần
                        if (checkDuplicate && !string.IsNullOrEmpty(bookIDStr))
                        {
                            int bookID;
                            if (int.TryParse(bookIDStr, out bookID))
                            {
                                if (DatabaseHelper.IsIDExists("Books", "BookID", bookID))
                                {
                                    skipCount++;
                                    continue; // Bỏ qua sách đã tồn tại
                                }
                            }
                        }

                        // Thêm sách vào database
                        string insertQuery = @"
                            INSERT INTO Books (Title, Author, Publisher, PublishYear, Category, Quantity, ISBN, Description)
                            VALUES (@Title, @Author, @Publisher, @PublishYear, @Category, @Quantity, @ISBN, @Description)";

                        SqlParameter[] parameters = {
                            new SqlParameter("@Title", title),
                            new SqlParameter("@Author", author),
                            new SqlParameter("@Publisher", publisher),
                            new SqlParameter("@PublishYear", publishYear),
                            new SqlParameter("@Category", category),
                            new SqlParameter("@Quantity", quantity),
                            new SqlParameter("@ISBN", isbn),
                            new SqlParameter("@Description", description)
                        };

                        int result = DatabaseHelper.ExecuteNonQuery(insertQuery, parameters);

                        if (result > 0)
                        {
                            successCount++;
                        }
                        else
                        {
                            errorCount++;
                        }
                    }
                    catch
                    {
                        errorCount++;
                    }
                }

                // Hiển thị kết quả
                string message = $"Kết quả Import:\n\n" +
                    $"- Tổng số sách trong file: {totalBooks}\n" +
                    $"- Thêm thành công: {successCount}\n" +
                    $"- Bỏ qua (trùng lặp): {skipCount}\n" +
                    $"- Lỗi: {errorCount}";

                MessageBox.Show(message, "Hoàn thành", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return successCount > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi import XML:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ========================================
        // PHẦN 3: VALIDATE XML
        // ========================================

        /// <summary>
        /// Kiểm tra tính hợp lệ của file XML
        /// </summary>
        public static bool ValidateXMLFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                XDocument xmlDoc = XDocument.Load(filePath);
                XElement library = xmlDoc.Root;

                if (library == null || library.Name != "Library")
                    return false;

                return library.Elements("Book").Any();
            }
            catch
            {
                return false;
            }
        }
    }
}