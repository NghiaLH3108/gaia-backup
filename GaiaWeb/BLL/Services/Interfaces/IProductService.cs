using System;
using System.Threading.Tasks;
using GaiaWeb.DAL.Models;

namespace GaiaWeb.BLL.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product?> GetProductStoryAsync(Guid qrToken);
    }
}
