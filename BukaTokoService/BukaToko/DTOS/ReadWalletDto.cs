using System.ComponentModel.DataAnnotations;

namespace BukaToko.DTOS
{
    public class ReadWalletDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public decimal Cash { get; set; }
    }
}
