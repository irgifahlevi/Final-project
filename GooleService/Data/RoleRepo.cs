using Goole_OpenId.Models;
using Microsoft.EntityFrameworkCore;

namespace Goole_OpenId.Data
{
    public class RoleRepo : IRoleRepo
    {
        private readonly GooleDbContext _context;
        public RoleRepo(GooleDbContext context)
        {
            _context = context;
        }
        public async Task<Role> GetRoleMemberAsync()
        {
            var role = await _context.Roles.Where(o => o.NameRole == "admin").FirstOrDefaultAsync();
            if (role == null)
            {
                throw new Exception("Role not found");
            }
            return role;
        }
    }
}
