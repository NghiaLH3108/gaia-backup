using System;
using System.Threading.Tasks;
using GaiaWeb.DAL.Data;
using GaiaWeb.DAL.Models;
using GaiaWeb.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GaiaWeb.DAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly GaiaDbContext _context;

        public ProductRepository(GaiaDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetProductStoryByQRTokenAsync(Guid qrToken)
        {
            return await _context.Products
                .Include(p => p.Batch)
                    .ThenInclude(b => b.Supplier)
                .Include(p => p.Batch)
                    .ThenInclude(b => b.MaterialImages)
                .Include(p => p.ProductTimelines)
                .Include(p => p.Stories)
                .FirstOrDefaultAsync(p => p.Qrtoken == qrToken);
        }
    }
}
