using System.ComponentModel.DataAnnotations;

namespace BukaToko.DTOS
{
    public class ReadCartDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
