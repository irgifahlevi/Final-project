using BukaToko.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BukaToko.Data
{
    public class WalletRepo : IWalletRepo
    {
        private readonly BukaTokoDbContext _context;

        public WalletRepo(BukaTokoDbContext context)
        {
            _context = context;
        }

        public bool ExternalWalletExists(string externalUsername)
        {
            return _context.Wallets.Any(w => w.Username == externalUsername);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
        public async Task<IEnumerable<Wallet>> GetAllWallets()
        {
            return await _context.Wallets.ToListAsync();
        }
        public async Task<Wallet> TopUp(string username, int cash)
        {
            var wallet =  _context.Wallets.FirstOrDefault(w => w.Username == username);
            if (wallet != null)
            {
                wallet.Cash += cash;
                _context.SaveChanges();
                return wallet;
            }
            else
            {
                throw new ArgumentException("Username not found");
            }
        }
    }
}
