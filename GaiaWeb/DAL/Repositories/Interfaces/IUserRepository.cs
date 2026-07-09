using GaiaWeb.DAL.Models;
using System.Threading.Tasks;

namespace GaiaWeb.DAL.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
}
