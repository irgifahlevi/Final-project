using AdaDanaService.Dtos;
using AdaDanaService.Models;

namespace AdaDanaService.Data
{
    public interface IUserService
    {
        Task AddUser(User user);
        Task<User> FindUserByUsername(string username);
        Task<User> FindUsernameDb(string username);
        Task<User> FindUser(string username);
        Task<User> FindUserNotBanned(string username);
        Task<User> FindUserBanned(string username);
        Task<IEnumerable<User>> GetAllUser();
        Task<IEnumerable<UserBelanceDto>> GetAllUserBelance();
        Task<User> GetUser(string username);
        Task UpdateUser(User user);
    }
}
