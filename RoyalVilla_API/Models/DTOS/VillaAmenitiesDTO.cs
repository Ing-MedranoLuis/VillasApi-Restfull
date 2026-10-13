using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.Models.DTOS
{
    public class VillaAmenitiesDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public string? Description { get; set; }



       

        public int villaId { get; set; }

        public string? villaName { get; set; }
    }
}
