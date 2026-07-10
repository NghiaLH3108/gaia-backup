using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GaiaWeb.DAL.Models;

namespace GaiaWeb.BLL.Services.Interfaces
{
    public interface IProductService
    {
        // Guest
        Task<Product?> GetProductStoryAsync(Guid qrToken);

        // UC20 – Admin CRUD
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int productId);
        Task<(bool Success, string Message)> CreateProductAsync(int batchId, string productName, string description, string currentStatus);
        Task<(bool Success, string Message)> UpdateProductAsync(int productId, string productName, string description, string currentStatus);
        Task<(bool Success, string Message)> DeleteProductAsync(int productId);
    }
}

