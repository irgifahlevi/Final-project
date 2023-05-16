using Goole_OpenId.Dtos;
using Goole_OpenId.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace Goole_OpenId.Data
{
    public interface IUserRepo
    {
        Task AddUserAsync(User user);
        Task AddUserRoleAsync(UserRole userRole);
        Task<User> GetUser(string username);
        Task UpdateUser(User user);
        Task<IEnumerable<User>> GetAllUser();
    }
}
