using AvoskaIsReal.Service.Images;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace AvoskaIsReal.Controllers
{
    public class UploadController : Controller
    {
        private ImageService _imageService;
        public UploadController(ImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost]
        public IActionResult UploadImage(IFormFile file)
        {
            return Upload(file, ImageType.ContentImage);
        }

        [HttpPost]
        public IActionResult UploadAvatar(IFormFile file)
        {
            return Upload(file, ImageType.Avatar);
        }

        private IActionResult Upload(IFormFile file, ImageType type)
        {
            if (file.Length == 0)
                return BadRequest();

            if (User.Identity is null || !(User.Identity.IsAuthenticated))
            {
                return Unauthorized();
            }

            try
            {
                string fileUrl = _imageService.SaveImage(file, type);
                return Json(new { location = fileUrl });
            }
            catch (ImageProcessingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
