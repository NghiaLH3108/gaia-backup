using GaiaWeb.DAL.Models;

namespace GaiaWeb.BLL.Services.Interfaces;

public interface ISupplierService
{
    // UC17 – Admin Manage Supplier
    Task<List<Supplier>> GetAllSuppliersAsync();
    Task<Supplier?> GetSupplierByIdAsync(int supplierId);
    Task<(bool Success, string Message)> CreateSupplierAsync(
        string warehouseName, string address, decimal latitude, decimal longitude,
        string fullName, string phone, string email, string password);
    Task<(bool Success, string Message)> UpdateSupplierAsync(
        int supplierId, string warehouseName, string address, decimal latitude, decimal longitude,
        string fullName, string phone);
    Task<(bool Success, string Message)> SoftDeleteSupplierAsync(int supplierId);
    Task<(bool Success, string Message)> RestoreSupplierAsync(int supplierId);
}
