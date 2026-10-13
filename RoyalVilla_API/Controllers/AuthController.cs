using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalVilla_API.Models.DTOS;
using RoyalVilla_API.Services;

namespace RoyalVilla_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    { private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

       [HttpPost("Register")]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>),StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> Register(RegisterRequestDTO registerRequestDTO)
        {

            if (registerRequestDTO == null)
            {
                return BadRequest(ApiResponse<object?>.BadRequest("User cannot be null, please provide some data", new { registerRequestDTO }));
            }
            if(await _authServices.IsEmailExistAsync(registerRequestDTO.Email))
            {
                return   BadRequest(ApiResponse<object?>.BadRequest($"The {registerRequestDTO.Email} already exist, please provide a new one", new { registerRequestDTO }));
            }

            var user = await _authServices.RegisterAsync(registerRequestDTO);
            if (user == null)
            {
                return BadRequest(ApiResponse<object?>.Conflict($"There is a conflict with the registration"));
            }
            var response = ApiResponse<UserDTO>.CreatedAt("User created successfully", user);
            return CreatedAtAction(nameof(Register),response);
        }


        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> Login(LoginRequestDTO loginRequestDTO)
        {

            if (loginRequestDTO == null)
            {
                return BadRequest(ApiResponse<object?>.BadRequest("Please fill the fields to log in", new { loginRequestDTO }));
            }
         

            var user = await _authServices.LoginAsync(loginRequestDTO);
            if (user == null)
            {
                return BadRequest(ApiResponse<object?>.BadRequest($"Password or email incorrect please check",new {loginRequestDTO }));
            }
            var response = ApiResponse<LoginResponseDTO>.Ok("User created successfully", user);
            return Ok(response);
        }
    }
}
