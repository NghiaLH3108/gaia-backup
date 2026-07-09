using System;
using System.Threading.Tasks;
using GaiaWeb.DAL.Models;

namespace GaiaWeb.DAL.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetProductStoryByQRTokenAsync(Guid qrToken);
    }
}
