using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace LibraryManagement.Models
{
    /// <summary>
    /// Model đại diện cho một độc giả trong thư viện
    /// </summary>
    public class Member
    {
        // Properties với data annotations
        [DisplayName("Mã độc giả")]
        public int MemberID { get; set; }

        [DisplayName("Họ và tên")]
        public string FullName { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }

        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        [DisplayName("Ngày tham gia")]
        public DateTime JoinDate { get; set; }

        [DisplayName("Trạng thái")]
        public string Status { get; set; } // Active, Inactive, Suspended

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public Member()
        {
            MemberID = 0;
            FullName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
            JoinDate = DateTime.Now;
            Status = "Active";
        }

        /// <summary>
        /// Constructor với tham số
        /// </summary>
        public Member(int memberID, string fullName, string email, string phone,
                     string address, DateTime joinDate, string status)
        {
            MemberID = memberID;
            FullName = fullName;
            Email = email;
            Phone = phone;
            Address = address;
            JoinDate = joinDate;
            Status = status;
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của email
        /// </summary>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true; // Email có thể để trống

            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của số điện thoại
        /// </summary>
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true; // Số điện thoại có thể để trống

            // Kiểm tra số điện thoại Việt Nam (10-11 số, bắt đầu bằng 0)
            string pattern = @"^0\d{9,10}$";
            return Regex.IsMatch(phone, pattern);
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của dữ liệu độc giả
        /// </summary>
        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(FullName))
            {
                errorMessage = "Họ tên không được để trống!";
                return false;
            }

            if (FullName.Length < 2 || FullName.Length > 100)
            {
                errorMessage = "Họ tên phải từ 2 đến 100 ký tự!";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Email) && !IsValidEmail(Email))
            {
                errorMessage = "Email không hợp lệ!";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Phone) && !IsValidPhone(Phone))
            {
                errorMessage = "Số điện thoại không hợp lệ! (Phải là số Việt Nam 10-11 chữ số)";
                return false;
            }

            if (JoinDate > DateTime.Now)
            {
                errorMessage = "Ngày tham gia không được lớn hơn ngày hiện tại!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Status))
            {
                errorMessage = "Trạng thái không được để trống!";
                return false;
            }

            string[] validStatuses = { "Active", "Inactive", "Suspended" };
            if (Array.IndexOf(validStatuses, Status) == -1)
            {
                errorMessage = "Trạng thái phải là: Active, Inactive hoặc Suspended!";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra độc giả có đang hoạt động không
        /// </summary>
        public bool IsActive()
        {
            return Status == "Active";
        }

        /// <summary>
        /// Tính số năm là thành viên
        /// </summary>
        public int GetMembershipYears()
        {
            return DateTime.Now.Year - JoinDate.Year;
        }

        /// <summary>
        /// Override ToString để hiển thị thông tin độc giả
        /// </summary>
        public override string ToString()
        {
            return $"[{MemberID}] {FullName} - {Phone} ({Status})";
        }

        /// <summary>
        /// Clone đối tượng Member
        /// </summary>
        public Member Clone()
        {
            return new Member
            {
                MemberID = this.MemberID,
                FullName = this.FullName,
                Email = this.Email,
                Phone = this.Phone,
                Address = this.Address,
                JoinDate = this.JoinDate,
                Status = this.Status
            };
        }

        /// <summary>
        /// So sánh hai đối tượng Member
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Member))
                return false;

            Member other = (Member)obj;
            return this.MemberID == other.MemberID;
        }

        public override int GetHashCode()
        {
            return MemberID.GetHashCode();
        }

        /// <summary>
        /// Lấy tên viết tắt (cho avatar)
        /// </summary>
        public string GetInitials()
        {
            if (string.IsNullOrWhiteSpace(FullName))
                return "??";

            string[] words = FullName.Trim().Split(' ');
            if (words.Length == 1)
                return words[0].Substring(0, Math.Min(2, words[0].Length)).ToUpper();

            return (words[0][0].ToString() + words[words.Length - 1][0].ToString()).ToUpper();
        }
    }
}