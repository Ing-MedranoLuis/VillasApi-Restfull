namespace RoyalVilla_API.Services
{
    public class ImageServices : IImageServices
    {
        private readonly IWebHostEnvironment _web;
        private readonly double MaxFileSize = 5 * 1024 * 1024; //5MB
        private readonly string[] AllowedExtension = { ".jpg", ".png", ".jpeg", ".web", ".gif" };

        public ImageServices(IWebHostEnvironment web)
        {
            _web = web;
        }
        public async Task<bool> DeleteImage(string image)
        {
            if (String.IsNullOrEmpty(image))
            {
                return false;
            }

            var pathFileName = Path.GetFileName(image);

            var folder = Path.Combine(_web.WebRootPath, "Images", "Villas",pathFileName);

            if (File.Exists(folder)) { 
            
            await Task.Run(() => File.Delete(folder));
                return true;
            }
            return false;

        }



        public async Task<string> UploadImage(IFormFile file)
        {

            try
            {
                if (!ValidateImage(file))
                {
                    throw new NotImplementedException("File is not valid");
                }

                var upLoadFolder = Path.Combine(_web.WebRootPath, "Images", "Villas");
                if (!Directory.Exists(upLoadFolder))
                {
                    Directory.CreateDirectory(upLoadFolder);
                }

                var fileExtention = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtention}";

                var filePath = Path.Combine(upLoadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return $"Images/Villas/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("", ex);
            }
        }
        public bool ValidateImage(IFormFile file)
        {
            if (file.Length > MaxFileSize)
            {
                return false;
            }
            if (file.Length == 0 || file == null)
            {
                return false;
            }

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtension.Contains(extension.ToLower()))
            {
                return false;
            }
            return true;
        }
    }
}
