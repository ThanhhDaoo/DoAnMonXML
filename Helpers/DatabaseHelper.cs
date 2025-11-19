using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LibraryManagement.Helpers
{
    /// <summary>
    /// Class quản lý kết nối và thao tác với SQL Server Database
    /// </summary>
    public class DatabaseHelper
    {
        // ============================================================
        // QUAN TRỌNG: SỬA CONNECTION STRING NÀY
        // ============================================================

        // Cách 1: Windows Authentication (Khuyên dùng - Không cần password)
        private static string connectionString = @"Data Source=DESKTOP-6F7FSVO;Initial Catalog=LibraryManagement;Integrated Security=True;";

        // Cách 2: SQL Server Authentication (Nếu có username/password)
        // private static string connectionString = @"Data Source=YOUR_SERVER_NAME;Initial Catalog=LibraryManagement;User ID=sa;Password=YOUR_PASSWORD";

        // VÍ DỤ THỰC TÊ:
        // Nếu server name là: DESKTOP-ABC123\SQLEXPRESS
        // private static string connectionString = @"Data Source=DESKTOP-ABC123\SQLEXPRESS;Initial Catalog=LibraryManagement;Integrated Security=True";

        // Hoặc đơn giản:
        // private static string connectionString = @"Data Source=localhost;Initial Catalog=LibraryManagement;Integrated Security=True";
        // private static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=LibraryManagement;Integrated Security=True";

        /// <summary>
        /// Lấy chuỗi kết nối hiện tại
        /// </summary>
        public static string GetConnectionString()
        {
            return connectionString;
        }

        /// <summary>
        /// Thiết lập chuỗi kết nối mới (nếu cần thay đổi runtime)
        /// </summary>
        public static void SetConnectionString(string newConnectionString)
        {
            connectionString = newConnectionString;
        }

        /// <summary>
        /// Kiểm tra kết nối đến SQL Server Database
        /// Trả về true nếu kết nối thành công, false nếu thất bại
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Kiểm tra thêm xem database có tồn tại không
                    string checkDbQuery = "SELECT DB_ID('LibraryManagement')";
                    using (SqlCommand cmd = new SqlCommand(checkDbQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            MessageBox.Show(
                                "Database 'LibraryManagement' không tồn tại!\n\n" +
                                "Vui lòng:\n" +
                                "1. Mở SQL Server Management Studio\n" +
                                "2. Chạy script LibraryManagement.sql để tạo database",
                                "Lỗi Database",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return false;
                        }
                    }

                    MessageBox.Show(
                        "✅ Kết nối Database thành công!\n\n" +
                        $"Server: {conn.DataSource}\n" +
                        $"Database: {conn.Database}\n" +
                        $"State: {conn.State}",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    return true;
                }
            }
            catch (SqlException sqlEx)
            {
                string errorMessage = "❌ Lỗi kết nối SQL Server:\n\n";

                switch (sqlEx.Number)
                {
                    case 2:
                    case 53:
                        errorMessage += "🔴 Không thể kết nối đến SQL Server!\n\n" +
                            "Kiểm tra:\n" +
                            "1. SQL Server đã được cài đặt chưa?\n" +
                            "2. SQL Server đang chạy không? (SQL Server Configuration Manager)\n" +
                            "3. Tên server đúng chưa? (Xem trong SSMS)\n\n" +
                            $"Tên server hiện tại: {connectionString.Split(';')[0].Replace("Data Source=", "")}";
                        break;

                    case 4060:
                        errorMessage += "🔴 Database 'LibraryManagement' không tồn tại!\n\n" +
                            "Giải quyết:\n" +
                            "1. Mở SQL Server Management Studio\n" +
                            "2. Chạy script LibraryManagement.sql\n" +
                            "3. Tạo database và các bảng";
                        break;

                    case 18456:
                        errorMessage += "🔴 Lỗi xác thực! Username/Password không đúng\n\n" +
                            "Giải quyết:\n" +
                            "1. Nếu dùng Windows Authentication: Dùng Integrated Security=True\n" +
                            "2. Nếu dùng SQL Authentication: Kiểm tra User ID và Password";
                        break;

                    default:
                        errorMessage += $"Mã lỗi: {sqlEx.Number}\n" +
                            $"Chi tiết: {sqlEx.Message}";
                        break;
                }

                MessageBox.Show(errorMessage, "Lỗi Kết Nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Lỗi không xác định:\n\n{ex.Message}\n\n" +
                    $"Stack Trace:\n{ex.StackTrace}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
        }

        /// <summary>
        /// Thực thi câu lệnh SELECT và trả về DataTable
        /// Dùng cho: Load dữ liệu lên DataGridView, ComboBox, v.v.
        /// </summary>
        /// <param name="query">Câu lệnh SQL SELECT</param>
        /// <param name="parameters">Mảng SqlParameter (tùy chọn)</param>
        /// <returns>DataTable chứa kết quả truy vấn</returns>
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Set timeout 30 giây
                        cmd.CommandTimeout = 30;

                        // Thêm parameters nếu có
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        // Sử dụng SqlDataAdapter để fill dữ liệu vào DataTable
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show(
                    $"❌ Lỗi SQL khi truy vấn dữ liệu:\n\n" +
                    $"Mã lỗi: {sqlEx.Number}\n" +
                    $"Chi tiết: {sqlEx.Message}\n\n" +
                    $"Query: {query}",
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Lỗi khi truy vấn dữ liệu:\n\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            return dataTable;
        }

        /// <summary>
        /// Thực thi câu lệnh INSERT, UPDATE, DELETE
        /// Trả về số dòng bị ảnh hưởng
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Mảng SqlParameter (tùy chọn)</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            int rowsAffected = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandTimeout = 30;

                        // Thêm parameters nếu có
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show(
                    $"❌ Lỗi SQL khi thực thi lệnh:\n\n" +
                    $"Mã lỗi: {sqlEx.Number}\n" +
                    $"Chi tiết: {sqlEx.Message}\n\n" +
                    $"Query: {query}",
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Lỗi khi thực thi lệnh:\n\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            return rowsAffected;
        }

        /// <summary>
        /// Thực thi câu lệnh trả về 1 giá trị duy nhất (Scalar)
        /// Dùng cho: COUNT, SUM, AVG, MAX, MIN, hoặc lấy 1 giá trị cụ thể
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Mảng SqlParameter (tùy chọn)</param>
        /// <returns>Object chứa giá trị (có thể null)</returns>
        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            object result = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandTimeout = 30;

                        // Thêm parameters nếu có
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        result = cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show(
                    $"❌ Lỗi SQL khi thực thi ExecuteScalar:\n\n" +
                    $"Mã lỗi: {sqlEx.Number}\n" +
                    $"Chi tiết: {sqlEx.Message}\n\n" +
                    $"Query: {query}",
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Lỗi khi thực thi ExecuteScalar:\n\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            return result;
        }

        /// <summary>
        /// Kiểm tra xem ID có tồn tại trong bảng không
        /// </summary>
        /// <param name="tableName">Tên bảng</param>
        /// <param name="idColumn">Tên cột ID</param>
        /// <param name="id">Giá trị ID cần kiểm tra</param>
        /// <returns>true nếu tồn tại, false nếu không</returns>
        public static bool IsIDExists(string tableName, string idColumn, int id)
        {
            try
            {
                string query = $"SELECT COUNT(*) FROM {tableName} WHERE {idColumn} = @ID";

                SqlParameter[] parameters = {
                    new SqlParameter("@ID", id)
                };

                object result = ExecuteScalar(query, parameters);

                if (result != null && result != DBNull.Value)
                {
                    int count = Convert.ToInt32(result);
                    return count > 0;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra xem giá trị có tồn tại trong cột không (dùng cho kiểm tra unique)
        /// </summary>
        public static bool IsValueExists(string tableName, string columnName, string value, int? excludeID = null, string idColumn = null)
        {
            try
            {
                string query = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @Value";

                SqlParameter[] parameters;

                if (excludeID.HasValue && !string.IsNullOrEmpty(idColumn))
                {
                    query += $" AND {idColumn} != @ExcludeID";
                    parameters = new SqlParameter[] {
                        new SqlParameter("@Value", value),
                        new SqlParameter("@ExcludeID", excludeID.Value)
                    };
                }
                else
                {
                    parameters = new SqlParameter[] {
                        new SqlParameter("@Value", value)
                    };
                }

                object result = ExecuteScalar(query, parameters);

                if (result != null && result != DBNull.Value)
                {
                    int count = Convert.ToInt32(result);
                    return count > 0;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy giá trị ID mới nhất sau khi INSERT (dùng SCOPE_IDENTITY)
        /// </summary>
        public static int GetLastInsertedID(string query, SqlParameter[] parameters = null)
        {
            try
            {
                // Thêm SCOPE_IDENTITY() vào cuối query
                string queryWithIdentity = query + "; SELECT SCOPE_IDENTITY();";

                object result = ExecuteScalar(queryWithIdentity, parameters);

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }

                return -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Thực thi nhiều câu lệnh SQL trong một Transaction
        /// </summary>
        public static bool ExecuteTransaction(string[] queries, SqlParameter[][] parameters = null)
        {
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();

                transaction = conn.BeginTransaction();

                for (int i = 0; i < queries.Length; i++)
                {
                    using (SqlCommand cmd = new SqlCommand(queries[i], conn, transaction))
                    {
                        if (parameters != null && parameters.Length > i && parameters[i] != null)
                        {
                            cmd.Parameters.AddRange(parameters[i]);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch { }
                }

                MessageBox.Show(
                    $"❌ Lỗi khi thực thi Transaction:\n\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return false;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();

                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
    }
}