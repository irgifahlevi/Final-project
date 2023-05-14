using System.ComponentModel.DataAnnotations;

namespace BukaToko.DTOS
{
    public class UpdateProductDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
