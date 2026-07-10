using System;
using System.Collections.Generic;
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

        // Guest
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

        // UC20 – Admin CRUD
        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Batch)
                    .ThenInclude(b => b.Supplier)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Batch)
                    .ThenInclude(b => b.Supplier)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductTimelines)
                .Include(p => p.Stories)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}

