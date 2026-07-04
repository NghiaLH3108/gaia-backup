# AI Guide - GAIA Web (.NET 8 Razor Pages)

## 1. Project Overview

GAIA là hệ thống truy xuất nguồn gốc (Product Traceability) và Storytelling cho các sản phẩm tái chế từ vỏ sầu riêng.

Hệ thống được xây dựng bằng:

- ASP.NET Core 8 Razor Pages
- Entity Framework Core
- SQL Server
- Repository Pattern
- Service Layer
- Dependency Injection

Có 2 nhóm người dùng chính:

- Guest
- Supplier
- Admin

---

# 2. Project Structure

Luôn tuân thủ đúng cấu trúc thư mục sau.

```
GaiaWeb
│
├── BLL
│   └── Services
│       └── Interfaces
│
├── DAL
│   ├── Data
│   │      GaiaDbContext.cs
│   │
│   ├── Models
│   │      User.cs
│   │      Supplier.cs
│   │      MaterialBatch.cs
│   │      MaterialImage.cs
│   │      TransportationHistory.cs
│   │      Product.cs
│   │      ProductTimeline.cs
│   │      Story.cs
│   │
│   └── Repositories
│       └── Interfaces
│
├── Helpers
│
├── Pages
│
├── wwwroot
│
├── Program.cs
└── appsettings.json
```

Không được tự ý thay đổi cấu trúc project.

---

# 3. Database

Database sử dụng SQL Server.

Tên database

```
GAIA_DB
```

Script database nằm trong

```
GAIA_DB.sql
```

AI phải sử dụng đúng các bảng đã được định nghĩa trong file SQL.

Các Entity hiện có gồm

- Users
- Supplier
- MaterialBatch
- MaterialImage
- TransportationHistory
- Product
- ProductTimeline
- Story

Không tự tạo thêm bảng mới nếu không được yêu cầu.

---

# 4. Kiến trúc

Luồng xử lý luôn theo thứ tự

```
Razor Page

↓

Service Interface

↓

Service

↓

Repository Interface

↓

Repository

↓

GaiaDbContext

↓

SQL Server
```

Không được gọi DbContext trực tiếp từ Razor Page.

Không viết logic trong Razor Page.

---

# 5. Coding Convention

## Entity

Model chỉ chứa

- Property
- Navigation Property

Không chứa Business Logic.

---

## Repository

Repository chỉ thực hiện

- CRUD
- Query Database

Không xử lý nghiệp vụ.

---

## Service

Service xử lý

- Business Logic
- Validation
- Mapping
- Transaction

Không viết SQL.

---

## Razor Page

Code Behind (.cshtml.cs)

Chỉ gọi Service.

Không truy cập Repository.

Không truy cập DbContext.

---

# 6. Dependency Injection

Tất cả Repository và Service phải đăng ký trong

Program.cs

Ví dụ

```
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
```

Không sử dụng Singleton cho DbContext.

---

# 7. Authentication

Có 2 Role

```
Admin

Supplier
```

Guest không cần đăng nhập.

Sau khi đăng nhập phải lưu Session.

Các trang quản trị chỉ cho Admin truy cập.

Supplier chỉ thao tác trên dữ liệu của chính mình.

---

# 8. Database Relationships

Users

↓

Supplier

↓

MaterialBatch

↓

MaterialImage

↓

TransportationHistory

↓

Product

↓

ProductTimeline

↓

Story

Quan hệ:

User (1)

↓

Supplier (1)

↓

MaterialBatch (N)

↓

MaterialImage (N)

↓

TransportationHistory (N)

↓

Product (N)

↓

ProductTimeline (N)

↓

Story (N)

AI phải sử dụng Navigation Property đúng theo quan hệ này.

---

# 9. Use Cases

## Guest

### UC01

View Product Story

Hiển thị câu chuyện của sản phẩm.

---

### UC02

View Product Timeline

Hiển thị timeline

- Thu gom
- Sơ chế
- Ép khuôn

---

### UC03

View Collection Location

Hiển thị địa điểm thu gom trên bản đồ.

---

### UC04

View Collection Images

Hiển thị ảnh nguyên liệu.

---

### UC05

View Handmade Video

Hiển thị video quy trình sản xuất.

(Video URL lấy từ ProductTimeline)

---

### UC06

View Environmental Message

Hiển thị thông điệp môi trường.

(Tạm thời lấy từ Story)

---

### UC07

View Compost Guide

Hiển thị hướng dẫn phân hủy.

(Tạm thời lấy từ Story)

---

## Supplier

### UC08

Login

---

### UC09

Create Material Batch

Tạo lô nguyên liệu mới.

---

### UC10

Upload Collection Images

Upload nhiều ảnh cho MaterialBatch.

---

### UC11

Enter Collection Information

Nhập

- Địa chỉ
- Khối lượng
- Thời gian thu gom

---

### UC12

Submit Batch

Chuyển trạng thái

Pending

---

### UC13

Update Transportation Status

Thêm TransportationHistory.

---

### UC14

View Submitted Batches

Danh sách Batch của Supplier.

---

### UC15

Edit Batch Before Approval

Chỉ được sửa khi

Status = Pending

---

## Admin

### UC16

Login

---

### UC17

Manage Supplier

CRUD Supplier.

---

### UC18

Approve Material Batch

Đổi trạng thái

Approved

---

### UC19

Reject Material Batch

Đổi trạng thái

Rejected

---

### UC20

Manage Product

CRUD Product.

---

### UC21

Upload Handmade Video

Cập nhật VideoUrl trong ProductTimeline.

Không tạo bảng Video riêng.

---

### UC22

View Dashboard

Hiển thị

- Tổng Supplier
- Tổng Batch
- Tổng Product
- Batch chờ duyệt

---

### UC23

View Statistics

Biểu đồ

- Batch theo tháng
- Khối lượng thu gom
- Product đã tạo

---

# 10. Entity Framework

Luôn sử dụng

```
async/await
```

và

```
ToListAsync()

FirstOrDefaultAsync()

AnyAsync()

CountAsync()
```

Không dùng synchronous query.

---

# 11. Naming Convention

Interface

```
IProductRepository

IProductService
```

Repository

```
ProductRepository
```

Service

```
ProductService
```

Method

```
GetAllAsync()

GetByIdAsync()

CreateAsync()

UpdateAsync()

DeleteAsync()
```

---

# 12. Razor Pages

Mỗi chức năng gồm

```
Index

Details

Create

Edit

Delete
```

Code Behind

```
Index.cshtml.cs

Create.cshtml.cs

Edit.cshtml.cs

...
```

---

# 13. UI

Ưu tiên

- Bootstrap 5
- Responsive
- DataTables (nếu cần)
- Font Awesome

Không sử dụng JavaScript Framework.

---

# 14. Coding Rules

AI phải:

- Tuân thủ SOLID.
- Không duplicate code.
- Không hardcode Connection String.
- Không viết SQL trong C#.
- Luôn Validate ModelState.
- Sử dụng Dependency Injection.
- Ưu tiên async/await.
- Code rõ ràng, dễ bảo trì.
- Comment ngắn gọn khi logic phức tạp.

Mọi mã nguồn sinh ra phải phù hợp với cấu trúc dự án và database `GAIA_DB.sql`.