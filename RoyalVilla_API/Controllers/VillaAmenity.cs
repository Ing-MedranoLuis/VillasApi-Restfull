using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTOS;
using RoyalVilla_API.Services;

namespace RoyalVilla_API.Controllers
{
    [Route("api/villaAmenity")]
    [ApiController]
    public class VillaAmenity : ControllerBase
    {
        private readonly IVillaAmenity _db;
        private readonly IMapper _mp;
        public VillaAmenity(IVillaAmenity db,IMapper mp)
        {
            _db = db;
            _mp = mp;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<ApiResponse<IEnumerable<VillaAmenitiesDTO>>>> AllVillasAmenities([FromQuery]string? filterByName, [FromQuery] int page=1,[FromQuery] int pageSize = 5)
        {
            try
            {
             

                if (filterByName!=null) 
                {
                    var filterdEntity =await _db.Filtering(filterByName);

                    var message = filterdEntity.Count() > 0 ? $"There are your filtered amenities with: {filterByName}" : $"No records match that criteria with this name: {filterByName}";
       
                    var response = ApiResponse<object?>.Ok(message, _mp.Map<List<VillaAmenitiesDTO>>(filterdEntity));
                    return Ok(response);
                }
                var result = await _db.GetAll(page,pageSize);
                if (result.Count() <= 0)
                {
                    var response = ApiResponse<object?>.Ok("There is not records to show", _mp.Map<List<VillaAmenitiesDTO>>(result));
                    return Ok(response);
                }

                var total=await _db.TotalRecords();
                var totalPages = (int)Math.Ceiling((double)total / pageSize);

                var messageToappend = new System.Text.StringBuilder();


                messageToappend.Append($"Total VillasAmenities are: {total}; (showing page: {page} of {totalPages})");
                

                var map = _mp.Map<List<VillaAmenitiesDTO>>(result);


                var newResponse = ApiResponse<object?>.Ok($"Here are the records {messageToappend}", map);
                return Ok(newResponse);
            }
            catch (Exception ex) {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            
            }

        }
        [HttpGet("{Id:int}")]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ApiResponse<VillaAmenitiesDTO>>> AllVillasAmenitiesById(int Id)
        {
            try
            {
                var result = await _db.GetById(Id);
                if (result == null)
                {
                    var response = ApiResponse<object?>.NotFound($"There are not records with the Id: {Id}" , new { });
                    return NotFound(response);
                }


                var map = _mp.Map<VillaAmenitiesDTO>(result);
                var newResponse = ApiResponse<object?>.Ok("Here are the records", map);
                return Ok(newResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);

            }

        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesCreateDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesCreateDTO>), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<ApiResponse<VillaAmenitiesCreateDTO>>> CreateVillaAmenity(VillaAmenitiesCreateDTO villaAmenitiesCreateDTO)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("Name", "Name could not be empty");
                    return Conflict(ModelState);
                }

                var createdRecord=_mp.Map<VillaAmenities>(villaAmenitiesCreateDTO);
                if(createdRecord ==null)
                {
                    return BadRequest(ApiResponse<VillaAmenitiesCreateDTO>.BadRequest("Model is totally null, please check",  createdRecord));
                }
                
                await _db.Create(createdRecord);
                var response = ApiResponse<VillaAmenitiesCreateDTO>.CreatedAt("Record created successfully", _mp.Map<VillaAmenitiesCreateDTO>(createdRecord));
                return CreatedAtAction(nameof(CreateVillaAmenity),response);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);

            }

        }


        [HttpPut("{Id:int}")]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesCreateDTO>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesCreateDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesCreateDTO>), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ApiResponse<VillaAmenitiesCreateDTO>>> UpdateVillaAmenity(int Id,VillaAmenitiesUpdateDTO villaAmenitiesUpdateDTO)
        {
            try
            {

                ApiResponse<object?> response;
                if (!ModelState.IsValid)
                {
                   ModelState.AddModelError("Name", "Name is required");
                    return BadRequest(ModelState);
                }
                if (Id <= 0)
                {
                     response = ApiResponse<object?>.BadRequest("Please insert an Id greater than 0", new { });
                    return BadRequest(response);
                }
                var retrivedData =await _db.GetById(Id);
                if (retrivedData == null)
                {
                     response = ApiResponse<object?>.BadRequest($"No records match with the Id provided: {Id}", new { });
                    return NotFound(response);
                }

               var dataToUpdate= _mp.Map(villaAmenitiesUpdateDTO, retrivedData);
               await _db.Update(dataToUpdate);
              

                return NoContent();



             
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);

            }

        }


        [HttpDelete("{Id:int}")]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesCreateDTO>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesCreateDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesCreateDTO>), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ApiResponse<VillaAmenitiesCreateDTO>>> DeleteVillaAmenities(int Id)
        {
            try
            {

                ApiResponse<object?> response;
               
                if (Id <= 0)
                {
                    response = ApiResponse<object?>.BadRequest("Please insert an Id greater than 0", new { });
                    return BadRequest(response);
                }
       
               if(!await _db.DeleteById(Id)){
                    response = ApiResponse<object?>.NotFound($"No records match with the Id provided: {Id}", new { });
                    return NotFound(response);
                }

                response = ApiResponse<object?>.Ok($"Record deleted with the Id: {Id}", new { });
                return Ok(response);




            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);

            }

        }
    }
}
