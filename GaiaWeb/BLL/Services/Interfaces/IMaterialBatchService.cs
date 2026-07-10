using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Http;

namespace GaiaWeb.BLL.Services.Interfaces;

public interface IMaterialBatchService
{
    // UC10/11/12 – Create new batch
    Task<(bool Success, string Message, string BatchCode)> CreateBatchAsync(
        int supplierId,
        decimal weightKg,
        string collectionAddress,
        decimal latitude,
        decimal longitude,
        DateTime collectionTime,
        IList<IFormFile> images,
        string webRootPath);

    // UC14 – Get all batches for supplier
    Task<List<MaterialBatch>> GetBatchesBySupplierIdAsync(int supplierId);

    // UC15 – Get batch detail (verifies supplier ownership)
    Task<MaterialBatch?> GetBatchDetailAsync(int batchId, int supplierId);

    // UC15 – Edit batch (only when Pending)
    Task<(bool Success, string Message)> UpdateBatchAsync(
        int batchId,
        int supplierId,
        string collectionAddress,
        decimal weightKg,
        DateTime collectionTime);

    // UC13 – Add transportation status update
    Task<(bool Success, string Message)> AddTransportationStatusAsync(
        int batchId,
        int supplierId,
        string status,
        string description);

    // UC18 – Admin: get all batches
    Task<List<MaterialBatch>> GetAllBatchesAsync();

    // UC18 – Approve a pending batch
    Task<(bool Success, string Message)> ApproveBatchAsync(int batchId);

    // UC19 – Reject a pending batch
    Task<(bool Success, string Message)> RejectBatchAsync(int batchId);
}
