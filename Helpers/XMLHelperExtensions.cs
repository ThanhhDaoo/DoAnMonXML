using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LibraryManagement.Helpers
{
    /// <summary>
    /// XML Helper cho Members
    /// </summary>
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

    /// <summary>
    /// XML Helper cho Loans
    /// </summary>
    public class XMLHelperLoans
    {
        public static bool ExportLoansToXML(DataGridView dgv, string filePath)
        {
            try
            {
                var xmlDoc = new System.Xml.Linq.XDocument(
                    new System.Xml.Linq.XDeclaration("1.0", "utf-8", "yes"),
                    new System.Xml.Linq.XElement("LoansList",
                        new System.Xml.Linq.XAttribute("ExportDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    )
                );

                var root = xmlDoc.Root;

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    var loan = new System.Xml.Linq.XElement("Loan",
                        new System.Xml.Linq.XElement("LoanID", row.Cells["LoanID"].Value),
                        new System.Xml.Linq.XElement("BookID", row.Cells["BookID"].Value),
                        new System.Xml.Linq.XElement("MemberID", row.Cells["MemberID"].Value),
                        new System.Xml.Linq.XElement("LoanDate", row.Cells["LoanDate"].Value),
                        new System.Xml.Linq.XElement("DueDate", row.Cells["DueDate"].Value),
                        new System.Xml.Linq.XElement("ReturnDate", row.Cells["ReturnDate"].Value ?? ""),
                        new System.Xml.Linq.XElement("Status", row.Cells["Status"].Value),
                        new System.Xml.Linq.XElement("Notes", row.Cells["Notes"].Value ?? "")
                    );

                    root.Add(loan);
                }

                xmlDoc.Save(filePath);
                MessageBox.Show("Export thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool ImportLoansFromXML(string filePath)
        {
            try
            {
                var xmlDoc = System.Xml.Linq.XDocument.Load(filePath);
                var loans = xmlDoc.Root.Elements("Loan");

                int success = 0;
                foreach (var loan in loans)
                {
                    string query = @"INSERT INTO Loans (BookID, MemberID, LoanDate, DueDate, ReturnDate, Status, Notes)
                                   VALUES (@BookID, @MemberID, @LoanDate, @DueDate, @ReturnDate, @Status, @Notes)";

                    SqlParameter[] parameters = {
                        new SqlParameter("@BookID", int.Parse(loan.Element("BookID").Value)),
                        new SqlParameter("@MemberID", int.Parse(loan.Element("MemberID").Value)),
                        new SqlParameter("@LoanDate", DateTime.Parse(loan.Element("LoanDate").Value)),
                        new SqlParameter("@DueDate", DateTime.Parse(loan.Element("DueDate").Value)),
                        new SqlParameter("@ReturnDate", string.IsNullOrEmpty(loan.Element("ReturnDate").Value) ? (object)DBNull.Value : DateTime.Parse(loan.Element("ReturnDate").Value)),
                        new SqlParameter("@Status", loan.Element("Status").Value),
                        new SqlParameter("@Notes", loan.Element("Notes").Value)
                    };

                    if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                        success++;
                }

                MessageBox.Show($"Import thành công {success} phiếu mượn!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
