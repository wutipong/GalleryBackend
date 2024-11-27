using PathLib;

namespace GalleryBackend
{
    public static class Configurations
    {
        public static string BaseDirectory { get; } = "/data";
        public static PosixPath BaseDirectoryPath { get; } = new PosixPath(BaseDirectory);

        public static int ThumbnailWidth { get; } = 400;
        public static int ThumbnailHeight { get; } = 300;

        public static int ViewImageWidth { get; } = 2048;
        public static int ViewImageHeight { get; } = 2048;
    }
}
