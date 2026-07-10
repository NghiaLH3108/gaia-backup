using GaiaWeb.DAL.Models;

namespace GaiaWeb.DAL.Repositories.Interfaces;

public interface IMaterialBatchRepository
{
    // UC10/11/12 – Create
    Task<MaterialBatch> CreateAsync(MaterialBatch batch);
    Task AddImageAsync(MaterialImage image);
    Task<string> GenerateNextBatchCodeAsync();

    // UC14 – View submitted batches
    Task<List<MaterialBatch>> GetBatchesBySupplierIdAsync(int supplierId);

    // UC15 – Edit batch
    Task<MaterialBatch?> GetByIdAsync(int batchId);
    Task UpdateAsync(MaterialBatch batch);

    // UC13 – Transportation
    Task AddTransportationHistoryAsync(TransportationHistory history);
    Task UpdateBatchStatusAsync(int batchId, string status);

    // UC18/UC19 – Admin: get all batches
    Task<List<MaterialBatch>> GetAllAsync();
    Task<List<MaterialBatch>> GetAllPendingAsync();
}

