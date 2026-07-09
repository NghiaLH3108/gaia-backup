using GaiaWeb.DAL.Data;
using GaiaWeb.DAL.Models;
using GaiaWeb.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GaiaWeb.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GaiaDbContext _context;

    public UserRepository(GaiaDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Supplier)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Supplier)
            .FirstOrDefaultAsync(u => u.UserId == id);
    }
}
