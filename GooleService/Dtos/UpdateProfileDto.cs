using System.ComponentModel.DataAnnotations;

namespace Goole_OpenId.Dtos
{
    public class UpdateProfileDto
    {
        public string? Fullname { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
    }
}
