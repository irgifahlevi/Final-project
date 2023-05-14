using AdaDanaService.Models;

namespace AdaDanaService.Data
{
    public interface IRoleService
    {
        Task<Role> GetRoleUser();
    }
}
