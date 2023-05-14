using AdaDanaService.Dtos;
using AdaDanaService.Models;
using Microsoft.EntityFrameworkCore;

namespace AdaDanaService.Data
{
    public class UserService : IUserService
    {
        private readonly AdaDanaContext _context;

        public UserService(AdaDanaContext context)
        {
            _context = context;
        }

        // Tambah user ke tabel user
        public async Task AddUser(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User> FindUser(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> FindUserByUsername(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user is null)
                throw new Exception("User not found");
            return user;
        }

        public async Task<User> FindUsernameDb(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> FindUserNotBanned(string username)
        {
            return await _context.Users.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Username == username
                    && u.Deletes == false
                    && u.UserRoles.Any(ur => ur.Role.Name == "User"));
        }

        public async Task<User> FindUserBanned(string username)
        {
            return await _context.Users.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Username == username
                    && u.Deletes == true
                    && u.UserRoles.Any(ur => ur.Role.Name == "User"));
        }

        // Mengambil all user dengan role user
        public async Task<IEnumerable<User>> GetAllUser()
        {
            var users = await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name != "Admin" && ur.Role.Name != "Manager"))
                .Where(u => !u.Deletes)
                .ToListAsync();
            return users;
        }

        public async Task<User> GetUser(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(o => o.Username == username && o.Deletes == false);
            // jika user tidak ditemukan
            if (user is null)
                throw new ArgumentException($"Username '{username}' not found or is banned.");
            return user;
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserBelanceDto>> GetAllUserBelance()
        {
            var usersBelance = await _context.Users
            .Where(u => !u.UserRoles.Any(r => r.Role.Name == "Admin" || r.Role.Name == "Manager"))
            .Where(u => !u.Deletes)
            .Select(u => new UserBelanceDto
            {
                Id = u.Id,
                Username = u.Username,
                Saldo = u.Wallets.Sum(w => w.Saldo),
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
            return usersBelance;
        }
    }
}
