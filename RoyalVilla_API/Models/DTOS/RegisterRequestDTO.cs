using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.Models.DTOS
{
    public class RegisterRequestDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Role { get; set; } = "Customer";

    }
}
