IF DB_ID(N'ProjectTaskDB') IS NULL
BEGIN
	CREATE DATABASE ProjectTaskDB;
END
GO

USE ProjectTaskDB;
GO

IF OBJECT_ID(N'dbo.DuAn', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.DuAn
	(
		MaDuAn int IDENTITY(1,1) NOT NULL CONSTRAINT PK_DuAn PRIMARY KEY,
		TenDuAn nvarchar(150) NOT NULL,
		NgayBatDau datetime NULL,
		NgayKetThuc datetime NULL,
		TrangThai nvarchar(50) NOT NULL CONSTRAINT DF_DuAn_TrangThai DEFAULT N'Đang chạy'
	);
END
GO

IF OBJECT_ID(N'dbo.ThanhVien', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.ThanhVien
	(
		MaThanhVien int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ThanhVien PRIMARY KEY,
		HoTen nvarchar(100) NOT NULL,
		Email varchar(100) NULL CONSTRAINT UQ_ThanhVien_Email UNIQUE,
		ChuyenMon nvarchar(100) NULL
	);
END
GO

IF OBJECT_ID(N'dbo.CongViec', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.CongViec
	(
		MaTask int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CongViec PRIMARY KEY,
		MaDuAn int NOT NULL,
		TenTask nvarchar(200) NOT NULL,
		MoTa nvarchar(max) NULL,
		HanNop datetime NULL,
		DoUuTien nvarchar(20) NOT NULL CONSTRAINT DF_CongViec_DoUuTien DEFAULT N'Trung bình',
		TrangThai nvarchar(50) NOT NULL CONSTRAINT DF_CongViec_TrangThai DEFAULT N'To Do',
		CONSTRAINT FK_CongViec_DuAn FOREIGN KEY (MaDuAn) REFERENCES dbo.DuAn(MaDuAn) ON DELETE CASCADE
	);
END
GO

IF OBJECT_ID(N'dbo.PhanCong', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.PhanCong
	(
		MaTask int NOT NULL,
		MaThanhVien int NOT NULL,
		NgayGiao datetime NOT NULL CONSTRAINT DF_PhanCong_NgayGiao DEFAULT GETDATE(),
		CONSTRAINT PK_PhanCong PRIMARY KEY (MaTask, MaThanhVien),
		CONSTRAINT FK_PhanCong_CongViec FOREIGN KEY (MaTask) REFERENCES dbo.CongViec(MaTask) ON DELETE CASCADE,
		CONSTRAINT FK_PhanCong_ThanhVien FOREIGN KEY (MaThanhVien) REFERENCES dbo.ThanhVien(MaThanhVien) ON DELETE CASCADE
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CongViec_MaDuAn_TrangThai' AND object_id = OBJECT_ID(N'dbo.CongViec'))
	CREATE INDEX IX_CongViec_MaDuAn_TrangThai ON dbo.CongViec(MaDuAn, TrangThai);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ThanhVien)
BEGIN
	INSERT INTO dbo.ThanhVien (HoTen, Email, ChuyenMon)
	VALUES (N'Nguyễn Văn An', 'an@example.com', N'Backend'),
		   (N'Trần Thị Bình', 'binh@example.com', N'Frontend');
END
GO