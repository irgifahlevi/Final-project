using BukaToko.DTOS;
using BukaToko.Models;

namespace BukaToko.SyncService
{
    public interface IGooleDataClient
    {
        Task<UserToken> SendUserToGoole(LoginUserDto user);
    }
}
