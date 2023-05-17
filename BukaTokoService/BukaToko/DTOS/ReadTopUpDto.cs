using System.ComponentModel.DataAnnotations;

namespace BukaToko.DTOS
{
    public class ReadTopUpDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public int Cash { get; set; }
    }
}
