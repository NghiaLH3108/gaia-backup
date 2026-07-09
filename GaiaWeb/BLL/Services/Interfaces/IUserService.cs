using GaiaWeb.DAL.Models;
using System.Threading.Tasks;

namespace GaiaWeb.BLL.Services.Interfaces;

public interface IUserService
{
    Task<User?> LoginAsync(string email, string password);
}
