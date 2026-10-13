namespace RoyalVilla_API.Models.DTOS
{
    public class LoginResponseDTO
    {
        public string? token { get; set; }
        public UserDTO? user { get; set; }
    }
}
