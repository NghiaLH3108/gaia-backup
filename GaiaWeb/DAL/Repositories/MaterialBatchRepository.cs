using GaiaWeb.DAL.Data;
using GaiaWeb.DAL.Models;
using GaiaWeb.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GaiaWeb.DAL.Repositories;

public class MaterialBatchRepository : IMaterialBatchRepository
{
    private readonly GaiaDbContext _context;

    public MaterialBatchRepository(GaiaDbContext context)
    {
        _context = context;
    }

    // UC12 – Create batch
    public async Task<MaterialBatch> CreateAsync(MaterialBatch batch)
    {
        _context.MaterialBatches.Add(batch);
        await _context.SaveChangesAsync();
        return batch;
    }

    // UC10 – Add image
    public async Task AddImageAsync(MaterialImage image)
    {
        _context.MaterialImages.Add(image);
        await _context.SaveChangesAsync();
    }

    // UC12 – Auto-generate batch code (BAT001, BAT002...)
    public async Task<string> GenerateNextBatchCodeAsync()
    {
        var lastCode = await _context.MaterialBatches
            .Where(b => b.BatchCode.StartsWith("BAT"))
            .OrderByDescending(b => b.BatchCode)
            .Select(b => b.BatchCode)
            .FirstOrDefaultAsync();

        int next = 1;
        if (lastCode != null && lastCode.Length > 3 && int.TryParse(lastCode.Substring(3), out int last))
            next = last + 1;

        return $"BAT{next:D3}";
    }

    // UC14 – Get all batches for a supplier, eager-load images and transport history
    public async Task<List<MaterialBatch>> GetBatchesBySupplierIdAsync(int supplierId)
    {
        return await _context.MaterialBatches
            .Where(b => b.SupplierId == supplierId)
            .Include(b => b.MaterialImages)
            .Include(b => b.TransportationHistories)
            .OrderByDescending(b => b.CreatedDate)
            .ToListAsync();
    }

    // UC15 / UC13 – Get single batch by ID, eager-load all relations
    public async Task<MaterialBatch?> GetByIdAsync(int batchId)
    {
        return await _context.MaterialBatches
            .Include(b => b.MaterialImages)
            .Include(b => b.TransportationHistories.OrderByDescending(h => h.UpdateTime))
            .Include(b => b.Supplier)
                .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(b => b.BatchId == batchId);
    }

    // UC15 – Save batch edits
    public async Task UpdateAsync(MaterialBatch batch)
    {
        _context.MaterialBatches.Update(batch);
        await _context.SaveChangesAsync();
    }

    // UC13 – Append new transportation history entry
    public async Task AddTransportationHistoryAsync(TransportationHistory history)
    {
        _context.TransportationHistories.Add(history);
        await _context.SaveChangesAsync();
    }

    // UC13 – Update batch status after transportation event
    public async Task UpdateBatchStatusAsync(int batchId, string status)
    {
        var batch = await _context.MaterialBatches.FindAsync(batchId);
        if (batch != null)
        {
            batch.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}
