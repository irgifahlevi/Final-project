using BukaToko.Models;

namespace BukaToko.Data
{
    public interface IWalletRepo
    {
        Task<Wallet> TopUp(string username, int cash);
        bool ExternalWalletExists(string externalUsername);
        Task<IEnumerable<Wallet>> GetAllWallets();
        bool SaveChanges();
    }
}
