using BukaToko.DTOS;
using BukaToko.Models;

namespace BukaToko.Data
{
    public interface IAccountRepo
    {
        string Register (User user);
        string RegisterManager(User user);
        string RegisterAdmin(User user);
        UserToken Login (LoginUserDto user);
        bool SaveChanges();
        string Banned(BannedUserDto bannedUser);
    }
}
