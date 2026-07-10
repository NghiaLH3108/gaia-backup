using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using GaiaWeb.DAL.Repositories.Interfaces;

namespace GaiaWeb.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Guest
        public async Task<Product?> GetProductStoryAsync(Guid qrToken)
        {
            var product = await _productRepository.GetProductStoryByQRTokenAsync(qrToken);

            if (product != null)
            {
                if (product.ProductTimelines != null)
                    product.ProductTimelines = product.ProductTimelines.OrderBy(t => t.StepOrder).ToList();

                if (product.Stories != null)
                    product.Stories = product.Stories.OrderBy(s => s.DisplayOrder).ToList();
            }

            return product;
        }

        // UC20 – Admin CRUD
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }

        public async Task<(bool Success, string Message)> CreateProductAsync(
            int batchId, string productName, string description, string currentStatus)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return (false, "Tên sản phẩm không được để trống.");

            var product = new Product
            {
                BatchId = batchId,
                ProductName = productName,
                Description = description,
                Qrtoken = Guid.NewGuid(),
                CurrentStatus = currentStatus,
                CreatedDate = DateTime.Now
            };

            await _productRepository.CreateAsync(product);
            return (true, $"Tạo sản phẩm '{productName}' thành công. QR Token: {product.Qrtoken}");
        }

        public async Task<(bool Success, string Message)> UpdateProductAsync(
            int productId, string productName, string description, string currentStatus)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return (false, "Không tìm thấy sản phẩm.");

            if (string.IsNullOrWhiteSpace(productName))
                return (false, "Tên sản phẩm không được để trống.");

            product.ProductName = productName;
            product.Description = description;
            product.CurrentStatus = currentStatus;

            await _productRepository.UpdateAsync(product);
            return (true, "Cập nhật sản phẩm thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return (false, "Không tìm thấy sản phẩm.");

            await _productRepository.DeleteAsync(productId);
            return (true, $"Đã xóa sản phẩm '{product.ProductName}'.");
        }
    }
}

