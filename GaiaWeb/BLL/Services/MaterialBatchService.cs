using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using GaiaWeb.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GaiaWeb.BLL.Services;

public class MaterialBatchService : IMaterialBatchService
{
    private readonly IMaterialBatchRepository _batchRepository;

    public MaterialBatchService(IMaterialBatchRepository batchRepository)
    {
        _batchRepository = batchRepository;
    }

    // ──────────────────────────────────────────────
    // UC10/11/12 – Create new batch with image upload
    // ──────────────────────────────────────────────
    public async Task<(bool Success, string Message, string BatchCode)> CreateBatchAsync(
        int supplierId,
        decimal weightKg,
        string collectionAddress,
        decimal latitude,
        decimal longitude,
        DateTime collectionTime,
        IList<IFormFile> images,
        string webRootPath)
    {
        if (weightKg <= 0)
            return (false, "Trọng lượng phải lớn hơn 0.", string.Empty);

        if (string.IsNullOrWhiteSpace(collectionAddress))
            return (false, "Địa chỉ thu gom không được để trống.", string.Empty);

        try
        {
            var batchCode = await _batchRepository.GenerateNextBatchCodeAsync();

            var batch = new MaterialBatch
            {
                SupplierId    = supplierId,
                BatchCode     = batchCode,
                WeightKg      = weightKg,
                CollectionAddress = collectionAddress,
                Latitude      = latitude,
                Longitude     = longitude,
                CollectionTime = collectionTime,
                Status        = "Pending",
                CreatedDate   = DateTime.Now
            };

            var created = await _batchRepository.CreateAsync(batch);

            // Save uploaded images
            if (images != null && images.Count > 0)
            {
                var uploadDir = Path.Combine(webRootPath, "images", "uploads");
                Directory.CreateDirectory(uploadDir);

                foreach (var file in images)
                {
                    if (file.Length == 0) continue;

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (ext != ".jpg" && ext != ".jpeg" && ext != ".png") continue;
                    if (file.Length > 10 * 1024 * 1024) continue; // skip > 10MB

                    var uniqueName = $"{created.BatchId}_{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadDir, uniqueName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    await _batchRepository.AddImageAsync(new MaterialImage
                    {
                        BatchId    = created.BatchId,
                        ImageUrl   = $"/images/uploads/{uniqueName}",
                        UploadTime = DateTime.Now
                    });
                }
            }

            return (true, "Gửi phiếu thu gom thành công! Lô nguyên liệu đã ở trạng thái Pending.", batchCode);
        }
        catch (Exception ex)
        {
            return (false, $"Lỗi khi xử lý phiếu thu gom: {ex.Message}", string.Empty);
        }
    }

    // ──────────────────────────────────────────────
    // UC14 – List batches belonging to supplier
    // ──────────────────────────────────────────────
    public async Task<List<MaterialBatch>> GetBatchesBySupplierIdAsync(int supplierId)
    {
        return await _batchRepository.GetBatchesBySupplierIdAsync(supplierId);
    }

    // ──────────────────────────────────────────────
    // UC15 / UC13 – Get single batch (with ownership check)
    // ──────────────────────────────────────────────
    public async Task<MaterialBatch?> GetBatchDetailAsync(int batchId, int supplierId)
    {
        var batch = await _batchRepository.GetByIdAsync(batchId);
        if (batch == null || batch.SupplierId != supplierId) return null;
        return batch;
    }

    // ──────────────────────────────────────────────
    // UC15 – Edit batch before approval
    // ──────────────────────────────────────────────
    public async Task<(bool Success, string Message)> UpdateBatchAsync(
        int batchId,
        int supplierId,
        string collectionAddress,
        decimal weightKg,
        DateTime collectionTime)
    {
        var batch = await _batchRepository.GetByIdAsync(batchId);
        if (batch == null || batch.SupplierId != supplierId)
            return (false, "Không tìm thấy lô hàng.");

        // UC15: only editable when Pending
        if (batch.Status != "Pending")
            return (false, "Lô hàng này không thể chỉnh sửa vì trạng thái không phải Pending.");

        if (string.IsNullOrWhiteSpace(collectionAddress))
            return (false, "Địa chỉ thu gom không được để trống.");

        if (weightKg <= 0)
            return (false, "Trọng lượng phải lớn hơn 0.");

        batch.CollectionAddress = collectionAddress;
        batch.WeightKg          = weightKg;
        batch.CollectionTime    = collectionTime;

        await _batchRepository.UpdateAsync(batch);
        return (true, "Cập nhật lô hàng thành công.");
    }

    // ──────────────────────────────────────────────
    // UC13 – Add transportation history entry
    // ──────────────────────────────────────────────
    public async Task<(bool Success, string Message)> AddTransportationStatusAsync(
        int batchId,
        int supplierId,
        string status,
        string description)
    {
        var batch = await _batchRepository.GetByIdAsync(batchId);
        if (batch == null || batch.SupplierId != supplierId)
            return (false, "Không tìm thấy lô hàng.");

        // Only allow transportation updates when Approved or already Transporting
        if (batch.Status != "Approved" && batch.Status != "Transporting")
            return (false, "Chỉ có thể cập nhật vận chuyển khi lô hàng đã được phê duyệt hoặc đang vận chuyển.");

        var allowed = new[] { "Transporting", "ArrivedFactory" };
        if (!allowed.Contains(status))
            return (false, "Trạng thái vận chuyển không hợp lệ.");

        var history = new TransportationHistory
        {
            BatchId     = batchId,
            Status      = status,
            Description = description,
            UpdateTime  = DateTime.Now
        };

        await _batchRepository.AddTransportationHistoryAsync(history);
        await _batchRepository.UpdateBatchStatusAsync(batchId, status);

        return (true, "Cập nhật trạng thái vận chuyển thành công.");
    }

    // ──────────────────────────────────────────────
    // UC18 – Admin: get all batches
    // ──────────────────────────────────────────────
    public async Task<List<MaterialBatch>> GetAllBatchesAsync()
    {
        return await _batchRepository.GetAllAsync();
    }

    // ──────────────────────────────────────────────
    // UC18 – Approve a pending batch
    // ──────────────────────────────────────────────
    public async Task<(bool Success, string Message)> ApproveBatchAsync(int batchId)
    {
        var batch = await _batchRepository.GetByIdAsync(batchId);
        if (batch == null)
            return (false, "Không tìm thấy lô hàng.");

        if (batch.Status != "Pending")
            return (false, $"Lô hàng hiện đang ở trạng thái '{batch.Status}', không thể phê duyệt.");

        batch.Status = "Approved";
        batch.ApprovedTime = DateTime.Now;
        await _batchRepository.UpdateAsync(batch);

        return (true, $"Đã phê duyệt lô hàng {batch.BatchCode} thành công.");
    }

    // ──────────────────────────────────────────────
    // UC19 – Reject a pending batch
    // ──────────────────────────────────────────────
    public async Task<(bool Success, string Message)> RejectBatchAsync(int batchId)
    {
        var batch = await _batchRepository.GetByIdAsync(batchId);
        if (batch == null)
            return (false, "Không tìm thấy lô hàng.");

        if (batch.Status != "Pending")
            return (false, $"Lô hàng hiện đang ở trạng thái '{batch.Status}', không thể từ chối.");

        batch.Status = "Rejected";
        await _batchRepository.UpdateAsync(batch);

        return (true, $"Đã từ chối lô hàng {batch.BatchCode}.");
    }

    // UC22/23 – Dashboard and Statistics
    public async Task<decimal> GetTotalWeightSavedAsync()
    {
        var batches = await _batchRepository.GetAllAsync();
        return batches
            .Where(b => b.Status == "Approved" || b.Status == "Transporting" || b.Status == "ArrivedFactory")
            .Sum(b => b.WeightKg);
    }

    public async Task<Dictionary<string, decimal>> GetMonthlyWeightCollectionAsync()
    {
        var batches = await _batchRepository.GetAllAsync();
        return batches
            .Where(b => b.Status == "Approved" || b.Status == "Transporting" || b.Status == "ArrivedFactory")
            .GroupBy(b => b.CollectionTime.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.WeightKg));
    }

    public async Task<Dictionary<string, decimal>> GetTopSuppliersWeightAsync(int limit)
    {
        var batches = await _batchRepository.GetAllAsync();
        return batches
            .Where(b => b.Status == "Approved" || b.Status == "Transporting" || b.Status == "ArrivedFactory")
            .GroupBy(b => b.Supplier?.WarehouseName ?? "N/A")
            .OrderByDescending(g => g.Sum(b => b.WeightKg))
            .Take(limit)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.WeightKg));
    }
}

