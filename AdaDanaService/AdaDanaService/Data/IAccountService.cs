using AdaDanaService.Dtos;
using AdaDanaService.Models;

namespace AdaDanaService.Data
{
    public interface IAccountService
    {
        Task<bool> Register(RegisterUserDto registerUserDto);
        Task<UserToken> Login(LoginDto login);
        Task UpdatePasswordUser(UpdatePassword updatePassword);
        Task Banned(string username);
        Task Unbanned(string username);
    }
}
