using GaiaWeb.DAL.Models;

namespace GaiaWeb.DAL.Repositories.Interfaces;

public interface ISupplierRepository
{
    // UC17 – Admin CRUD Supplier
    Task<List<Supplier>> GetAllAsync();
    Task<Supplier?> GetByIdAsync(int supplierId);
    Task<Supplier> CreateAsync(Supplier supplier, User user);
    Task UpdateAsync(Supplier supplier);

    // Soft delete: set User.Status = "Inactive"
    Task SoftDeleteAsync(int supplierId);
    Task RestoreAsync(int supplierId);

    // Helper: check if supplier has any non-rejected batches
    Task<bool> HasActiveBatchesAsync(int supplierId);
}
