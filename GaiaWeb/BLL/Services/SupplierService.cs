using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using GaiaWeb.DAL.Repositories.Interfaces;

namespace GaiaWeb.BLL.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUserRepository _userRepository;

    public SupplierService(ISupplierRepository supplierRepository, IUserRepository userRepository)
    {
        _supplierRepository = supplierRepository;
        _userRepository = userRepository;
    }

    public async Task<List<Supplier>> GetAllSuppliersAsync()
    {
        return await _supplierRepository.GetAllAsync();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int supplierId)
    {
        return await _supplierRepository.GetByIdAsync(supplierId);
    }

    public async Task<(bool Success, string Message)> CreateSupplierAsync(
        string warehouseName, string address, decimal latitude, decimal longitude,
        string fullName, string phone, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (false, "Email và mật khẩu không được để trống.");

        if (string.IsNullOrWhiteSpace(warehouseName))
            return (false, "Tên kho không được để trống.");

        if (string.IsNullOrWhiteSpace(phone))
            return (false, "Số điện thoại không được để trống.");

        if (string.IsNullOrWhiteSpace(address))
            return (false, "Địa chỉ không được để trống.");


        // Check duplicate email
        var existing = await _userRepository.GetByEmailAsync(email);
        if (existing != null)
            return (false, "Email này đã được sử dụng bởi tài khoản khác.");

        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                FullName = fullName,
                Phone = phone,
                Email = email,
                PasswordHash = passwordHash,
                Role = "Supplier",
                Status = "Active",
                CreatedDate = DateTime.Now
            };

            var supplier = new Supplier
            {
                WarehouseName = warehouseName,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                CreatedDate = DateTime.Now
            };

            await _supplierRepository.CreateAsync(supplier, user);
            return (true, $"Tạo nhà cung cấp '{warehouseName}' thành công.");
        }
        catch (Exception ex)
        {
            var innerMsg = ex.InnerException != null ? ex.InnerException.Message : "";
            return (false, $"Lỗi khi tạo nhà cung cấp: {ex.Message} {innerMsg}");
        }
    }

    public async Task<(bool Success, string Message)> UpdateSupplierAsync(
        int supplierId, string warehouseName, string address, decimal latitude, decimal longitude,
        string fullName, string phone)
    {
        var supplier = await _supplierRepository.GetByIdAsync(supplierId);
        if (supplier == null)
            return (false, "Không tìm thấy nhà cung cấp.");

        if (string.IsNullOrWhiteSpace(warehouseName))
            return (false, "Tên kho không được để trống.");

        if (string.IsNullOrWhiteSpace(phone))
            return (false, "Số điện thoại không được để trống.");

        if (string.IsNullOrWhiteSpace(address))
            return (false, "Địa chỉ không được để trống.");

        supplier.WarehouseName = warehouseName;
        supplier.Address = address;
        supplier.Latitude = latitude;
        supplier.Longitude = longitude;

        if (supplier.User != null)
        {
            supplier.User.FullName = fullName;
            supplier.User.Phone = phone;
        }

        await _supplierRepository.UpdateAsync(supplier);
        return (true, "Cập nhật thông tin nhà cung cấp thành công.");
    }

    // Soft delete: deactivates the user account so they can no longer login
    public async Task<(bool Success, string Message)> SoftDeleteSupplierAsync(int supplierId)
    {
        var hasActive = await _supplierRepository.HasActiveBatchesAsync(supplierId);
        if (hasActive)
            return (false, "Không thể vô hiệu hóa nhà cung cấp vì còn lô hàng chưa bị từ chối. Hãy từ chối các lô hàng liên quan trước.");

        await _supplierRepository.SoftDeleteAsync(supplierId);
        return (true, "Đã vô hiệu hóa tài khoản nhà cung cấp thành công.");
    }

    public async Task<(bool Success, string Message)> RestoreSupplierAsync(int supplierId)
    {
        var supplier = await _supplierRepository.GetByIdAsync(supplierId);
        if (supplier == null)
            return (false, "Không tìm thấy nhà cung cấp.");

        await _supplierRepository.RestoreAsync(supplierId);
        return (true, "Đã kích hoạt lại tài khoản nhà cung cấp thành công.");
    }
}
