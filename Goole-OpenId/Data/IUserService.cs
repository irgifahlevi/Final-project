using Goole_OpenId.Dtos;
using Goole_OpenId.Models;

namespace Goole_OpenId.Data
{
    public interface IUserService
    {
        Task RegisterUserAsync(User user);
        Task<UserToken> LoginUserAsync(LoginDto login);
        Task UpdateProfileUser(UpdateProfileDto updateProfileDto);
        Task UpdatePassword(string password);
        Task<bool> Banned(string username);
    }
}
