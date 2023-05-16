using Goole_OpenId.Models;

namespace Goole_OpenId.Data
{
    public interface IRoleRepo
    {
        Task<Role> GetRoleMemberAsync();
    }
}
