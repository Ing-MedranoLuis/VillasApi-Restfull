using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTOS;

namespace RoyalVilla_API.Services
{
    public class VillaAmenity : IVillaAmenity
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public VillaAmenity(IMapper mapper,ApplicationDbContext context)
        {_context = context; 
            _mapper = mapper;
            
        }

        public async Task<bool> Create(VillaAmenities villaAmenities)
        {
            var result = VillaAmenetyExist(villaAmenities.Name);
            if (result)
            {
                return false;
            }
            villaAmenities.CreatedDate = DateTime.Now;
            await _context.villaAmenities.AddAsync(villaAmenities);
            return await Save();
        }

        public async Task<bool> DeleteById(int id)
        {
            var record=_context.villaAmenities.FirstOrDefault(x => x.Id == id);
            if (record == null)
            {
                return await Task.FromResult(false);
            }
             _context.villaAmenities.Remove(record);

            return await Save();
        }

        public async Task<IEnumerable<VillaAmenities>> Filtering(string name)
        {
        var query= _context.villaAmenities.AsQueryable();
            if (!String.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.ToLower() == name.ToLower());
            }
            return await query.ToListAsync();
        
        }

        public async Task<IEnumerable<VillaAmenities>> GetAll(int page = 1, int pageSize = 5)
        {
            if(page<1)page = 1;
            if (pageSize < 1)pageSize= 5;
            if (pageSize > 100) pageSize = 100;
            var skip = (page - 1) * pageSize;
            return await _context.villaAmenities.Skip(skip).Take(pageSize).OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<VillaAmenities?> GetById(int id)
        {
          
            
                return await _context.villaAmenities.FirstOrDefaultAsync(u => u.Id == id);
            
         
          
            
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0 ? true : false;
        }

        public async Task<int> TotalRecords()
        {
          return await _context.villaAmenities.CountAsync();
        }

        public async Task<bool> Update(VillaAmenities entity)
        {
            try
            {
                if (String.IsNullOrEmpty(entity.Name))
                {
                    return false;
                }
                entity.UpdatedDate= DateTime.Now;
                _context.villaAmenities.Update(entity);
                return await Save();

            }
            catch (Exception ex) 
            {

                throw new NotImplementedException("Something went wrong" + ex);
            
            }
          
            
        }

        public bool VillaAmenetyExist(string villaAmenityName)
        {
            return  _context.villaAmenities.Any(v => v.Name.ToLower() == villaAmenityName.ToLower());
        }
    }
}
