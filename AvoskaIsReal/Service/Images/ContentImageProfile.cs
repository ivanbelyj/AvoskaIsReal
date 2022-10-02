namespace AvoskaIsReal.Service.Images
{
    public class ContentImageProfile : IImageProfile
    {
        private const int MB = 1048576;
        public ImageType Type => ImageType.ContentImage;

        public string Folder => "uploaded-images";

        // public int MinHeight => 1024;
        // public int MinWidth => 1024;

        public int MaxSizeBytes => 10 * MB;

        public bool CropToSquare => false;
    }
}
