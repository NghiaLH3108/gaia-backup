-- =============================================
-- GAIA_DB - SQL Server Script
-- =============================================

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'GAIA_DB')
BEGIN
    ALTER DATABASE GAIA_DB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE GAIA_DB;
END
GO

CREATE DATABASE GAIA_DB
    COLLATE Vietnamese_CI_AS;
GO

USE GAIA_DB;
GO

-- =============================================
-- CREATE TABLES
-- =============================================

CREATE TABLE Users (
    UserId       INT IDENTITY(1,1) PRIMARY KEY,
    FullName     NVARCHAR(100)  NOT NULL,
    Phone        VARCHAR(15)    NOT NULL,
    Email        VARCHAR(100)   NOT NULL,
    PasswordHash NVARCHAR(255)  NOT NULL,
    Role         NVARCHAR(20)   NOT NULL,
    Status       NVARCHAR(20)   NOT NULL DEFAULT N'Active',
    CreatedDate  DATETIME       NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Supplier (
    SupplierId    INT IDENTITY(1,1) PRIMARY KEY,
    UserId        INT            NOT NULL,
    WarehouseName NVARCHAR(200)  NOT NULL,
    Address       NVARCHAR(255)  NOT NULL,
    Latitude      DECIMAL(10,7)  NOT NULL,
    Longitude     DECIMAL(10,7)  NOT NULL,
    CreatedDate   DATETIME       NOT NULL DEFAULT GETDATE()
);

CREATE TABLE MaterialBatch (
    BatchId           INT IDENTITY(1,1) PRIMARY KEY,
    SupplierId        INT            NOT NULL,
    BatchCode         VARCHAR(20)    NOT NULL,
    WeightKg          DECIMAL(10,2)  NOT NULL,
    CollectionAddress NVARCHAR(255)  NOT NULL,
    Latitude          DECIMAL(10,7)  NOT NULL,
    Longitude         DECIMAL(10,7)  NOT NULL,
    CollectionTime    DATETIME       NOT NULL,
    ApprovedTime      DATETIME       NULL,
    Status            NVARCHAR(30)   NOT NULL DEFAULT N'Pending',
    CreatedDate       DATETIME       NOT NULL DEFAULT GETDATE()
);

CREATE TABLE MaterialImage (
    ImageId    INT IDENTITY(1,1) PRIMARY KEY,
    BatchId    INT            NOT NULL,
    ImageUrl   NVARCHAR(255)  NOT NULL,
    UploadTime DATETIME       NOT NULL DEFAULT GETDATE()
);

CREATE TABLE TransportationHistory (
    HistoryId   INT IDENTITY(1,1) PRIMARY KEY,
    BatchId     INT            NOT NULL,
    Status      NVARCHAR(30)   NOT NULL,
    Description NVARCHAR(255)  NOT NULL,
    UpdateTime  DATETIME       NOT NULL
);

CREATE TABLE Product (
    ProductId     INT IDENTITY(1,1) PRIMARY KEY,
    BatchId       INT            NOT NULL,
    ProductName   NVARCHAR(200)  NOT NULL,
    Description   NVARCHAR(500)  NOT NULL,
    QRToken       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    CurrentStatus NVARCHAR(30)   NOT NULL DEFAULT N'Available',
    CreatedDate   DATETIME       NOT NULL DEFAULT GETDATE()
);

CREATE TABLE ProductTimeline (
    TimelineId   INT IDENTITY(1,1) PRIMARY KEY,
    ProductId    INT            NOT NULL,
    StepOrder    INT            NOT NULL,
    Title        NVARCHAR(100)  NOT NULL,
    Description  NVARCHAR(500)  NOT NULL,
    Location     NVARCHAR(255)  NOT NULL,
    Latitude     DECIMAL(10,7)  NOT NULL,
    Longitude    DECIMAL(10,7)  NOT NULL,
    ImageUrl     NVARCHAR(255)  NOT NULL,
    VideoUrl     NVARCHAR(255)  NOT NULL,
    TimelineTime DATETIME       NOT NULL
);

CREATE TABLE Story (
    StoryId      INT IDENTITY(1,1) PRIMARY KEY,
    ProductId    INT            NOT NULL,
    Title        NVARCHAR(200)  NOT NULL,
    Content      NVARCHAR(MAX)  NOT NULL,
    DisplayOrder INT            NOT NULL
);

-- =============================================
-- ALTER TABLE ADD CONSTRAINTS
-- =============================================

ALTER TABLE Users
    ADD CONSTRAINT CHK_Users_Role   CHECK (Role   IN (N'Supplier', N'Admin')),
        CONSTRAINT CHK_Users_Status CHECK (Status IN (N'Active', N'Inactive')),
        CONSTRAINT UQ_Users_Email   UNIQUE (Email),
        CONSTRAINT UQ_Users_Phone   UNIQUE (Phone);

ALTER TABLE Supplier
    ADD CONSTRAINT FK_Supplier_Users  FOREIGN KEY (UserId)     REFERENCES Users(UserId),
        CONSTRAINT UQ_Supplier_UserId UNIQUE (UserId);

ALTER TABLE MaterialBatch
    ADD CONSTRAINT FK_Batch_Supplier  FOREIGN KEY (SupplierId) REFERENCES Supplier(SupplierId),
        CONSTRAINT CHK_Batch_Weight   CHECK (WeightKg > 0),
        CONSTRAINT CHK_Batch_Status   CHECK (Status IN (N'Pending', N'Approved', N'Rejected', N'Transporting', N'ArrivedFactory')),
        CONSTRAINT UQ_Batch_Code      UNIQUE (BatchCode);

ALTER TABLE MaterialImage
    ADD CONSTRAINT FK_Image_Batch     FOREIGN KEY (BatchId)    REFERENCES MaterialBatch(BatchId);

ALTER TABLE TransportationHistory
    ADD CONSTRAINT FK_Transport_Batch FOREIGN KEY (BatchId)    REFERENCES MaterialBatch(BatchId),
        CONSTRAINT CHK_Transport_Status CHECK (Status IN (N'Collected', N'Loaded', N'On the way', N'Arrived Factory'));

ALTER TABLE Product
    ADD CONSTRAINT FK_Product_Batch   FOREIGN KEY (BatchId)    REFERENCES MaterialBatch(BatchId),
        CONSTRAINT CHK_Product_Status CHECK (CurrentStatus IN (N'Available', N'Sold', N'Reserved'));

ALTER TABLE ProductTimeline
    ADD CONSTRAINT FK_Timeline_Product FOREIGN KEY (ProductId)  REFERENCES Product(ProductId) ON DELETE CASCADE,
        CONSTRAINT CHK_Timeline_Step   CHECK (StepOrder IN (1, 2, 3));

ALTER TABLE Story
    ADD CONSTRAINT FK_Story_Product    FOREIGN KEY (ProductId)  REFERENCES Product(ProductId) ON DELETE CASCADE,
        CONSTRAINT CHK_Story_Order     CHECK (DisplayOrder BETWEEN 1 AND 5);

-- =============================================
-- INDEXES
-- =============================================

CREATE INDEX IDX_Batch_SupplierId      ON MaterialBatch(SupplierId);
CREATE INDEX IDX_Image_BatchId         ON MaterialImage(BatchId);
CREATE INDEX IDX_Transport_BatchId     ON TransportationHistory(BatchId);
CREATE INDEX IDX_Product_BatchId       ON Product(BatchId);
CREATE INDEX IDX_Timeline_ProductId    ON ProductTimeline(ProductId);
CREATE INDEX IDX_Story_ProductId       ON Story(ProductId);
CREATE UNIQUE INDEX UQ_Product_QRToken ON Product(QRToken);

-- =============================================
-- INSERT Users (8 Supplier + 2 Admin)
-- =============================================

SET IDENTITY_INSERT Users ON;

INSERT INTO Users (UserId, FullName, Phone, Email, PasswordHash, Role, Status, CreatedDate) VALUES
(1,  N'Nguyễn Văn An',      '0901234561', 'an.nguyen@gaia.vn',       '$2a$12$hVnT1KqB8sP9mRjL0dEw3O', N'Supplier', N'Active',   '2024-01-10 08:00:00'),
(2,  N'Trần Thị Bình',      '0901234562', 'binh.tran@gaia.vn',       '$2a$12$hVnT1KqB8sP9mRjL0dEw3O', N'Supplier', N'Active',   '2024-01-12 08:30:00'),
(3,  N'Lê Minh Cường',      '0901234563', 'cuong.le@gaia.vn',        '$2a$12$hVnT1KqB8sP9mRjL0dEw3O', N'Supplier', N'Active',   '2024-01-15 09:00:00'),
(4,  N'Phạm Thị Dung',      '0901234564', 'dung.pham@gaia.vn',       '$2a$12$hVnT1KqB8sP9mRjL0dEw3O', N'Supplier', N'Active',   '2024-01-18 09:30:00'),
(5,  N'Hoàng Văn Em',       '0901234565', 'em.hoang@gaia.vn',        '$2a$12$hVnT1KqB8sP9mRjL0dEw3O', N'Supplier', N'Active',   '2024-01-20 10:00:00'),
(6,  N'Vũ Thị Phương',      '0901234566', 'phuong.vu@gaia.vn',       '$2a$12$hVnT1KqB8sP9mRjL0dEw3O', N'Supplier', N'Active',   '2024-01-22 10:30:00'),
(7,  N'Đặng Quốc Hùng',     '0901234567', 'hung.dang@gaia.vn',       '$2a$12$hVnT1KqB8sP9mRjL0dEw3O', N'Supplier', N'Active',   '2024-01-25 11:00:00'),
(8,  N'Bùi Thị Lan',        '0901234568', 'lan.bui@gaia.vn',         '$2a$12$hVnT1KqB8sP9mRjL0dEw3O', N'Supplier', N'Inactive', '2024-01-28 11:30:00'),
(9,  N'Nguyễn Thị Mai',     '0901234569', 'mai.admin@gaia.vn',       '$2a$12$hVnT1KqB8sP9mRjL0dEw3O', N'Admin',    N'Active',   '2024-01-05 07:00:00'),
(10, N'Trần Quang Khải',    '0901234570', 'khai.admin@gaia.vn',      '$2a$12$hVnT1KqB8sP9mRjL0dEw3O', N'Admin',    N'Active',   '2024-01-05 07:30:00');

SET IDENTITY_INSERT Users OFF;

-- =============================================
-- INSERT Supplier
-- =============================================

SET IDENTITY_INSERT Supplier ON;

INSERT INTO Supplier (SupplierId, UserId, WarehouseName, Address, Latitude, Longitude, CreatedDate) VALUES
(1, 1, N'Vựa Sầu Riêng An Phú',        N'Ấp An Phú, Xã Tam Bình, Huyện Cai Lậy, Tiền Giang',           10.4586234, 106.0123456, '2024-01-10 08:30:00'),
(2, 2, N'Vựa Sầu Riêng Bình Thủy',     N'Ấp Bình Thủy, Xã Phú Phụng, Huyện Chợ Lách, Bến Tre',         10.1823456, 106.1234567, '2024-01-12 09:00:00'),
(3, 3, N'Nhà Vườn Cường Long Hồ',      N'Ấp Long Phụng, Xã Long Phụng, Huyện Long Hồ, Vĩnh Long',       10.2234567, 105.9876543, '2024-01-15 09:30:00'),
(4, 4, N'Vựa Trái Cây Dung Phong Điền',N'Ấp Tân Lộc, Xã Tân Thới, Huyện Phong Điền, Cần Thơ',          10.0345678, 105.7654321, '2024-01-18 10:00:00'),
(5, 5, N'Trang Trại Em Định Quán',      N'Thôn Phú Lộc, Xã Phú Lộc, Huyện Định Quán, Đồng Nai',         11.2456789, 107.3456789, '2024-01-20 10:30:00'),
(6, 6, N'Hợp Tác Xã Phương Ea', N'Thôn Ea Nam, Xã Ea Nam, Đắk Lắk',             13.0567890, 108.0567890, '2024-01-22 11:00:00'),
(7, 7, N'Vườn Hùng Di Linh',           N'Thôn Tân Hà, Xã Tân Châu, Huyện Di Linh, Lâm Đồng',            11.5678901, 108.0678901, '2024-01-25 11:30:00'),
(8, 8, N'Nhà Vườn Lan Bù Đốp',         N'Thôn Bình Thắng, Xã Thanh Hòa, Huyện Bù Đốp, Bình Phước',     11.8789012, 106.8789012, '2024-01-28 12:00:00');

SET IDENTITY_INSERT Supplier OFF;

-- =============================================
-- INSERT MaterialBatch
-- =============================================

SET IDENTITY_INSERT MaterialBatch ON;

INSERT INTO MaterialBatch (BatchId, SupplierId, BatchCode, WeightKg, CollectionAddress, Latitude, Longitude, CollectionTime, ApprovedTime, Status, CreatedDate) VALUES
(1,  1, 'BAT001', 850.00,  N'Ấp An Phú, Xã Tam Bình, Cai Lậy, Tiền Giang',           10.4586234, 106.0123456, '2024-02-01 06:00:00', '2024-02-02 08:00:00', N'ArrivedFactory', '2024-02-01 06:30:00'),
(2,  2, 'BAT002', 620.50,  N'Ấp Bình Thủy, Xã Phú Phụng, Chợ Lách, Bến Tre',         10.1823456, 106.1234567, '2024-02-03 06:00:00', '2024-02-04 09:00:00', N'ArrivedFactory', '2024-02-03 06:30:00'),
(3,  3, 'BAT003', 1100.75, N'Ấp Long Phụng, Xã Long Phụng, Long Hồ, Vĩnh Long',       10.2234567, 105.9876543, '2024-02-05 06:30:00', '2024-02-06 08:30:00', N'ArrivedFactory', '2024-02-05 07:00:00'),
(4,  4, 'BAT004', 430.00,  N'Ấp Tân Lộc, Xã Tân Thới, Phong Điền, Cần Thơ',          10.0345678, 105.7654321, '2024-02-07 07:00:00', '2024-02-08 09:30:00', N'ArrivedFactory', '2024-02-07 07:30:00'),
(5,  5, 'BAT005', 975.25,  N'Thôn Phú Lộc, Xã Phú Lộc, Định Quán, Đồng Nai',         11.2456789, 107.3456789, '2024-02-10 06:00:00', '2024-02-11 08:00:00', N'Transporting',   '2024-02-10 06:30:00'),
(6,  6, 'BAT006', 1200.00, N'Thôn Ea Nam, Xã Ea Nam, Đắk Lắk',             13.0567890, 108.0567890, '2024-02-12 05:30:00', '2024-02-13 09:00:00', N'Approved',       '2024-02-12 06:00:00'),
(7,  7, 'BAT007', 560.00,  N'Thôn Tân Hà, Xã Tân Châu, Di Linh, Lâm Đồng',           11.5678901, 108.0678901, '2024-02-14 07:00:00', NULL,                  N'Pending',        '2024-02-14 07:30:00'),
(8,  8, 'BAT008', 380.50,  N'Thôn Bình Thắng, Xã Thanh Hòa, Bù Đốp, Bình Phước',     11.8789012, 106.8789012, '2024-02-15 06:00:00', NULL,                  N'Rejected',       '2024-02-15 06:30:00'),
(9,  1, 'BAT009', 700.00,  N'Ấp An Phú, Xã Tam Bình, Cai Lậy, Tiền Giang',           10.4586234, 106.0123456, '2024-02-18 06:00:00', '2024-02-19 08:00:00', N'ArrivedFactory', '2024-02-18 06:30:00'),
(10, 2, 'BAT010', 890.00,  N'Ấp Bình Thủy, Xã Phú Phụng, Chợ Lách, Bến Tre',         10.1823456, 106.1234567, '2024-02-20 06:00:00', '2024-02-21 09:00:00', N'ArrivedFactory', '2024-02-20 06:30:00');

SET IDENTITY_INSERT MaterialBatch OFF;

-- =============================================
-- INSERT MaterialImage
-- =============================================

SET IDENTITY_INSERT MaterialImage ON;

INSERT INTO MaterialImage (ImageId, BatchId, ImageUrl, UploadTime) VALUES
(1,  1,  '/images/batch1_1.jpg',  '2024-02-01 07:00:00'),
(2,  1,  '/images/batch1_2.jpg',  '2024-02-01 07:05:00'),
(3,  2,  '/images/batch2_1.jpg',  '2024-02-03 07:00:00'),
(4,  3,  '/images/batch3_1.jpg',  '2024-02-05 07:30:00'),
(5,  3,  '/images/batch3_2.jpg',  '2024-02-05 07:35:00'),
(6,  4,  '/images/batch4_1.jpg',  '2024-02-07 08:00:00'),
(7,  5,  '/images/batch5_1.jpg',  '2024-02-10 07:00:00'),
(8,  6,  '/images/batch6_1.jpg',  '2024-02-12 06:30:00'),
(9,  9,  '/images/batch9_1.jpg',  '2024-02-18 07:00:00'),
(10, 10, '/images/batch10_1.jpg', '2024-02-20 07:00:00');

SET IDENTITY_INSERT MaterialImage OFF;

-- =============================================
-- INSERT TransportationHistory (~35 records)
-- =============================================

SET IDENTITY_INSERT TransportationHistory ON;

INSERT INTO TransportationHistory (HistoryId, BatchId, Status, Description, UpdateTime) VALUES
-- Batch 1
(1,  1, N'Collected',       N'Đã thu gom vỏ sầu riêng tại vựa An Phú, Tiền Giang',         '2024-02-01 06:30:00'),
(2,  1, N'Loaded',          N'Đã xếp hàng lên xe tải, chuẩn bị xuất phát',                  '2024-02-01 08:00:00'),
(3,  1, N'On the way',      N'Xe đang vận chuyển về nhà máy GAIA tại TP. Hồ Chí Minh',      '2024-02-01 09:30:00'),
(4,  1, N'Arrived Factory', N'Lô hàng đã về tới nhà máy, đang tiến hành kiểm tra chất lượng','2024-02-02 07:30:00'),
-- Batch 2
(5,  2, N'Collected',       N'Đã thu gom vỏ sầu riêng tại vựa Bình Thủy, Bến Tre',          '2024-02-03 06:30:00'),
(6,  2, N'Loaded',          N'Hàng đã được đóng gói và đưa lên xe vận chuyển',              '2024-02-03 08:30:00'),
(7,  2, N'On the way',      N'Xe đang trên đường từ Bến Tre về TP. Hồ Chí Minh',            '2024-02-03 10:00:00'),
(8,  2, N'Arrived Factory', N'Lô hàng BAT002 đã cập bến nhà máy, sẵn sàng sơ chế',         '2024-02-04 08:00:00'),
-- Batch 3
(9,  3, N'Collected',       N'Đã thu gom đủ 1.100 kg vỏ sầu riêng tại Long Hồ, Vĩnh Long', '2024-02-05 07:00:00'),
(10, 3, N'Loaded',          N'Hàng đã lên xe container, cân đủ trọng lượng',                '2024-02-05 09:00:00'),
(11, 3, N'On the way',      N'Xe container đang di chuyển qua phà Mỹ Thuận',                '2024-02-05 11:00:00'),
(12, 3, N'Arrived Factory', N'Lô hàng lớn nhất tháng 2 đã về tới nhà máy an toàn',         '2024-02-06 07:30:00'),
-- Batch 4
(13, 4, N'Collected',       N'Thu gom vỏ sầu riêng tại Phong Điền, Cần Thơ',               '2024-02-07 07:30:00'),
(14, 4, N'Loaded',          N'Đã đóng hàng và lên xe, xuất phát lúc 10 giờ sáng',          '2024-02-07 10:00:00'),
(15, 4, N'On the way',      N'Xe đang trên cao tốc hướng về TP. Hồ Chí Minh',              '2024-02-07 12:00:00'),
(16, 4, N'Arrived Factory', N'Hàng về xưởng, nhân viên đã nhận và ký biên bản',            '2024-02-08 08:30:00'),
-- Batch 5
(17, 5, N'Collected',       N'Thu gom vỏ sầu riêng tại trang trại Định Quán, Đồng Nai',    '2024-02-10 06:30:00'),
(18, 5, N'Loaded',          N'Hàng đã được xếp lên xe, xuất phát lúc 9 giờ',              '2024-02-10 09:00:00'),
(19, 5, N'On the way',      N'Xe đang vận chuyển, dự kiến về xưởng lúc 14 giờ',            '2024-02-10 11:00:00'),
-- Batch 6
(20, 6, N'Collected',       N'Thu gom vỏ sầu riêng tại HTX Ea, Đắk Lắk',           '2024-02-12 06:00:00'),
(21, 6, N'Loaded',          N'Xe tải lớn đã chất đủ 1.200 kg hàng, sẵn sàng khởi hành',   '2024-02-12 08:00:00'),
-- Batch 7
(22, 7, N'Collected',       N'Đã tiếp nhận vỏ sầu riêng tại vườn Di Linh, Lâm Đồng',      '2024-02-14 07:30:00'),
-- Batch 8
(23, 8, N'Collected',       N'Thu gom tại Bù Đốp, Bình Phước – phát hiện độ ẩm cao',       '2024-02-15 06:30:00'),
-- Batch 9
(24, 9, N'Collected',       N'Thu gom đợt 2 tại vựa An Phú, Tiền Giang',                   '2024-02-18 06:30:00'),
(25, 9, N'Loaded',          N'Hàng đã lên xe, xuất phát lúc 8 giờ',                        '2024-02-18 08:00:00'),
(26, 9, N'On the way',      N'Xe đang di chuyển theo tuyến quốc lộ 1A',                    '2024-02-18 10:00:00'),
(27, 9, N'Arrived Factory', N'Lô hàng BAT009 đã về đến nhà máy, kiểm tra đạt chất lượng',  '2024-02-19 07:00:00'),
-- Batch 10
(28, 10, N'Collected',      N'Thu gom vỏ sầu riêng đợt 2 tại Chợ Lách, Bến Tre',          '2024-02-20 06:30:00'),
(29, 10, N'Loaded',         N'Hàng đã được xếp gọn lên xe tải, cân đủ 890 kg',             '2024-02-20 08:30:00'),
(30, 10, N'On the way',     N'Xe đang trên đường về nhà máy, qua phà Rạch Miễu',           '2024-02-20 10:30:00'),
(31, 10, N'Arrived Factory',N'Lô hàng BAT010 về xưởng, bàn giao cho bộ phận sơ chế',      '2024-02-21 08:00:00');

SET IDENTITY_INSERT TransportationHistory OFF;

-- =============================================
-- INSERT Product (10 sản phẩm, dùng batch đã ArrivedFactory)
-- =============================================

SET IDENTITY_INSERT Product ON;

INSERT INTO Product (ProductId, BatchId, ProductName, Description, QRToken, CurrentStatus, CreatedDate) VALUES
(1,  1, N'Chậu cây GAIA Mini',        N'Chậu trồng cây mini được ép từ vỏ sầu riêng tự nhiên. Thân thiện môi trường, có khả năng phân hủy sinh học sau 2 năm sử dụng. Phù hợp trồng xương rồng, cây thủy sinh cỡ nhỏ.',                           NEWID(), N'Available', '2024-02-10 08:00:00'),
(2,  1, N'Lót ly GAIA Tròn',          N'Lót ly hình tròn đường kính 10cm, dày 5mm, làm từ xơ vỏ sầu riêng ép nhiệt. Bề mặt nhám tự nhiên giữ nhiệt tốt, thiết kế tối giản phù hợp mọi không gian.',                                             NEWID(), N'Available', '2024-02-10 08:30:00'),
(3,  2, N'Khay trang trí đa năng',    N'Khay hình chữ nhật 20x15cm từ vỏ sầu riêng nghiền mịn. Dùng đựng đồ dùng bàn làm việc, khay trà nhỏ hoặc trang trí nội thất. Bề mặt được phủ lớp dầu thực vật bảo vệ tự nhiên.',                        NEWID(), N'Sold',      '2024-02-12 08:00:00'),
(4,  2, N'Hộp bút sinh học GAIA',     N'Hộp bút hình trụ cao 12cm, ép từ hỗn hợp vỏ sầu riêng và tinh bột sắn. Chịu lực tốt, không thấm nước nhẹ, phù hợp để bàn học sinh và văn phòng hiện đại.',                                              NEWID(), N'Available', '2024-02-12 09:00:00'),
(5,  3, N'Đế nến thiên nhiên',        N'Đế đặt nến hình tròn đường kính 8cm từ bã vỏ sầu riêng. Chịu nhiệt ổn định, tôn lên vẻ đẹp mộc mạc của không gian thư giãn. Kèm lớp silicone chống trơn ở đáy.',                                        NEWID(), N'Available', '2024-02-15 08:00:00'),
(6,  3, N'Khung ảnh GAIA Rustic',     N'Khung ảnh 10x15cm mang phong cách rustic từ vỏ sầu riêng ép. Mặt lưng có gờ đứng và móc treo. Tặng kèm giấy nền họa tiết lá tự nhiên.',                                                                  NEWID(), N'Reserved',  '2024-02-15 09:00:00'),
(7,  4, N'Bộ lót nồi GAIA Set 3',     N'Bộ 3 lót nồi kích thước S/M/L từ vỏ sầu riêng ép dày. Chịu nhiệt đến 180°C, cách nhiệt hiệu quả. Phù hợp bếp gia đình muốn giảm thiểu đồ nhựa trong bếp.',                                             NEWID(), N'Available', '2024-02-18 08:00:00'),
(8,  4, N'Móc khóa GAIA',             N'Móc khóa nhỏ gọn hình lá sầu riêng cách điệu, kích thước 4x2cm, dày 5mm. Được ép định hình bằng khuôn thép, bề mặt đánh bóng thủ công. Quà tặng ý nghĩa.',                                              NEWID(), N'Available', '2024-02-18 09:00:00'),
(9,  9, N'Tấm lót bàn phím GAIA',     N'Tấm lót bàn phím 45x20cm từ vỏ sầu riêng cán mỏng ép nhiệt. Nhẹ, bền, chống trơn trượt. Mang lại cảm giác gõ phím tự nhiên và giảm tiếng ồn.',                                                          NEWID(), N'Available', '2024-02-22 08:00:00'),
(10, 10, N'Bình cắm hoa GAIA Mini',   N'Bình cắm hoa nhỏ cao 10cm, đường kính miệng 5cm từ vỏ sầu riêng ép dày và phủ nhựa sinh học bên trong chống thấm nước. Phù hợp cắm hoa tươi và hoa khô.',                                                NEWID(), N'Available', '2024-02-25 08:00:00');

SET IDENTITY_INSERT Product OFF;

-- =============================================
-- INSERT ProductTimeline (10 sản phẩm x 3 bước = 30 records)
-- =============================================

SET IDENTITY_INSERT ProductTimeline ON;

INSERT INTO ProductTimeline (TimelineId, ProductId, StepOrder, Title, Description, Location, Latitude, Longitude, ImageUrl, VideoUrl, TimelineTime) VALUES
-- Product 1 - Chậu cây GAIA Mini
(1,  1, 1, N'Thu gom',  N'Vỏ sầu riêng sau thu hoạch được thu gom tại vựa An Phú, huyện Cai Lậy. Bà con nông dân tách vỏ thủ công, giữ lại phần xơ dày làm nguyên liệu.',             N'Vựa An Phú, Cai Lậy, Tiền Giang',     10.4586234, 106.0123456, '/images/timeline1_1.jpg', '/videos/process_collect.mp4', '2024-02-01 06:00:00'),
(2,  1, 2, N'Sơ chế',  N'Vỏ sầu riêng được rửa sạch, phơi khô dưới nắng 2 ngày, sau đó nghiền thành bột thô. Công đoạn sơ chế đảm bảo loại bỏ vi khuẩn và độ ẩm dư thừa.',         N'Nhà máy GAIA, Quận 9, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline1_2.jpg', '/videos/process_clean.mp4',   '2024-02-04 08:00:00'),
(3,  1, 3, N'Ép khuôn', N'Bột vỏ sầu riêng trộn cùng chất kết dính sinh học, đổ vào khuôn chậu cây, ép nhiệt ở 160°C trong 8 phút. Sản phẩm được kiểm tra độ bền trước khi đóng gói.',N'Xưởng ép khuôn GAIA, TP. Hồ Chí Minh',  10.8141597, 106.7743977, '/images/timeline1_3.jpg', '/videos/process_mold.mp4',    '2024-02-08 10:00:00'),
-- Product 2 - Lót ly GAIA Tròn
(4,  2, 1, N'Thu gom',  N'Vỏ sầu riêng chín được thu gom ngay sau khi mùa vụ kết thúc tại Cai Lậy. Bộ phận thu mua GAIA phân loại tại chỗ, chọn phần vỏ dày đều.',                   N'Vựa An Phú, Cai Lậy, Tiền Giang',     10.4586234, 106.0123456, '/images/timeline2_1.jpg', '/videos/process_collect.mp4', '2024-02-01 07:00:00'),
(5,  2, 2, N'Sơ chế',  N'Nguyên liệu được ngâm nước vôi khử khuẩn 30 phút, sau đó sấy khô ở 80°C. Bột xơ được rây qua lưới mịn để đạt độ đồng đều cần thiết.',                    N'Nhà máy GAIA, Quận 9, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline2_2.jpg', '/videos/process_clean.mp4',   '2024-02-04 09:00:00'),
(6,  2, 3, N'Ép khuôn', N'Hỗn hợp bột được đổ vào khuôn lót ly tròn, ép nhiệt định hình. Bề mặt được đánh nhám nhẹ để tạo ma sát giữ ly. Sản phẩm qua kiểm định chất lượng.',     N'Xưởng ép khuôn GAIA, TP. Hồ Chí Minh',  10.8141597, 106.7743977, '/images/timeline2_3.jpg', '/videos/process_mold.mp4',    '2024-02-08 11:00:00'),
-- Product 3 - Khay trang trí đa năng
(7,  3, 1, N'Thu gom',  N'Vỏ sầu riêng Ri6 từ vùng Chợ Lách nổi tiếng được thu gom theo hợp đồng với hộ bà Trần Thị Bình. Nguyên liệu tươi, vỏ dày đều, xơ chắc.',                 N'Vựa Bình Thủy, Chợ Lách, Bến Tre',    10.1823456, 106.1234567, '/images/timeline3_1.jpg', '/videos/process_collect.mp4', '2024-02-03 06:00:00'),
(8,  3, 2, N'Sơ chế',  N'Vỏ được tách xơ, loại bỏ phần gai nhọn, rửa sạch và phơi nắng. Phần xơ đạt tiêu chuẩn độ ẩm dưới 12% mới đưa vào dây chuyền nghiền.',                  N'Nhà máy GAIA, Quận 9, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline3_2.jpg', '/videos/process_clean.mp4',   '2024-02-06 08:00:00'),
(9,  3, 3, N'Ép khuôn', N'Bột xơ sầu riêng ép thành khay chữ nhật dưới áp suất 50 tấn. Bề mặt được phủ dầu lanh tự nhiên để chống thấm nhẹ. Sản phẩm hoàn thiện sau 15 phút.',  N'Xưởng ép khuôn GAIA, TP. Hồ Chí Minh',  10.8141597, 106.7743977, '/images/timeline3_3.jpg', '/videos/process_mold.mp4',    '2024-02-09 09:00:00'),
-- Product 4 - Hộp bút sinh học GAIA
(10, 4, 1, N'Thu gom',  N'Thu gom vỏ sầu riêng từ vựa Chợ Lách theo đợt thứ hai trong tháng. Lô nguyên liệu này có phần vỏ dày hơn trung bình, rất phù hợp tạo hình trụ.',         N'Vựa Bình Thủy, Chợ Lách, Bến Tre',    10.1823456, 106.1234567, '/images/timeline4_1.jpg', '/videos/process_collect.mp4', '2024-02-03 07:00:00'),
(11, 4, 2, N'Sơ chế',  N'Xơ vỏ sầu riêng được xay nhỏ, trộn thêm 20% tinh bột sắn để tăng độ kết dính. Hỗn hợp được ủ 12 giờ cho nguyên liệu ngấm đều trước khi ép.',           N'Nhà máy GAIA, Quận 9, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline4_2.jpg', '/videos/process_clean.mp4',   '2024-02-06 09:00:00'),
(12, 4, 3, N'Ép khuôn', N'Khuôn hộp bút hình trụ được làm nóng sẵn. Hỗn hợp nguyên liệu được đổ vào, ép và giữ nhiệt 10 phút. Sau làm nguội, sản phẩm được tháo khuôn và kiểm tra.', N'Xưởng ép khuôn GAIA, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline4_3.jpg', '/videos/process_mold.mp4',    '2024-02-09 10:00:00'),
-- Product 5 - Đế nến thiên nhiên
(13, 5, 1, N'Thu gom',  N'Nguyên liệu đến từ vùng sầu riêng Long Hồ, Vĩnh Long – giống Monthong nổi tiếng vỏ dày và thơm. Nông dân thu gom ngay trong ngày để giữ độ tươi.',       N'Ấp Long Phụng, Long Hồ, Vĩnh Long',   10.2234567, 105.9876543, '/images/timeline5_1.jpg', '/videos/process_collect.mp4', '2024-02-05 06:30:00'),
(14, 5, 2, N'Sơ chế',  N'Vỏ sầu riêng được rửa qua 3 lần nước, sau đó nghiền thô và sàng lọc. Xơ đạt tiêu chuẩn được sấy bằng máy sấy công nghiệp xuống độ ẩm 10%.',            N'Nhà máy GAIA, Quận 9, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline5_2.jpg', '/videos/process_clean.mp4',   '2024-02-08 08:00:00'),
(15, 5, 3, N'Ép khuôn', N'Đế nến được ép từ khuôn silicon chịu nhiệt, tạo hình tròn đều. Mặt dưới dán silicone chống trượt. Sản phẩm được kiểm tra chịu nhiệt thực tế với nến thật.', N'Xưởng ép khuôn GAIA, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline5_3.jpg', '/videos/process_mold.mp4',    '2024-02-12 10:00:00'),
-- Product 6 - Khung ảnh GAIA Rustic
(16, 6, 1, N'Thu gom',  N'Lô vỏ sầu riêng Ri6 Long Hồ được tuyển chọn kỹ, ưu tiên phần vỏ dày không có vết nấm. GAIA ký kết thu mua định kỳ hàng tuần với hộ ông Lê Minh Cường.',   N'Ấp Long Phụng, Long Hồ, Vĩnh Long',   10.2234567, 105.9876543, '/images/timeline6_1.jpg', '/videos/process_collect.mp4', '2024-02-05 07:30:00'),
(17, 6, 2, N'Sơ chế',  N'Xơ vỏ được nghiền thành bột mịn cỡ hạt 0.5mm. Thêm 15% keo sinh học từ tinh bột ngô. Hỗn hợp trộn đều và cán thành tấm phẳng cho công đoạn ép khung.',  N'Nhà máy GAIA, Quận 9, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline6_2.jpg', '/videos/process_clean.mp4',   '2024-02-08 09:00:00'),
(18, 6, 3, N'Ép khuôn', N'Tấm nguyên liệu được cắt và ghép vào khuôn khung ảnh, ép định hình 12 phút. Mặt sau gắn móc treo inox. Sản phẩm đóng gói kèm hướng dẫn tái chế.',     N'Xưởng ép khuôn GAIA, TP. Hồ Chí Minh',  10.8141597, 106.7743977, '/images/timeline6_3.jpg', '/videos/process_mold.mp4',    '2024-02-12 11:00:00'),
-- Product 7 - Bộ lót nồi GAIA Set 3
(19, 7, 1, N'Thu gom',  N'Thu gom vỏ sầu riêng tại Phong Điền, Cần Thơ – vùng có vườn sầu riêng ven sông nổi tiếng. Nguyên liệu được chọn lọc theo tiêu chí vỏ dày trên 2cm.',     N'Ấp Tân Lộc, Phong Điền, Cần Thơ',     10.0345678, 105.7654321, '/images/timeline7_1.jpg', '/videos/process_collect.mp4', '2024-02-07 07:00:00'),
(20, 7, 2, N'Sơ chế',  N'Vỏ nghiền thành hạt thô, trộn với xơ dừa nghiền để tăng khả năng chịu nhiệt. Hỗn hợp được kiểm tra nhiệt độ chịu đựng trước khi đưa vào ép.',           N'Nhà máy GAIA, Quận 9, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline7_2.jpg', '/videos/process_clean.mp4',   '2024-02-10 08:00:00'),
(21, 7, 3, N'Ép khuôn', N'Ba kích cỡ lót nồi được ép cùng lúc trên 3 khuôn riêng biệt ở 180°C. Bộ sản phẩm hoàn thiện được thử nghiệm chịu nhiệt thực tế trước khi xuất xưởng.',  N'Xưởng ép khuôn GAIA, TP. Hồ Chí Minh',  10.8141597, 106.7743977, '/images/timeline7_3.jpg', '/videos/process_mold.mp4',    '2024-02-14 10:00:00'),
-- Product 8 - Móc khóa GAIA
(22, 8, 1, N'Thu gom',  N'Nguyên liệu lấy từ cùng lô thu gom Phong Điền, tận dụng phần vỏ nhỏ còn lại sau khi chọn nguyên liệu cho bộ lót nồi. Giảm thiểu lãng phí tối đa.',      N'Ấp Tân Lộc, Phong Điền, Cần Thơ',     10.0345678, 105.7654321, '/images/timeline8_1.jpg', '/videos/process_collect.mp4', '2024-02-07 07:30:00'),
(23, 8, 2, N'Sơ chế',  N'Phần vỏ nhỏ được nghiền siêu mịn, trộn nhựa sinh học và tạo thành hỗn hợp paste. Paste được đổ khuôn và sấy sơ bộ để chuẩn bị cho công đoạn ép.',       N'Nhà máy GAIA, Quận 9, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline8_2.jpg', '/videos/process_clean.mp4',   '2024-02-10 09:00:00'),
(24, 8, 3, N'Ép khuôn', N'Khuôn hình lá sầu riêng cách điệu được ép chính xác bằng máy CNC. Sau đó thợ thủ công đánh bóng từng chiếc móc khóa. Gắn khoen inox và kiểm định xong.', N'Xưởng ép khuôn GAIA, TP. Hồ Chí Minh',  10.8141597, 106.7743977, '/images/timeline8_3.jpg', '/videos/process_mold.mp4',    '2024-02-14 11:00:00'),
-- Product 9 - Tấm lót bàn phím GAIA
(25, 9, 1, N'Thu gom',  N'Lô nguyên liệu đợt 2 từ vựa An Phú, Tiền Giang. Vỏ sầu riêng mùa vụ chính chất lượng cao, xơ mịn và đồng đều, lý tưởng để cán thành tấm phẳng.',       N'Vựa An Phú, Cai Lậy, Tiền Giang',     10.4586234, 106.0123456, '/images/timeline9_1.jpg', '/videos/process_collect.mp4', '2024-02-18 06:00:00'),
(26, 9, 2, N'Sơ chế',  N'Nguyên liệu nghiền mịn và cán thành tấm dày 4mm, rộng 50cm. Tấm nguyên liệu được sấy phẳng, tránh cong vênh, sau đó cắt đúng kích thước theo thiết kế.',  N'Nhà máy GAIA, Quận 9, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline9_2.jpg', '/videos/process_clean.mp4',   '2024-02-21 08:00:00'),
(27, 9, 3, N'Ép khuôn', N'Tấm nguyên liệu đã cắt được ép nhiệt định hình lần cuối. Mặt dưới phủ lớp cao su sinh học chống trượt. Kiểm tra độ phẳng và đóng gói kèm giấy chứng nhận.', N'Xưởng ép khuôn GAIA, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline9_3.jpg', '/videos/process_mold.mp4',    '2024-02-24 10:00:00'),
-- Product 10 - Bình cắm hoa GAIA Mini
(28, 10, 1, N'Thu gom',  N'Vỏ sầu riêng đợt 2 từ Chợ Lách, Bến Tre. Đây là lô nguyên liệu có phần vỏ dày và ít gai nhất trong năm, rất thích hợp để ép thành vật phẩm có thành mỏng.', N'Vựa Bình Thủy, Chợ Lách, Bến Tre',  10.1823456, 106.1234567, '/images/timeline10_1.jpg', '/videos/process_collect.mp4', '2024-02-20 06:00:00'),
(29, 10, 2, N'Sơ chế',  N'Xơ vỏ được nghiền và phối trộn với nhựa sinh học PLA. Hỗn hợp được đùn ép thành ống trụ rỗng sơ bộ, sau đó cắt thành từng đoạn cao 10cm.',              N'Nhà máy GAIA, Quận 9, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline10_2.jpg', '/videos/process_clean.mp4',  '2024-02-23 08:00:00'),
(30, 10, 3, N'Ép khuôn', N'Ống trụ được đưa vào khuôn bình cắm hoa, ép định hình miệng bình và đáy. Bên trong phủ nhựa sinh học chống thấm nước. Kiểm tra rò rỉ bằng thử nước thực tế.', N'Xưởng ép khuôn GAIA, TP. Hồ Chí Minh', 10.8141597, 106.7743977, '/images/timeline10_3.jpg', '/videos/process_mold.mp4', '2024-02-26 10:00:00');

SET IDENTITY_INSERT ProductTimeline OFF;

-- =============================================
-- INSERT Story (10 sản phẩm x 5 = 50 records)
-- =============================================

SET IDENTITY_INSERT Story ON;

INSERT INTO Story (StoryId, ProductId, Title, Content, DisplayOrder) VALUES
-- Product 1 - Chậu cây GAIA Mini
(1,  1, N'Tôi từng là vỏ sầu riêng',
N'Tôi sinh ra dưới tán cây sầu riêng già tại vùng đất Cai Lậy, Tiền Giang – nơi nổi tiếng với những mùa sầu riêng thơm lừng. Trong nhiều năm qua, hàng nghìn tấn vỏ sầu riêng bị đổ đi như rác thải sau mỗi mùa vụ. Không ai nghĩ rằng lớp vỏ gai góc, nặng mùi kia có thể trở thành điều gì đó đẹp đẽ và hữu ích. Cho đến ngày GAIA đến – và mọi thứ thay đổi.', 1),
(2,  1, N'Hành trình thu gom',
N'Một buổi sáng sớm tháng Hai, khi sương còn đọng trên lá, đội thu gom của GAIA đến vựa An Phú. Những người nông dân cẩn thận tách từng mảnh vỏ, xếp gọn lên xe tải. Hành trình từ vườn về nhà máy mất gần bốn tiếng đồng hồ – qua những cánh đồng lúa, qua những con kênh xanh mát. Tôi nhìn lại nơi mình sinh ra lần cuối trước khi bắt đầu một cuộc đời hoàn toàn mới.',  2),
(3,  1, N'Quy trình thủ công',
N'Tại nhà máy, tôi được rửa sạch bằng nước vôi khử khuẩn, phơi khô dưới nắng hai ngày. Rồi những người thợ lành nghề nghiền tôi thành bột, rây qua lưới mịn, trộn cùng chất kết dính sinh học từ tinh bột ngô. Hỗn hợp ấy được đổ vào khuôn chậu cây, ép nhiệt ở 160 độ C. Khi thoát ra khỏi khuôn, tôi không còn là vỏ sầu riêng nữa – tôi đã trở thành một chiếc chậu nhỏ xinh, sẵn sàng ôm ấp mầm xanh.',  3),
(4,  1, N'Thông điệp môi trường',
N'Mỗi năm, Việt Nam thải ra hàng trăm nghìn tấn vỏ sầu riêng sau mùa thu hoạch. Phần lớn bị đốt hoặc chôn lấp, gây ô nhiễm không khí và đất. GAIA ra đời với sứ mệnh đơn giản: biến rác thải nông nghiệp thành tài nguyên. Khi bạn chọn chiếc chậu cây này, bạn đang nói không với nhựa dùng một lần và nói có với một tương lai xanh hơn cho thế hệ tiếp theo.',  4),
(5,  1, N'Trở về với đất',
N'Sau khoảng hai năm sử dụng, chiếc chậu nhỏ của tôi sẽ bắt đầu phân hủy một cách tự nhiên. Đất và vi sinh vật sẽ từ từ hấp thụ tôi trở lại. Không độc hại, không ô nhiễm – chỉ là sự tuần hoàn tuyệt vời của thiên nhiên. Tôi bắt đầu là vỏ sầu riêng, tôi sống một cuộc đời ý nghĩa là chiếc chậu xanh, và tôi sẽ kết thúc bằng cách trở về nơi mình xuất phát – lòng đất mẹ.',  5),
-- Product 2 - Lót ly GAIA Tròn
(6,  2, N'Tôi từng là vỏ sầu riêng',
N'Tôi sinh ra từ những trái sầu riêng Ri6 căng mọng ở vùng Cai Lậy xanh tươi. Vỏ của tôi dày và đặc, che chở phần ruột vàng óng bên trong. Nhưng khi mùa vụ qua đi, người ta chỉ lấy phần ruột và bỏ lại tôi. Năm này qua năm khác, hàng tấn vỏ như tôi nằm thối rữa ngoài đồng. Rồi một ngày, GAIA nhìn thấy giá trị trong điều mà người khác bỏ đi.',  1),
(7,  2, N'Hành trình thu gom',
N'Buổi sáng sớm khi người nông dân vừa kết thúc vụ thu hoạch, xe thu gom của GAIA đã chờ sẵn ngoài cổng vựa. Vỏ sầu riêng được cân, phân loại ngay tại chỗ. Những mảnh vỏ đạt tiêu chuẩn được chất lên xe, bắt đầu hành trình về nhà máy. Trên đường đi, qua từng cánh đồng, từng mái nhà, tôi tự hỏi mình sẽ trở thành điều gì ở phía cuối con đường ấy.',  2),
(8,  2, N'Quy trình thủ công',
N'Tại nhà máy GAIA, vỏ được ngâm trong nước vôi loãng để khử khuẩn, sau đó sấy khô và nghiền thành bột. Bột xơ được rây qua lưới mịn, trộn đều với chất kết dính an toàn. Hỗn hợp đổ vào khuôn tròn, ép nhiệt và để nguội. Khi tháo khuôn, chiếc lót ly nhỏ nhắn hiện ra – bề mặt nhám tự nhiên, màu nâu ấm áp của đất, mang theo câu chuyện của cánh đồng Tiền Giang.',  3),
(9,  2, N'Thông điệp môi trường',
N'Một chiếc lót ly đơn giản, nhưng ẩn sau đó là cả một vòng tròn tái sinh đẹp đẽ. Thay vì dùng lót ly cao su hay nhựa tổng hợp, bạn đang dùng một sản phẩm sinh ra từ thiên nhiên và sẽ trả về thiên nhiên. GAIA tin rằng những thay đổi nhỏ trong thói quen hàng ngày – như chọn một chiếc lót ly từ vỏ sầu riêng – chính là những viên gạch đầu tiên xây nên tương lai bền vững.',  4),
(10, 2, N'Trở về với đất',
N'Một ngày nào đó, khi chiếc lót ly đã hoàn thành sứ mệnh của mình, bạn có thể đặt nó vào thùng phân hữu cơ. Trong vòng vài tháng, vi sinh vật đất sẽ chuyển hóa tôi thành mùn bón cây. Vòng đời của tôi khép lại một cách trọn vẹn: từ vỏ sầu riêng, thành lót ly, rồi trở thành dưỡng chất nuôi cây xanh tiếp theo. Đó là điều GAIA gọi là kinh tế tuần hoàn trong thực tế.',  5),
-- Product 3 - Khay trang trí đa năng
(11, 3, N'Tôi từng là vỏ sầu riêng',
N'Vùng Chợ Lách của Bến Tre nổi tiếng là thủ phủ sầu riêng với những vườn cây trải dài theo các con kênh đào. Tôi là một phần của vụ thu hoạch tháng Hai, khi trái sầu riêng chín rộ và người dân tất bật từ sáng sớm. Sau khi ruột ngọt ngào được đưa đến tay người tiêu dùng, vỏ của tôi – dày, chắc, thơm mùi đất – được trao cho GAIA để bắt đầu một cuộc hành trình mới.',  1),
(12, 3, N'Hành trình thu gom',
N'Xe thu gom GAIA đến vựa Bình Thủy khi trời vừa tảng sáng. Bà Trần Thị Bình – người gắn bó với nghề sầu riêng ba mươi năm – đích thân giám sát việc phân loại. Chỉ những mảnh vỏ dày đều, không nấm mốc, mới được chọn. Hàng trăm ký vỏ xếp gọn trên xe, lên đường về nhà máy GAIA tại thành phố. Mỗi kilogram là một phần câu chuyện của người nông dân Bến Tre.',  2),
(13, 3, N'Quy trình thủ công',
N'Vỏ được tách gai, rửa sạch và phơi nắng tự nhiên trong hai ngày. Sau đó nghiền thành bột thô, sàng lọc để đạt độ đồng đều. Bột trộn với dầu lanh và chất kết dính sinh học, đổ khuôn khay chữ nhật, ép dưới áp suất năm mươi tấn. Bề mặt khay sau đó được phủ thêm một lớp dầu lanh tự nhiên, vừa bảo vệ vừa tạo ra màu sắc nâu ấm đặc trưng của GAIA.',  3),
(14, 3, N'Thông điệp môi trường',
N'Chiếc khay nhỏ này có thể thay thế hoàn toàn cho khay nhựa hay khay kim loại trong không gian bàn làm việc của bạn. Nhựa mất hơn năm trăm năm để phân hủy; khay GAIA chỉ cần vài năm. Mỗi sản phẩm GAIA bán ra đồng nghĩa với việc thêm vài ký vỏ sầu riêng được cứu khỏi bãi rác. Đây là sự thay đổi mà bạn có thể nhìn thấy, chạm vào và sử dụng mỗi ngày.',  4),
(15, 3, N'Trở về với đất',
N'Khi chiếc khay đã hoàn thành vai trò của mình, đừng vứt nó vào thùng rác thông thường. Hãy để nó dưới gốc cây trong vườn, hoặc cho vào thùng phân hữu cơ. Chỉ sau vài tháng, chiếc khay sẽ tan biến vào đất, trả lại chất dinh dưỡng cho cây trồng. Một vòng đời khép kín, không để lại dấu vết ô nhiễm – đó là cam kết của mỗi sản phẩm mang tên GAIA.',  5),
-- Product 4 - Hộp bút sinh học GAIA
(16, 4, N'Tôi từng là vỏ sầu riêng',
N'Tôi là vỏ của những trái sầu riêng Ri6 bội thu năm nay tại Chợ Lách. Vỏ của tôi dày và rắn chắc hơn những vụ trước, có lẽ nhờ mùa mưa thuận lợi. Khi trái chín được hái xuống, người ta tách ruột vàng óng và bỏ lại phần vỏ. Nhưng GAIA đã nhìn thấy tiềm năng trong lớp vỏ gai nhọn ấy – một tiềm năng mà phần lớn thế giới vẫn chưa nhận ra.',  1),
(17, 4, N'Hành trình thu gom',
N'Đợt thu gom thứ hai tháng Hai tại Chợ Lách diễn ra thuận lợi. Vỏ sầu riêng được cân và kiểm tra độ ẩm ngay tại vựa. Những mảnh vỏ đạt tiêu chuẩn – không dập, không nấm, độ dày trên hai centimeter – được chất lên xe. Tôi cùng hàng trăm ký vỏ khác rời Bến Tre trong buổi sáng sớm, bắt đầu hành trình biến đổi về phía nhà máy GAIA.',  2),
(18, 4, N'Quy trình thủ công',
N'Tại nhà máy, xơ vỏ được xay nhỏ rồi trộn với hai mươi phần trăm tinh bột sắn để tăng độ kết dính. Hỗn hợp ủ mười hai tiếng cho ngấm đều, rồi đổ vào khuôn hộp bút hình trụ gia nhiệt sẵn. Sau mười phút ép nhiệt và làm nguội, hộp bút thoát khuôn gọn gàng. Mỗi chiếc hộp được kiểm tra thủ công trước khi đóng gói – đó là sự tỉ mỉ mà GAIA luôn giữ vững.',  3),
(19, 4, N'Thông điệp môi trường',
N'Hộp bút nhựa thông thường sẽ tồn tại hàng trăm năm trong môi trường sau khi bạn vứt đi. Hộp bút GAIA thì khác – sau khi không còn sử dụng, nó sẽ phân hủy hoàn toàn và an toàn. Bạn không chỉ đang dùng một chiếc hộp bút; bạn đang tham gia vào một phong trào tiêu dùng có trách nhiệm, nơi mỗi lựa chọn mua sắm là một phiếu bầu cho tương lai xanh.',  4),
(20, 4, N'Trở về với đất',
N'Tuổi thọ của hộp bút GAIA ước tính từ ba đến năm năm trong điều kiện sử dụng bình thường. Khi đến lúc nghỉ hưu, hãy chôn nó trong đất hoặc thêm vào thùng ủ phân. Trong vòng sáu tháng, hộp bút sẽ hòa tan vào đất, trở thành phần tử dinh dưỡng nuôi cây. Tôi bắt đầu từ đất, sống cuộc đời ý nghĩa bên cạnh bạn, và cuối cùng, tôi lại trở về đất.',  5),
-- Product 5 - Đế nến thiên nhiên
(21, 5, N'Tôi từng là vỏ sầu riêng',
N'Tôi đến từ vùng Long Hồ, Vĩnh Long – nơi những vườn sầu riêng Monthong xanh tốt dọc theo sông Cổ Chiên. Phần vỏ của tôi dày và có hương thơm đặc trưng của đất phù sa miền Tây. Sau khi ruột thơm ngon được đưa đến bàn tiệc, tôi nằm lại một mình trong đống vỏ. Nhưng số phận của tôi đã thay đổi khi đội thu gom GAIA xuất hiện vào một buổi sáng bình thường.',  1),
(22, 5, N'Hành trình thu gom',
N'Ông Lê Minh Cường – người gắn bó với vườn sầu riêng từ thuở hai mươi tuổi – nay đã ký hợp đồng định kỳ với GAIA. Mỗi tuần, xe thu gom đến nhà vườn Long Phụng, cân và chở về những túi vỏ sầu riêng được phân loại cẩn thận. Tôi lên xe trong buổi sáng sớm tháng Hai, nhìn lại khu vườn quen thuộc một lần cuối, rồi bắt đầu hành trình mới của mình.',  2),
(23, 5, N'Quy trình thủ công',
N'Vỏ sầu riêng qua ba lần rửa nước sạch, sau đó sấy công nghiệp xuống độ ẩm mười phần trăm. Phần xơ được nghiền và sàng kỹ, trộn với chất kết dính sinh học. Hỗn hợp đổ vào khuôn silicon chịu nhiệt hình tròn, ép định hình. Mặt dưới được dán miếng silicon chống trượt. Mỗi chiếc đế nến được thử nghiệm thực tế với nến thật để đảm bảo an toàn cháy nổ.',  3),
(24, 5, N'Thông điệp môi trường',
N'Một buổi tối thư giãn với ánh nến ấm áp giờ đây có thể trở nên trọn vẹn hơn khi bạn biết chiếc đế nến dưới ngọn lửa kia sinh ra từ mảnh đất miền Tây. Mỗi sản phẩm GAIA là một câu chuyện về sự kết nối – giữa người nông dân, nhà máy, và người tiêu dùng. Cùng nhau, chúng ta đang viết lại câu chuyện về rác thải nông nghiệp tại Việt Nam.',  4),
(25, 5, N'Trở về với đất',
N'Chiếc đế nến nhỏ sẽ đồng hành cùng bạn qua nhiều mùa nến thơm. Khi bề mặt mòn dần hoặc bạn muốn thay mới, đừng bỏ vào thùng rác thông thường. Hãy nghiền nhỏ và cho vào chậu cây – đế nến sẽ phân hủy từ từ, bổ sung chất hữu cơ cho đất. Vòng đời tôi kết thúc ở đây, và đây cũng chính là khởi đầu cho một sự sống mới. Đó là vẻ đẹp của tái sinh.',  5),
-- Product 6 - Khung ảnh GAIA Rustic
(26, 6, N'Tôi từng là vỏ sầu riêng',
N'Tôi là vỏ của những trái sầu riêng Ri6 đặc sản Long Hồ. Người ta thường nói sầu riêng Long Hồ ngon vì đất phù sa màu mỡ ven sông. Phần vỏ của tôi dày đến ba centimeter, xơ chắc và mịn – điều mà các kỹ thuật viên GAIA đặc biệt tìm kiếm. Từ một mảnh vỏ bị bỏ lại, tôi sẽ trở thành khung ảnh lưu giữ những khoảnh khắc đáng nhớ nhất đời người.',  1),
(27, 6, N'Hành trình thu gom',
N'GAIA ký hợp đồng thu mua định kỳ hàng tuần với vườn nhà ông Lê Minh Cường. Sự ổn định của hợp đồng giúp người nông dân yên tâm và giúp GAIA có nguồn nguyên liệu chất lượng đều đặn. Mỗi lô hàng đều được ghi chép nguồn gốc rõ ràng – từ tọa độ GPS vườn thu gom đến thời gian xuất phát. Minh bạch chuỗi cung ứng là cam kết cốt lõi của GAIA.',  2),
(28, 6, N'Quy trình thủ công',
N'Xơ vỏ được nghiền siêu mịn, trộn mười lăm phần trăm keo sinh học từ tinh bột ngô. Hỗn hợp cán thành tấm phẳng, sau đó cắt và ghép vào khuôn khung ảnh, ép mười hai phút. Phần lưng khung được gắn móc treo bằng inox bền bỉ. Thợ thủ công kiểm tra từng góc khung, đảm bảo phẳng và vuông vắn. Đây là sản phẩm đòi hỏi sự tỉ mỉ cao nhất trong dòng GAIA.',  3),
(29, 6, N'Thông điệp môi trường',
N'Khung ảnh thường làm bằng nhựa, gỗ ép hoặc kim loại – tất cả đều để lại dấu ấn môi trường đáng kể trong quá trình sản xuất. Khung ảnh GAIA chọn con đường khác: tận dụng phế liệu nông nghiệp, không cần khai thác thêm tài nguyên. Mỗi khung ảnh GAIA treo trên tường nhà bạn không chỉ lưu giữ kỷ niệm – mà còn kể một câu chuyện về sự chọn lựa có trách nhiệm.',  4),
(30, 6, N'Trở về với đất',
N'Phần lớn khung ảnh nhựa khi hỏng sẽ nằm trong bãi rác hàng thế kỷ. Khung ảnh GAIA khi kết thúc sứ mệnh có thể được tháo móc inox ra riêng để tái sử dụng, còn phần khung thì có thể phân hủy sinh học. Đây là thiết kế có chủ đích – sản phẩm được tạo ra với ý thức về điểm cuối vòng đời từ ngay đầu quá trình sản xuất.',  5),
-- Product 7 - Bộ lót nồi GAIA Set 3
(31, 7, N'Tôi từng là vỏ sầu riêng',
N'Tôi sinh ra từ những vườn sầu riêng ven sông tại Phong Điền, Cần Thơ. Vùng đất này nổi tiếng với những mùa trái cây bội thu, và sầu riêng là niềm tự hào của người dân nơi đây. Phần vỏ dày và chắc của tôi thường bị đổ xuống kênh hoặc đốt ngoài đồng. Năm nay, tôi may mắn được GAIA lựa chọn để bắt đầu một cuộc đời thứ hai – hữu ích và ý nghĩa hơn.',  1),
(32, 7, N'Hành trình thu gom',
N'Phong Điền có nhiều vườn sầu riêng nằm dọc theo các con rạch nhỏ. Xe thu gom GAIA phải đi qua những con đường đất hẹp để đến tận vườn. Bà Phạm Thị Dung tiếp đón đội thu gom với những túi vỏ đã phân loại sẵn. Tiêu chí chọn lọc khắt khe: vỏ dày trên hai centimeter, không có dấu hiệu nấm mốc. Chỉ nguyên liệu tốt nhất mới được đưa vào dây chuyền GAIA.',  2),
(33, 7, N'Quy trình thủ công',
N'Xơ vỏ sầu riêng được nghiền và trộn với xơ dừa – một sự kết hợp tạo ra khả năng chịu nhiệt vượt trội. Hỗn hợp này được ép trong khuôn lót nồi ba kích cỡ ở nhiệt độ một trăm tám mươi độ C. Mỗi bộ ba chiếc được kiểm tra thử nhiệt thực tế với nồi nóng trước khi đóng gói. Đây là bộ sản phẩm có yêu cầu kỹ thuật cao nhất trong toàn bộ dòng GAIA.',  3),
(34, 7, N'Thông điệp môi trường',
N'Bếp ăn là nơi tạo ra nhiều chất thải nhựa nhất trong gia đình: túi ni lông, hộp nhựa, lót nồi silicone. GAIA muốn thay đổi điều đó từ những thứ nhỏ nhất. Bộ lót nồi từ vỏ sầu riêng là lời nhắn nhủ rằng bếp xanh không chỉ là ăn rau – mà còn là chọn đồ dùng bếp thân thiện với môi trường. Bắt đầu từ chiếc lót nồi, và bạn đang bắt đầu một cuộc cách mạng nhỏ.',  4),
(35, 7, N'Trở về với đất',
N'Bộ lót nồi GAIA có thể chịu đựng vài năm sử dụng trong môi trường bếp. Khi chúng mòn dần, hãy nghiền nhỏ và rải vào vườn rau. Thành phần tự nhiên trong lót nồi sẽ cải thiện cấu trúc đất và cung cấp chất hữu cơ. Từ vỏ sầu riêng Cần Thơ, qua tay người thợ GAIA, lên bàn bếp nhà bạn, và cuối cùng nuôi dưỡng vườn rau – đó là vòng tuần hoàn hoàn hảo.',  5),
-- Product 8 - Móc khóa GAIA
(36, 8, N'Tôi từng là vỏ sầu riêng',
N'Tôi là phần vỏ nhỏ còn lại sau khi những mảnh lớn đã được chọn để làm bộ lót nồi. Thông thường, những mảnh vỡ nhỏ như tôi sẽ bị bỏ đi – không đủ lớn, không đủ dày để làm sản phẩm thông thường. Nhưng GAIA không lãng phí. Với công nghệ nghiền siêu mịn và ép định hình, ngay cả những mảnh vụn nhỏ nhất cũng có thể trở thành điều gì đó đáng yêu và hữu ích.',  1),
(37, 8, N'Hành trình thu gom',
N'Không có hành trình riêng cho tôi – tôi đến cùng chuyến xe với những mảnh vỏ lớn từ Phong Điền. Nhưng câu chuyện của tôi bắt đầu tại nhà máy, khi người phân loại nhận ra những mảnh nhỏ này không nên bỏ phí. Quyết định giữ lại và tận dụng những phần "thừa" chính là triết lý zero-waste mà GAIA theo đuổi. Không có gì là rác thải nếu ta biết cách nhìn.',  2),
(38, 8, N'Quy trình thủ công',
N'Phần vỏ nhỏ được nghiền siêu mịn và trộn nhựa sinh học tạo paste. Paste đổ vào khuôn hình lá sầu riêng cách điệu, được thiết kế bởi nghệ nhân người Việt. Sau khi ép và tháo khuôn, thợ thủ công dùng giấy nhám mịn đánh bóng từng chiếc móc khóa. Gắn khoen inox vào, hoàn thiện xong. Mỗi chiếc móc khóa là một tác phẩm thủ công nhỏ, không chiếc nào giống chiếc nào hoàn toàn.',  3),
(39, 8, N'Thông điệp môi trường',
N'Móc khóa là vật nhỏ, nhưng thông điệp nó mang theo không nhỏ chút nào. Khi bạn lấy chìa khóa ra khỏi túi mỗi ngày, bạn sẽ thấy chiếc móc nâu đất mộc mạc và nhớ rằng: mình đang lựa chọn sống xanh. Chia sẻ câu chuyện về chiếc móc khóa này với bạn bè – đó là cách đơn giản nhất để lan tỏa thông điệp về tiêu dùng có trách nhiệm.',  4),
(40, 8, N'Trở về với đất',
N'Chiếc móc khóa nhỏ xíu này sẽ theo bạn đi khắp nơi, chứng kiến bao khoảnh khắc đời thường. Khi đến lúc nghỉ hưu, hãy đặt nó vào đất ẩm của vườn. Chỉ sau vài tháng, lớp nhựa sinh học và xơ vỏ sầu riêng sẽ hòa tan dần vào đất. Một vật nhỏ, một hành động nhỏ – nhưng nhân lên hàng nghìn lần, đó là sự khác biệt mà GAIA và những người như bạn đang tạo ra.',  5),
-- Product 9 - Tấm lót bàn phím GAIA
(41, 9, N'Tôi từng là vỏ sầu riêng',
N'Tôi đến từ lô thu gom đợt hai tại vựa An Phú, Cai Lậy – những mảnh vỏ Ri6 chất lượng tốt nhất trong năm. Xơ của tôi mịn và đồng đều, rất phù hợp để cán thành tấm phẳng. Mùa này, nông dân được mùa lớn, và lượng vỏ dư thừa nhiều hơn bao giờ hết. Thay vì đốt hay đổ xuống kênh, tất cả được GAIA thu gom và trao cho một cuộc đời mới đầy ý nghĩa.',  1),
(42, 9, N'Hành trình thu gom',
N'Đợt thu gom thứ hai trong tháng tại Cai Lậy diễn ra suôn sẻ. Bầu trời trong xanh, con đường qua các vườn sầu riêng thơm ngát. Người nông dân đã quen với lịch thu gom định kỳ của GAIA, sẵn sàng từng túi nguyên liệu gọn gàng. Bảy trăm ký vỏ lên xe, bắt đầu hành trình về nhà máy. Trong số đó có phần của tôi – chờ đợi được biến thành điều gì đó hữu ích.',  2),
(43, 9, N'Quy trình thủ công',
N'Nguyên liệu nghiền mịn được cán thành tấm dày bốn milimet theo công nghệ độc đáo của GAIA. Tấm nguyên liệu sấy phẳng ở nhiệt độ thấp để tránh cong vênh, sau đó cắt chính xác theo kích thước chuẩn bàn phím. Ép nhiệt định hình lần cuối, mặt dưới phủ cao su sinh học chống trượt. Mỗi tấm lót được đo độ phẳng bằng thước laser trước khi đóng gói.',  3),
(44, 9, N'Thông điệp môi trường',
N'Bàn làm việc của bạn là nơi bạn dành nhiều giờ nhất trong ngày. Tại sao không để nơi đó kể một câu chuyện về sự bền vững? Tấm lót bàn phím GAIA thay thế hoàn toàn cho tấm nhựa hay cao su tổng hợp thông thường. Mỗi lần nhìn xuống bàn phím, bạn được nhắc nhớ về một vùng đất, một người nông dân, và một cam kết với tương lai xanh hơn.',  4),
(45, 9, N'Trở về với đất',
N'Với lớp cao su sinh học bên dưới, tấm lót GAIA được thiết kế để phân hủy hoàn toàn. Sau vài năm sử dụng, khi bề mặt mòn đi, hãy cắt nhỏ và cho vào thùng ủ phân hữu cơ. Tấm lót sẽ phân hủy trong vòng sáu đến mười hai tháng. Bạn đang ngồi trên lịch sử của một vùng đất trái cây, và khi nó kết thúc, nó sẽ nuôi dưỡng đất cho mùa vụ tiếp theo.',  5),
-- Product 10 - Bình cắm hoa GAIA Mini
(46, 10, N'Tôi từng là vỏ sầu riêng',
N'Vùng Chợ Lách của Bến Tre không chỉ có sầu riêng – đây còn là xứ sở của hoa và cây cảnh. Tôi là vỏ của những trái sầu riêng cuối mùa, khi người ta đã thu hoạch gần hết và chỉ còn lại những trái muộn. Vỏ tôi đặc biệt mỏng và ít gai hơn bình thường – điều mà các kỹ thuật viên GAIA ngay lập tức nhận ra là nguyên liệu lý tưởng cho chiếc bình cắm hoa tinh tế.',  1),
(47, 10, N'Hành trình thu gom',
N'Đợt thu gom tháng Hai lần hai tại Chợ Lách mang về lô nguyên liệu đặc biệt – những mảnh vỏ mỏng, ít gai, chất lượng tốt nhất trong năm. Đây là điều hiếm gặp và đội thu mua GAIA biết phải trân trọng từng kilogram. Tám trăm chín mươi ký vỏ được chất lên xe trong buổi sáng sớm tháng Hai, bắt đầu hành trình cuối cùng về nhà máy GAIA.',  2),
(48, 10, N'Quy trình thủ công',
N'Xơ vỏ được nghiền và phối trộn với nhựa sinh học PLA theo tỉ lệ đặc biệt. Hỗn hợp đùn ép thành ống trụ rỗng, cắt thành đoạn cao mười centimeter. Khuôn bình cắm hoa định hình miệng bình loe nhẹ và đáy phẳng. Bên trong phủ nhựa sinh học chống thấm. Mỗi bình được thử nước thực tế – rót nước vào giữ hai mươi bốn tiếng để kiểm tra độ kín.',  3),
(49, 10, N'Thông điệp môi trường',
N'Một bình hoa nhỏ trên bàn làm việc hay cửa sổ nhà bạn có thể kể một câu chuyện lớn. Chọn bình hoa GAIA là chọn ủng hộ người nông dân miền Tây, là giảm thêm một vật dụng nhựa trong nhà, là gửi đi một thông điệp rằng bạn quan tâm đến nơi mình đang sống. Vẻ đẹp thật sự không chỉ nằm ở hoa – mà còn ở chiếc bình đang ôm ấp chúng.',  4),
(50, 10, N'Trở về với đất',
N'Lớp nhựa sinh học bên trong bình hoa GAIA sẽ phân hủy chậm hơn phần vỏ ngoài – nhưng cả hai đều sẽ trở về đất trong vài năm. Khi chiếc bình đã sống trọn sứ mệnh của mình, hãy để nó nghỉ ngơi trong vườn nhà. Đất sẽ ôm ấp nó, vi sinh vật sẽ chuyển hóa nó, và một mùa hoa mới sẽ nở từ chính phần đất ấy. Vòng tròn của sự sống – không có điểm kết thúc, chỉ có sự chuyển hóa.',  5);

SET IDENTITY_INSERT Story OFF;

-- =============================================
-- VERIFICATION
-- =============================================

SELECT 'Users'                AS TableName, COUNT(*) AS RecordCount FROM Users                UNION ALL
SELECT 'Supplier',                          COUNT(*)               FROM Supplier              UNION ALL
SELECT 'MaterialBatch',                     COUNT(*)               FROM MaterialBatch         UNION ALL
SELECT 'MaterialImage',                     COUNT(*)               FROM MaterialImage          UNION ALL
SELECT 'TransportationHistory',             COUNT(*)               FROM TransportationHistory  UNION ALL
SELECT 'Product',                           COUNT(*)               FROM Product               UNION ALL
SELECT 'ProductTimeline',                   COUNT(*)               FROM ProductTimeline        UNION ALL
SELECT 'Story',                             COUNT(*)               FROM Story;
GO