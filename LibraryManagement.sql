-- =====================================================
-- SCRIPT TẠO DATABASE QUẢN LÝ THƯ VIỆN
-- =====================================================
-- Bổ sung câu lệnh if exists
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'LibraryManagement')
BEGIN
    USE master;
    ALTER DATABASE LibraryManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LibraryManagement;
END
GO
-- Tạo Database
CREATE DATABASE LibraryManagement;
GO

USE LibraryManagement;
GO

-- =====================================================
-- 1. BẢNG USERS (Quản lý đăng nhập)
-- =====================================================
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Role NVARCHAR(20) DEFAULT 'User',
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- =====================================================
-- 2. BẢNG BOOKS (Quản lý sách)
-- =====================================================
CREATE TABLE Books (
    BookID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Author NVARCHAR(100),
    Publisher NVARCHAR(100),
    PublishYear INT,
    Category NVARCHAR(50),
    Quantity INT DEFAULT 0,
    ISBN NVARCHAR(20),
    Description NVARCHAR(500)
);

-- =====================================================
-- 3. BẢNG MEMBERS (Quản lý độc giả)
-- =====================================================
CREATE TABLE Members (
    MemberID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Address NVARCHAR(200),
    JoinDate DATE DEFAULT GETDATE(),
    Status NVARCHAR(20) DEFAULT 'Active'
);

-- =====================================================
-- 4. BẢNG LOANS (Quản lý mượn/trả sách)
-- =====================================================
CREATE TABLE Loans (
    LoanID INT PRIMARY KEY IDENTITY(1,1),
    BookID INT FOREIGN KEY REFERENCES Books(BookID),
    MemberID INT FOREIGN KEY REFERENCES Members(MemberID),
    LoanDate DATE DEFAULT GETDATE(),
    DueDate DATE,
    ReturnDate DATE NULL,
    Status NVARCHAR(20) DEFAULT 'Borrowed',
    Notes NVARCHAR(200)
);

-- =====================================================
-- THÊM DỮ LIỆU MẪU
-- =====================================================

-- Thêm User mặc định
INSERT INTO Users (Username, Password, FullName, Role) 
VALUES 
('admin', 'admin123', N'Quản trị viên', 'Admin'),
('user1', '123456', N'Nguyễn Văn A', 'User');

-- Thêm sách mẫu
INSERT INTO Books (Title, Author, Publisher, PublishYear, Category, Quantity, ISBN, Description)
VALUES 
(N'Lập trình C# cơ bản', N'Nguyễn Văn A', N'NXB Giáo dục', 2023, N'Công nghệ', 10, '978-6041234567', N'Sách hướng dẫn lập trình C# từ cơ bản đến nâng cao'),
(N'XML và Web Services', N'Trần Thị B', N'NXB Thống Kê', 2022, N'Công nghệ', 5, '978-6041234568', N'Giới thiệu về XML và ứng dụng trong Web Services'),
(N'Cơ sở dữ liệu SQL Server', N'Lê Văn C', N'NXB Đại học Quốc gia', 2023, N'Công nghệ', 8, '978-6041234569', N'Hướng dẫn thiết kế và quản trị CSDL SQL Server'),
(N'Đắc nhân tâm', N'Dale Carnegie', N'NXB Tổng hợp', 2020, N'Kỹ năng sống', 15, '978-6041234570', N'Sách về kỹ năng giao tiếp và xây dựng mối quan hệ'),
(N'Tôi thấy hoa vàng trên cỏ xanh', N'Nguyễn Nhật Ánh', N'NXB Trẻ', 2021, N'Văn học', 20, '978-6041234571', N'Tiểu thuyết về tuổi thơ ở miền quê Việt Nam');

-- Thêm độc giả mẫu
INSERT INTO Members (FullName, Email, Phone, Address, Status)
VALUES 
(N'Phạm Thị Mai', 'mai.pham@email.com', '0912345678', N'123 Đường Lê Lợi, Đà Nẵng', 'Active'),
(N'Hoàng Văn Nam', 'nam.hoang@email.com', '0923456789', N'456 Đường Trần Phú, Đà Nẵng', 'Active'),
(N'Trần Thị Lan', 'lan.tran@email.com', '0934567890', N'789 Đường Nguyễn Huệ, Đà Nẵng', 'Active');

-- Thêm phiếu mượn mẫu
INSERT INTO Loans (BookID, MemberID, LoanDate, DueDate, Status)
VALUES 
(1, 1, '2024-11-01', '2024-11-15', 'Borrowed'),
(2, 2, '2024-11-05', '2024-11-19', 'Borrowed'),
(3, 3, '2024-10-20', '2024-11-03', 'Returned');

-- Cập nhật ReturnDate cho sách đã trả
UPDATE Loans SET ReturnDate = '2024-11-02' WHERE LoanID = 3;

-- =====================================================
-- KIỂM TRA DỮ LIỆU
-- =====================================================
SELECT * FROM Users;
SELECT * FROM Books;
SELECT * FROM Members;
SELECT * FROM Loans;