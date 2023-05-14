using System.ComponentModel.DataAnnotations;

namespace BukaToko.DTOS
{
    public class ReadOrderDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public bool Shipped { get; set; }
        [Required]
        public bool Checkout { get; set; }
    }
}
