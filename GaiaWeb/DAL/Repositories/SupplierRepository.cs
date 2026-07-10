using GaiaWeb.DAL.Data;
using GaiaWeb.DAL.Models;
using GaiaWeb.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GaiaWeb.DAL.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly GaiaDbContext _context;

    public SupplierRepository(GaiaDbContext context)
    {
        _context = context;
    }

    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers
            .Include(s => s.User)
            .Include(s => s.MaterialBatches)
            .OrderByDescending(s => s.CreatedDate)
            .ToListAsync();
    }

    public async Task<Supplier?> GetByIdAsync(int supplierId)
    {
        return await _context.Suppliers
            .Include(s => s.User)
            .Include(s => s.MaterialBatches)
            .FirstOrDefaultAsync(s => s.SupplierId == supplierId);
    }

    public async Task<Supplier> CreateAsync(Supplier supplier, User user)
    {
        // Save user first to get UserId
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        supplier.UserId = user.UserId;
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return supplier;
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();
    }

    // Soft delete: mark User as Inactive so they cannot login
    public async Task SoftDeleteAsync(int supplierId)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

        if (supplier?.User != null)
        {
            supplier.User.Status = "Inactive";
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasActiveBatchesAsync(int supplierId)
    {
        return await _context.MaterialBatches
            .AnyAsync(b => b.SupplierId == supplierId && b.Status != "Rejected");
    }

    public async Task RestoreAsync(int supplierId)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

        if (supplier?.User != null)
        {
            supplier.User.Status = "Active";
            await _context.SaveChangesAsync();
        }
    }
}
