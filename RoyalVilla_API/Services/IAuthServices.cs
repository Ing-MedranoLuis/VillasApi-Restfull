using RoyalVilla_API.Models.DTOS;

namespace RoyalVilla_API.Services
{
    public interface IAuthServices
    {
        
        Task<bool> IsEmailExistAsync(string email);
        Task<UserDTO> RegisterAsync(RegisterRequestDTO registerRequestDTO);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);

    }
}
