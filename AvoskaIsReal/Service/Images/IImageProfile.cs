namespace AvoskaIsReal.Service.Images
{
    public interface IImageProfile
    {
        ImageType Type { get; }
        string Folder { get; }
        // int MinHeight { get; }
        // int MinWidth { get; }
        int MaxSizeBytes { get; }
    }
}
