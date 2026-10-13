using AutoMapper;
using RoyalVilla_API.Models.DTOS;

namespace RoyalVilla_API.Models.Mapper
{
    public class MapperSettings:Profile
    {
        public MapperSettings()
        {
            CreateMap<Villa, VillasCreateDTO>().ReverseMap();
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
            CreateMap<Villa, VillaDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<VillaAmenities, VillaAmenitiesUpdateDTO>().ReverseMap();
            CreateMap<VillaAmenities, VillaAmenitiesCreateDTO>().ReverseMap();
            CreateMap<VillaAmenitiesDTO, VillaAmenities>();
            CreateMap<VillaAmenities, VillaAmenitiesDTO>().ForMember(dest => dest.villaName, opt => opt.MapFrom(src => src.villa != null ? src.villa.Name : null));
            //.ForMember(dest => dest.villaName, opt => opt.MapFrom(src => src.villa != null ? src.villa.Name : null));       CreateMap<VillaAmenitiesDTO, VillaAmenities>();

        }
    }
}
