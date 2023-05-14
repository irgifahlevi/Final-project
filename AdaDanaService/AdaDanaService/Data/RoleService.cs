using AdaDanaService.Models;
using Microsoft.EntityFrameworkCore;

namespace AdaDanaService.Data
{
    public class RoleService : IRoleService
    {
        private readonly AdaDanaContext _context;

        public RoleService(AdaDanaContext context)
        {
            _context = context;
        }

        // Ambil role name dari tabel role
        public async Task<Role> GetRoleUser()
        {
            var roleName = await _context.Roles.Where(o => o.Name == "User").FirstOrDefaultAsync();
            if (roleName is null)
                throw new Exception("Role name not found");
            return roleName;
        }
    }
}
