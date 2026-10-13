using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTOS;
using RoyalVilla_API.Services;

namespace RoyalVilla_API.Controllers
{

    [ApiController]
    [Route("api/villas")]
    [Authorize(Roles ="Admin")]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _map;
        private readonly IImageServices _image;
        public VillaController(ApplicationDbContext context, IMapper map, IImageServices image)
        {
            _context = context;
            _map = map;
            _image = image;
        }
        [HttpGet]

        [Route("AllVillas")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<VillaDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<VillaDTO>>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<VillaDTO>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<IEnumerable<VillaDTO>>>> GetVillas()
        {

            try
            {
                var villas = await _context.villa.ToListAsync();
                if (villas != null)
                {

                    var response = ApiResponse<List<VillaDTO>>.Ok("", _map.Map<List<VillaDTO>>(villas));

                    return Ok(response);
                }
                return Ok(ApiResponse<List<VillaDTO>>.Ok("No result to show", _map.Map<List<VillaDTO>>(villas)));

            }
            catch (Exception ex)
            {
                {
                    return BadRequest(ApiResponse<object>.BadRequest("your reques could't be completed" + "error: ", ex.Message));
                }

            }


        }
        [HttpGet("{id:int}", Name = "GetVillasId")]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<ApiResponse<VillaDTO>>> GetVillasId(int id)
        {
            try
            {
                if (id <= 0)
                {

                    return NotFound(ApiResponse<VillaDTO>.NotFound("Id must be greater than 0:", new { id = id }));
                }
                var villa = await _context.villa.FirstOrDefaultAsync(x => x.Id == id);
                if (villa != null)
                {
                    var response = ApiResponse<VillaDTO>.Ok("Record retrived successfully", _map.Map<VillaDTO>(villa));
                    return Ok(response);
                }

                return NotFound(ApiResponse<object>.NotFound($"There is not villa with that id:{id} ", new { id = id }));

            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponse<object>.BadRequest("your reques could't be completed. Error message: ", ex.Message));
            }
        }


        [HttpPost]
        [Route("CreateVilla")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<ApiResponse<VillasCreateDTO>>> CreateVilla([FromForm] VillasCreateDTO villaDTO)
        {
            try
            {
                if (String.IsNullOrEmpty(villaDTO.Name))
                {


                    return BadRequest(ApiResponse<VillasCreateDTO>.BadRequest("Villa name must not be empty", new { name = villaDTO.Name }));
                }
                if (villaDTO != null)
                {


                    if (villaDTO.Image != null)
                    {
                        if (!_image.ValidateImage(villaDTO.Image))
                        {
                            return BadRequest(ApiResponse<VillasCreateDTO>.BadRequest("The image is not valid", villaDTO.Image));
                        }
                        var originalImage = await _image.UploadImage(villaDTO.Image);
                        villaDTO.ImageUrl = originalImage;
                    }
                    var villaMap = _map.Map<Villa>(villaDTO);

                    villaMap.CreatedDate = DateTime.Now;
                    await _context.villa.AddAsync(villaMap);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetVillasId), new { id = villaMap.Id }, ApiResponse<VillasCreateDTO>.CreatedAt("Villa created successfully", villaDTO));
                }




                return NotFound(ApiResponse<VillasCreateDTO>.NotFound("", new { message = "Object must not be null" }));

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error ocurred " + ex.Message);
            }
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [Route("UpdateVilla")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status204NoContent)]
        public async Task<ActionResult<ApiResponse<VillaUpdateDTO>>> UpdateVilla(int id, [FromForm] VillaUpdateDTO villaDTO)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<VillaUpdateDTO>.BadRequest("Id must be greater than 0 id=", id));
                }
                var villaToUpdate = await _context.villa.FirstOrDefaultAsync(v => v.Id == id);
                if (villaToUpdate == null)
                {
                    return NotFound(ApiResponse<VillaUpdateDTO>.NotFound("We could not find a villa with that id=", id));
                }

                if (villaDTO.Image != null)
                {
                    if (!_image.ValidateImage(villaDTO.Image))
                    {
                        return BadRequest(ApiResponse<VillasCreateDTO>.BadRequest("The image is not valid", villaDTO.Image));
                    }
                    villaDTO.ImageUrl = await _image.UploadImage(villaDTO.Image);
                    if (villaToUpdate.ImageUrl != null)
                    {
                        await _image.DeleteImage(villaToUpdate.ImageUrl);
                    }

                }


                villaToUpdate.UpdatedDate = DateTime.Now;
                var response = ApiResponse<object>.NoContent("Villa updated successfully", _map.Map(villaDTO, villaToUpdate));

                await _context.SaveChangesAsync();
                return Ok(response);

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error ocurred" + ex.Message);

            }

        }


        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [Route("DeleteVilla")]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<VillaDTO>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Villa>>> DeleteVilla(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<Villa>.BadRequest("Please insert a valid number greater than 0", id));
                }
                var villaToDelete = await _context.villa.FirstOrDefaultAsync(v => v.Id == id);

                if (villaToDelete == null)
                {

                    return NotFound(ApiResponse<Villa>.BadRequest($"There is not record that match the criteria with that id={id}", id));
                }
                if (villaToDelete.ImageUrl != null)
                {
                    await _image.DeleteImage(villaToDelete.ImageUrl);

                }

                _context.villa.Remove(villaToDelete);
                await _context.SaveChangesAsync();
                return Ok(ApiResponse<Villa>.Ok("Records deleted successfully", villaToDelete));

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error ocurred" + ex.Message);

            }

        }

    }
}
