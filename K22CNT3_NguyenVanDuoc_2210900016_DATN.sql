create database QuanLyVot
use QuanLyVot
/* ===========================================
   1. QUAN TRI
   =========================================== */
create table QuanTri (
	ID_QT INT IDENTITY PRIMARY KEY,
    TaiKhoan NVARCHAR(100) UNIQUE NOT NULL,
    MatKhau NVARCHAR(255) NOT NULL,
    VaiTro NVARCHAR(50) NOT NULL,
     TrangThai BIT DEFAULT 1
)

/* ===========================================
   2. SAN PHAM
   =========================================== */
create table SanPham (
	ID_SP INT IDENTITY PRIMARY KEY,
    TenSP NVARCHAR(150) NOT NULL,
    ThuongHieu NVARCHAR(100),
    DonGia DECIMAL(15,2) NOT NULL,
    SoLuong INT DEFAULT 0,
    DVT NVARCHAR(50),
    BaoHanhThang INT,
    HinhAnh NVARCHAR(255),      -- ẢNH SẢN PHẨM
    MoTa NVARCHAR(MAX),
	TrangThai BIT DEFAULT 1    -- 0: ngừng bán | 1: đang bán | 2: hết hàng
)

/* ===========================================
   3. NHA CUNG CAP
   =========================================== */
create table NhaCungCap (
	ID_NCC INT IDENTITY PRIMARY KEY,
    TenNCC NVARCHAR(150) NOT NULL,
    DiaChi NVARCHAR(255),
    DienThoai VARCHAR(20),
    Email VARCHAR(100)
)

/* ===========================================
   4. PHIEU NHAP
   =========================================== */
CREATE TABLE PhieuNhap (
     ID_PhieuNhap INT IDENTITY PRIMARY KEY,
    NgayNhap DATETIME DEFAULT GETDATE(),
    ID_NCC INT,
    TongTien DECIMAL(18,2),

    FOREIGN KEY (ID_NCC) REFERENCES NhaCungCap(ID_NCC)
)

/* ===========================================
   5. CHI TIET PHIEU NHAP
   =========================================== */
CREATE TABLE ChiTietPhieuNhap (
    ID_CTPhieuNhap INT IDENTITY PRIMARY KEY,
    ID_PhieuNhap INT,
    ID_SP INT,
    SoLuong INT,
    DonGiaNhap DECIMAL(18,2),

    FOREIGN KEY (ID_PhieuNhap) REFERENCES PhieuNhap(ID_PhieuNhap),
    FOREIGN KEY (ID_SP) REFERENCES SanPham(ID_SP)
)

/* ===========================================
   6. KHACH HANG
   =========================================== */
CREATE TABLE KhachHang (
	ID_KhachHang INT IDENTITY PRIMARY KEY,
    TenKhach NVARCHAR(100) NOT NULL,
    DienThoai VARCHAR(15) NOT NULL,
    Email VARCHAR(100),
    DiaChi NVARCHAR(255),
)

/* ===========================================
   7. ĐON HANG
   =========================================== */
CREATE TABLE DonHang (
    ID_DonHang INT IDENTITY PRIMARY KEY,
    ID_KhachHang INT NOT NULL,
    NgayDat DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(18,2),
    TrangThai NVARCHAR(50), -- ChoXuLy, DangGiao, HoanThanh, Huy
    FOREIGN KEY (ID_KhachHang) REFERENCES KhachHang(ID_KhachHang)
)

/* ===========================================
   8. CHI TIET ĐON HANG
   =========================================== */
CREATE TABLE ChiTietDonHang (
    ID_CTDonHang INT IDENTITY PRIMARY KEY,
    ID_DonHang INT NOT NULL,
    ID_SP INT NOT NULL,
    SoLuong INT,
    DonGia DECIMAL(18,2),
    ThanhTien AS (SoLuong * DonGia),
    CONSTRAINT FK_CTDonHang_DonHang
    FOREIGN KEY (ID_DonHang) REFERENCES DonHang(ID_DonHang),

    CONSTRAINT FK_CTDonHang_SanPham
    FOREIGN KEY (ID_SP) REFERENCES SanPham(ID_SP)
)
/* ===========================
   9. GIO HANG
   ============================ */
CREATE TABLE GioHang (
	ID_GioHang INT IDENTITY PRIMARY KEY,
    ID_KhachHang INT NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 0,

    FOREIGN KEY (ID_KhachHang) REFERENCES KhachHang(ID_KhachHang)
);

/* ===========================
   10. GIO HANG CHI TIET
   ============================ */
CREATE TABLE GioHangChiTiet (
    ID_CTGH INT IDENTITY PRIMARY KEY,
    ID_GioHang INT NOT NULL,
    ID_SP INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(15,2) NOT NULL,  -- giá tại thời điểm mua
    ThanhTien AS (SoLuong * DonGia),

    FOREIGN KEY (ID_GioHang) REFERENCES GioHang(ID_GioHang),
    FOREIGN KEY (ID_SP) REFERENCES SanPham(ID_SP),
    UNIQUE (ID_GioHang, ID_SP) -- tránh trùng sản phẩm trong 1 giỏ
)
/* ===========================
   11. THANH TOAN
   ============================ */
CREATE TABLE ThanhToan (
    ID_ThanhToan INT IDENTITY PRIMARY KEY,
    ID_DonHang INT,
    PhuongThuc NVARCHAR(50),
    TrangThai NVARCHAR(50),
    NgayThanhToan DATETIME,

    FOREIGN KEY (ID_DonHang) REFERENCES DonHang(ID_DonHang)
);

/* ===========================
   12. LOẠI DỊCH VỤ
   ============================ */
CREATE TABLE LoaiDichVu (
    ID_LoaiDV INT IDENTITY PRIMARY KEY,
    TenLoaiDV NVARCHAR(100) NOT NULL, -- Đóng cầu, Thay cước, Vệ sinh vợt, Bảo dưỡng
    MoTa NVARCHAR(500)
)

/* ===========================
   13. DỊCH VỤ
   ============================ */
CREATE TABLE DichVu (
    ID_DichVu INT IDENTITY PRIMARY KEY,
    TenDichVu NVARCHAR(200) NOT NULL, -- 'Đóng cầu Yonex BG65', 'Thay cước Victor VBS66N'
    ID_LoaiDV INT NOT NULL,
    DonGia DECIMAL(15,2) NOT NULL,
    ThoiGianThucHien INT, -- Số phút ước tính
    TrangThai BIT DEFAULT 1,
    
    FOREIGN KEY (ID_LoaiDV) REFERENCES LoaiDichVu(ID_LoaiDV)
)

/* ===========================
   14. ĐƠN DỊCH VỤ
   ============================ */
CREATE TABLE DonDichVu (
    ID_DonDV INT IDENTITY PRIMARY KEY,
    ID_KhachHang INT,
    TenKhach NVARCHAR(100), -- Khách vãng lai không cần đăng ký
    SDT VARCHAR(15),
    NgayNhan DATETIME DEFAULT GETDATE(),
    NgayTra DATE,
    TongTien DECIMAL(18,2) DEFAULT 0,
    TrangThai NVARCHAR(50) DEFAULT 'Chờ xử lý', -- Chờ xử lý, Đang làm, Hoàn thành, Đã trả
    GhiChu NVARCHAR(500),
    
    FOREIGN KEY (ID_KhachHang) REFERENCES KhachHang(ID_KhachHang)
)

/* ===========================
   15. CHI TIẾT ĐƠN DỊCH VỤ
   ============================ */
CREATE TABLE ChiTietDonDichVu (
    ID_CTDonDV INT IDENTITY PRIMARY KEY,
    ID_DonDV INT NOT NULL,
    ID_DichVu INT NOT NULL,
    ID_SP INT, -- Vợt cần làm dịch vụ
    SoLuong INT DEFAULT 1,
    DonGia DECIMAL(15,2),
    ThanhTien AS (SoLuong * DonGia),
    ThongSoKyThuat NVARCHAR(500), -- Lực căng: 28lbs, Màu cước: Xanh...
    
    FOREIGN KEY (ID_DonDV) REFERENCES DonDichVu(ID_DonDV),
    FOREIGN KEY (ID_DichVu) REFERENCES DichVu(ID_DichVu),
    FOREIGN KEY (ID_SP) REFERENCES SanPham(ID_SP)
)
/* ===========================
   16. SÂN CẦU LÔNG
   ============================ */
CREATE TABLE SanCauLong ( 
	ID_San INT IDENTITY PRIMARY KEY,
    TenSan NVARCHAR(100) NOT NULL, -- Sân 1, Sân VIP, Sân thi đấu
    LoaiSan NVARCHAR(50), -- Tiêu chuẩn, VIP
    GiaThueTheoGio DECIMAL(15,2) NOT NULL,
    TrangThai NVARCHAR(50) DEFAULT 'Trống', -- Trống, Đang thuê, Bảo trì
    MoTa NVARCHAR(500)
)


/* ===========================
   17. ĐẶT SÂN
   ============================ */
CREATE TABLE DatSan (
    ID_DatSan INT IDENTITY PRIMARY KEY,
    ID_KhachHang INT,
    ID_San INT NOT NULL,
    NgayDat DATE NOT NULL,
    GioBatDau TIME NOT NULL,
    GioKetThuc TIME NOT NULL,
    SoGio INT,
    TongTien DECIMAL(18,2),
    TrangThai NVARCHAR(50) DEFAULT 'Đã đặt', -- Đã đặt, Đang chơi, Hoàn thành, Hủy
    TienCoc DECIMAL(15,2) DEFAULT 0,
    GhiChu NVARCHAR(500),
    
    FOREIGN KEY (ID_KhachHang) REFERENCES KhachHang(ID_KhachHang),
    FOREIGN KEY (ID_San) REFERENCES SanCauLong(ID_San)
)
/* ===========================
   18. HỘI VIÊN
   ============================ */
CREATE TABLE HoiVien (
    ID_HoiVien INT IDENTITY PRIMARY KEY,
    ID_KhachHang INT UNIQUE NOT NULL,
    MaThe VARCHAR(20) UNIQUE NOT NULL, -- CARD001
    NgayDangKy DATE DEFAULT GETDATE(),
    NgayHetHan DATE,
    CapDo NVARCHAR(50) DEFAULT 'Đồng', -- Đồng, Bạc, Vàng, Kim cương
    DiemTichLuy INT DEFAULT 0,
    UuDai DECIMAL(5,2) DEFAULT 0, -- % giảm giá
    
    FOREIGN KEY (ID_KhachHang) REFERENCES KhachHang(ID_KhachHang)
)

/* ===========================
   19. LỊCH SỬ TÍCH ĐIỂM
   ============================ */
CREATE TABLE LichSuTichDiem (
    ID_LichSu INT IDENTITY PRIMARY KEY,
    ID_HoiVien INT NOT NULL,
    ID_DonHang INT,
    ID_DonDV INT,
    DiemCong INT DEFAULT 0,
    DiemTru INT DEFAULT 0,
    LyDo NVARCHAR(200),
    NgayTichDiem DATETIME DEFAULT GETDATE(),
    
    FOREIGN KEY (ID_HoiVien) REFERENCES HoiVien(ID_HoiVien),
    FOREIGN KEY (ID_DonHang) REFERENCES DonHang(ID_DonHang),
    FOREIGN KEY (ID_DonDV) REFERENCES DonDichVu(ID_DonDV)
)
ALTER TABLE GioHang
ADD TongTien DECIMAL(18,2) NULL;




