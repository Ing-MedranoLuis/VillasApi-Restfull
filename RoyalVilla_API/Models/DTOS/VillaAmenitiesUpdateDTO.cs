using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.Models.DTOS
{
    public class VillaAmenitiesUpdateDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public string? Description { get; set; }



        [Required]

        public int villaId { get; set; }

    }
}
