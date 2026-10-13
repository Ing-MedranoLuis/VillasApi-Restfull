using RoyalVilla_API.Models;
using System.Reflection.Metadata.Ecma335;

namespace RoyalVilla_API.Services
{
    public interface IVillaAmenity
    {
        Task<IEnumerable<VillaAmenities>> GetAll(int page=1, int pageSize=5);
        Task<VillaAmenities?> GetById(int id);
        Task<bool> Create(VillaAmenities villaAmenities);

        bool VillaAmenetyExist(string villaAmenityName);
        Task<bool> Update(VillaAmenities entity);

        Task<bool> Save();
        Task<bool> DeleteById(int id);

        Task<IEnumerable<VillaAmenities>> Filtering(string name);

        Task<int> TotalRecords();

    }
}
