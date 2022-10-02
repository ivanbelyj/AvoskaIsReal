using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace AvoskaIsReal.Service.Images
{
    public class ImageService
    {
        private IEnumerable<IImageProfile> _imageProfiles;
        private IWebHostEnvironment _webHostEnvironment;

        private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png",
            ".gif" };
        public string[] AllowedExtensions => _allowedExtensions;

        public ImageService(IEnumerable<IImageProfile> imageProfiles,
            IWebHostEnvironment webHostEnvironment)
        {
            _imageProfiles = imageProfiles;
            _webHostEnvironment = webHostEnvironment;
        }

        public string SaveImage(IFormFile file, ImageType type)
        {
            IImageProfile? imageProfile = _imageProfiles.FirstOrDefault(p
                => p.Type == type);
            if (imageProfile is null)
                throw new ImageProcessingException("Профиль изображения не найден.");

            ValidateExtenstion(file);
            ValidateFileSize(file, imageProfile);

            Image image = Image.Load(file.OpenReadStream());
            // ValidateImageSize(image, imageProfile);

            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath,
                imageProfile.Folder);
            if (!File.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName;
            string filePath;
            do
            {
                fileName = UniqueFileName(file, imageProfile.Type.ToString());
                filePath = Path.Combine(folderPath, fileName);
            } while (File.Exists(filePath));

            // Resize(image, imageProfile);
            if (imageProfile.CropToSquare)
                CropToSquare(image);

            image.Save(filePath, new JpegEncoder() { Quality = 75 });
            return "/" + Path.Combine(imageProfile.Folder, fileName).PathToUrl();
        }

        private void CropToSquare(Image image)
        {
            var rectangle = GetCropRectangle(image);
            image.Mutate(action => action.Crop(rectangle));
        }

        private Rectangle GetCropRectangle(IImageInfo image)
        {
            if (image.Width > image.Height)
            {
                return new Rectangle((image.Width - image.Height) / 2, 0,
                    image.Height, image.Height);
            }
            else
            {
                return new Rectangle(0, (image.Height - image.Width) / 2,
                    image.Width, image.Width);
            }
        }

        //private void Resize(Image image, IImageProfile imageProfile)
        //{
        //    var resizeOptions = new ResizeOptions
        //    {
        //        Mode = ResizeMode.Min,
        //        Size = new Size(imageProfile.MinWidth)
        //    };

        //    image.Mutate(action => action.Resize(resizeOptions));
        //}


        private void ValidateExtenstion(IFormFile file)
        {
            if (AllowedExtensions.Contains(Path.GetExtension(file.FileName)))
                return;

            throw new ImageProcessingException("Неверный формат файла. Допустимы: "
                + string.Join(", ", AllowedExtensions));
        }

        private void ValidateFileSize(IFormFile file, IImageProfile profile)
        {
            if (file.Length > profile.MaxSizeBytes)
                throw new ImageProcessingException($"Размер файла равен {file.Length}"
                    + $" байт и превышает максимальный - {profile.MaxSizeBytes}");
        }
        //private void ValidateImageSize(Image image, IImageProfile profile)
        //{
        //    if (image.Width < profile.MinWidth || image.Height < profile.MinHeight)
        //        throw new ImageProcessingException($"Размер файла {image.Width}x{image.Height}"
        //            + $" меньше минимального - {profile.MinWidth}x{profile.MinHeight}");
        //}
        private string UniqueFileName(IFormFile file, string prefix)
        {
            string fileName = Path.GetRandomFileName();
            string fileExtension = Path.GetExtension(file.FileName);
            return $"{prefix}-{fileName}{fileExtension}";
        }
    }
}
