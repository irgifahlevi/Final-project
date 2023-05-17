using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BukaToko.DTOS
{
    public class ReadProductDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
