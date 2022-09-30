namespace AvoskaIsReal.Service.Images
{
    public class AvatarProfile : IImageProfile
    {
        private const int MB = 1048576;
        public ImageType Type => ImageType.Avatar;

        public string Folder => "uploaded-avatars";

        // public int MinHeight => 300;

        // public int MinWidth => 300;

        public int MaxSizeBytes => 5 * MB;
    }
}
