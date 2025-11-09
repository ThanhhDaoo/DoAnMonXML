using System;
using System.ComponentModel;

namespace LibraryManagement.Models
{
    /// <summary>
    /// Model đại diện cho một cuốn sách trong thư viện
    /// </summary>
    public class Book
    {
        // Properties với validation và data annotations
        [DisplayName("Mã sách")]
        public int BookID { get; set; }

        [DisplayName("Tên sách")]
        public string Title { get; set; }

        [DisplayName("Tác giả")]
        public string Author { get; set; }

        [DisplayName("Nhà xuất bản")]
        public string Publisher { get; set; }

        [DisplayName("Năm xuất bản")]
        public int PublishYear { get; set; }

        [DisplayName("Thể loại")]
        public string Category { get; set; }

        [DisplayName("Số lượng")]
        public int Quantity { get; set; }

        [DisplayName("ISBN")]
        public string ISBN { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public Book()
        {
            BookID = 0;
            Title = string.Empty;
            Author = string.Empty;
            Publisher = string.Empty;
            PublishYear = DateTime.Now.Year;
            Category = string.Empty;
            Quantity = 0;
            ISBN = string.Empty;
            Description = string.Empty;
        }

        /// <summary>
        /// Constructor với tham số
        /// </summary>
        public Book(int bookID, string title, string author, string publisher,
                   int publishYear, string category, int quantity, string isbn, string description)
        {
            BookID = bookID;
            Title = title;
            Author = author;
            Publisher = publisher;
            PublishYear = publishYear;
            Category = category;
            Quantity = quantity;
            ISBN = isbn;
            Description = description;
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của dữ liệu sách
        /// </summary>
        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Title))
            {
                errorMessage = "Tên sách không được để trống!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Author))
            {
                errorMessage = "Tác giả không được để trống!";
                return false;
            }

            if (PublishYear < 1800 || PublishYear > DateTime.Now.Year)
            {
                errorMessage = $"Năm xuất bản phải từ 1800 đến {DateTime.Now.Year}!";
                return false;
            }

            if (Quantity < 0)
            {
                errorMessage = "Số lượng không được âm!";
                return false;
            }

            if (!string.IsNullOrEmpty(ISBN) && ISBN.Length > 20)
            {
                errorMessage = "ISBN không được vượt quá 20 ký tự!";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Override ToString để hiển thị thông tin sách
        /// </summary>
        public override string ToString()
        {
            return $"[{BookID}] {Title} - {Author} ({PublishYear})";
        }

        /// <summary>
        /// Kiểm tra sách có sẵn để mượn không
        /// </summary>
        public bool IsAvailable()
        {
            return Quantity > 0;
        }

        /// <summary>
        /// Clone đối tượng Book
        /// </summary>
        public Book Clone()
        {
            return new Book
            {
                BookID = this.BookID,
                Title = this.Title,
                Author = this.Author,
                Publisher = this.Publisher,
                PublishYear = this.PublishYear,
                Category = this.Category,
                Quantity = this.Quantity,
                ISBN = this.ISBN,
                Description = this.Description
            };
        }

        /// <summary>
        /// So sánh hai đối tượng Book
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Book))
                return false;

            Book other = (Book)obj;
            return this.BookID == other.BookID;
        }

        public override int GetHashCode()
        {
            return BookID.GetHashCode();
        }
    }
}