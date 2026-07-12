using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GaiaWeb.DAL.Models;

namespace GaiaWeb.DAL.Repositories.Interfaces
{
    public interface IProductRepository
    {
        // Guest
        Task<Product?> GetProductStoryByQRTokenAsync(Guid qrToken);

        // UC20 – Admin CRUD
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int productId);
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int productId);

        // UC21 – Upload Handmade Video
        Task<List<ProductTimeline>> GetProductTimelinesByProductIdAsync(int productId);
        Task<ProductTimeline?> GetTimelineByIdAsync(int timelineId);
        Task UpdateTimelineAsync(ProductTimeline timeline);
    }
}


