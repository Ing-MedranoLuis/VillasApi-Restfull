namespace RoyalVilla_API.Services
{
    public interface IImageServices
    {
        Task<string> UploadImage(IFormFile file);
        Task<bool> DeleteImage(string image);
        bool ValidateImage(IFormFile file);
    }
}
