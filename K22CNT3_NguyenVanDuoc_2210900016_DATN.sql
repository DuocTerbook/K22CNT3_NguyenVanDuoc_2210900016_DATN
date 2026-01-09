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

select * from KhachHang

ALTER TABLE GioHang
ADD TongTien DECIMAL(18,2) NULL;





drop table ThanhToan
drop table GioHangChiTiet
drop table GioHang
drop table ChiTietDonHang
drop table DonHang
drop table ChiTietPhieuNhap
drop table PhieuNhap
drop table NhaCungCap
drop table SanPham
drop table KhachHang
drop table QuanTri

