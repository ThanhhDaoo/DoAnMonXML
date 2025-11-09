using System;
using System.ComponentModel;

namespace LibraryManagement.Models
{
    /// <summary>
    /// Model đại diện cho một phiếu mượn/trả sách
    /// </summary>
    public class Loan
    {
        // Properties với data annotations
        [DisplayName("Mã phiếu")]
        public int LoanID { get; set; }

        [DisplayName("Mã sách")]
        public int BookID { get; set; }

        [DisplayName("Mã độc giả")]
        public int MemberID { get; set; }

        [DisplayName("Ngày mượn")]
        public DateTime LoanDate { get; set; }

        [DisplayName("Hạn trả")]
        public DateTime DueDate { get; set; }

        [DisplayName("Ngày trả")]
        public DateTime? ReturnDate { get; set; } // Nullable vì có thể chưa trả

        [DisplayName("Trạng thái")]
        public string Status { get; set; } // Borrowed, Returned, Overdue, Lost

        [DisplayName("Ghi chú")]
        public string Notes { get; set; }

        // Navigation properties (không lưu trong DB nhưng dùng để hiển thị)
        [DisplayName("Tên sách")]
        public string BookTitle { get; set; }

        [DisplayName("Tên độc giả")]
        public string MemberName { get; set; }

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public Loan()
        {
            LoanID = 0;
            BookID = 0;
            MemberID = 0;
            LoanDate = DateTime.Now;
            DueDate = DateTime.Now.AddDays(14); // Mặc định hạn trả 14 ngày
            ReturnDate = null;
            Status = "Borrowed";
            Notes = string.Empty;
            BookTitle = string.Empty;
            MemberName = string.Empty;
        }

        /// <summary>
        /// Constructor với tham số
        /// </summary>
        public Loan(int loanID, int bookID, int memberID, DateTime loanDate,
                   DateTime dueDate, DateTime? returnDate, string status, string notes)
        {
            LoanID = loanID;
            BookID = bookID;
            MemberID = memberID;
            LoanDate = loanDate;
            DueDate = dueDate;
            ReturnDate = returnDate;
            Status = status;
            Notes = notes;
            BookTitle = string.Empty;
            MemberName = string.Empty;
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của dữ liệu phiếu mượn
        /// </summary>
        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (BookID <= 0)
            {
                errorMessage = "Phải chọn sách để mượn!";
                return false;
            }

            if (MemberID <= 0)
            {
                errorMessage = "Phải chọn độc giả!";
                return false;
            }

            if (LoanDate > DateTime.Now)
            {
                errorMessage = "Ngày mượn không được lớn hơn ngày hiện tại!";
                return false;
            }

            if (DueDate <= LoanDate)
            {
                errorMessage = "Hạn trả phải sau ngày mượn!";
                return false;
            }

            if (ReturnDate.HasValue && ReturnDate.Value < LoanDate)
            {
                errorMessage = "Ngày trả không được nhỏ hơn ngày mượn!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Status))
            {
                errorMessage = "Trạng thái không được để trống!";
                return false;
            }

            string[] validStatuses = { "Borrowed", "Returned", "Overdue", "Lost" };
            if (Array.IndexOf(validStatuses, Status) == -1)
            {
                errorMessage = "Trạng thái không hợp lệ!";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra phiếu mượn có quá hạn không
        /// </summary>
        public bool IsOverdue()
        {
            if (ReturnDate.HasValue)
                return false; // Đã trả rồi thì không quá hạn

            return DateTime.Now.Date > DueDate.Date;
        }

        /// <summary>
        /// Tính số ngày quá hạn
        /// </summary>
        public int GetOverdueDays()
        {
            if (!IsOverdue())
                return 0;

            return (DateTime.Now.Date - DueDate.Date).Days;
        }

        /// <summary>
        /// Tính số ngày đã mượn
        /// </summary>
        public int GetBorrowedDays()
        {
            DateTime endDate = ReturnDate ?? DateTime.Now;
            return (endDate.Date - LoanDate.Date).Days;
        }

        /// <summary>
        /// Tính phí phạt (nếu quá hạn)
        /// </summary>
        /// <param name="finePerDay">Phí phạt mỗi ngày (mặc định 5000 VNĐ)</param>
        public decimal CalculateFine(decimal finePerDay = 5000)
        {
            int overdueDays = GetOverdueDays();
            if (overdueDays <= 0)
                return 0;

            return overdueDays * finePerDay;
        }

        /// <summary>
        /// Đánh dấu sách đã trả
        /// </summary>
        public void MarkAsReturned()
        {
            ReturnDate = DateTime.Now;
            Status = "Returned";
        }

        /// <summary>
        /// Đánh dấu sách bị mất
        /// </summary>
        public void MarkAsLost()
        {
            Status = "Lost";
        }

        /// <summary>
        /// Cập nhật trạng thái tự động dựa trên ngày
        /// </summary>
        public void UpdateStatus()
        {
            if (ReturnDate.HasValue)
            {
                Status = "Returned";
            }
            else if (Status == "Lost")
            {
                // Giữ nguyên trạng thái Lost
            }
            else if (IsOverdue())
            {
                Status = "Overdue";
            }
            else
            {
                Status = "Borrowed";
            }
        }

        /// <summary>
        /// Override ToString để hiển thị thông tin phiếu mượn
        /// </summary>
        public override string ToString()
        {
            string returnInfo = ReturnDate.HasValue ?
                $"Đã trả: {ReturnDate.Value:dd/MM/yyyy}" :
                $"Hạn trả: {DueDate:dd/MM/yyyy}";

            return $"[{LoanID}] {BookTitle} - {MemberName} ({returnInfo})";
        }

        /// <summary>
        /// Clone đối tượng Loan
        /// </summary>
        public Loan Clone()
        {
            return new Loan
            {
                LoanID = this.LoanID,
                BookID = this.BookID,
                MemberID = this.MemberID,
                LoanDate = this.LoanDate,
                DueDate = this.DueDate,
                ReturnDate = this.ReturnDate,
                Status = this.Status,
                Notes = this.Notes,
                BookTitle = this.BookTitle,
                MemberName = this.MemberName
            };
        }

        /// <summary>
        /// So sánh hai đối tượng Loan
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Loan))
                return false;

            Loan other = (Loan)obj;
            return this.LoanID == other.LoanID;
        }

        public override int GetHashCode()
        {
            return LoanID.GetHashCode();
        }

        /// <summary>
        /// Lấy màu hiển thị dựa trên trạng thái
        /// </summary>
        public System.Drawing.Color GetStatusColor()
        {
            switch (Status)
            {
                case "Borrowed":
                    return System.Drawing.Color.Blue;
                case "Returned":
                    return System.Drawing.Color.Green;
                case "Overdue":
                    return System.Drawing.Color.Red;
                case "Lost":
                    return System.Drawing.Color.DarkRed;
                default:
                    return System.Drawing.Color.Black;
            }
        }
    }
}