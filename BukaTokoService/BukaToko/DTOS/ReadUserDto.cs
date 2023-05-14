using BukaToko.Models;

namespace BukaToko.DTOS
{
    public class ReadUserDto
    {
        public string Username { get; set; } = null!;
        public int WalletId { get; set; }
    }
}
