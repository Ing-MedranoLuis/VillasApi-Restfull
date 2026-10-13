using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.Models.DTOS
{
    public class UserDTO
    {
        public string Id { get; set; }
        public required string Email { get; set; }

        public required string Name { get; set; }

    
        public required string Role { get; set; } = "Customer";

    }
}
