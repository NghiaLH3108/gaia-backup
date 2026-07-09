using System;
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

        public async Task<Product?> GetProductStoryAsync(Guid qrToken)
        {
            var product = await _productRepository.GetProductStoryByQRTokenAsync(qrToken);

            if (product != null)
            {
                // Ensure ordering is correct as per UI logic
                if (product.ProductTimelines != null)
                {
                    product.ProductTimelines = product.ProductTimelines.OrderBy(t => t.StepOrder).ToList();
                }

                if (product.Stories != null)
                {
                    product.Stories = product.Stories.OrderBy(s => s.DisplayOrder).ToList();
                }
            }

            return product;
        }
    }
}
