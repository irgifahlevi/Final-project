using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AdaDanaService.Dtos;
using AdaDanaService.Models;
using Microsoft.EntityFrameworkCore;

namespace AdaDanaService.Data
{
    public class WalletService : IWalletService
    {
        private readonly AdaDanaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WalletService(AdaDanaContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task TopUp(int userId, int saldo)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                throw new Exception("Gagal mengambil nama pengguna dari token.");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userName);

            if (user == null)
            {
                throw new Exception("Pengguna tidak ditemukan.");
            }

            var wallet = await _context.Wallets.SingleOrDefaultAsync(w => w.UserId == user.Id);

            if (wallet == null)
            {
                throw new Exception("Dompet pengguna tidak ditemukan.");
            }

            wallet.Saldo += saldo;

            await _context.SaveChangesAsync();
        }

        public async Task CashOut(int userId, int saldo)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                throw new Exception("Gagal mengambil nama pengguna dari token.");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userName);

            if (user == null)
            {
                throw new Exception("Pengguna tidak ditemukan.");
            }

            var wallet = await _context.Wallets.SingleOrDefaultAsync(w => w.UserId == user.Id);

            if (wallet == null)
            {
                throw new Exception("Dompet pengguna tidak ditemukan.");
            }

            if (saldo > wallet.Saldo)
            {
                throw new Exception("Saldo tidak mencukupi untuk melakukan penarikan.");
            }

            wallet.Saldo -= saldo;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetBalance(int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet != null)
            {
                return wallet.Saldo;
            }
            else
            {
                throw new Exception("Wallet not found");
            }
        }

        public async Task AddWallet(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
        }

        public void CashoutOtherService(int walletId, int saldo)
        {
            var wallet = _context.Wallets.FirstOrDefault(w => w.Id == walletId);
            if (wallet != null)
            {
                wallet.Saldo -= saldo;
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException($"Wallet with Id '{walletId}' does not exist");
            }
        }
        public bool WalletExists(int walletId)
        {
            return _context.Wallets.Any(w => w.Id == walletId);
        }
        public void WalletUpdate(Wallet wallet)
        {
            wallet.UpdatedAt = DateTime.Now;

            // update data pada tabel wallet
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }

        public Wallet GetWalletByUserId(int userId)
        {
            return _context.Wallets.FirstOrDefault(w => w.UserId == userId);
        }

        public User ExternalUserExists(string username)
        {
            var user = _context.Users.FirstOrDefault(o => o.Username == username);
            return user;
        }

    }
}