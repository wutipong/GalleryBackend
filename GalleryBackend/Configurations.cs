using PathLib;

namespace GalleryBackend
{
    public static class Configurations
    {
        public static string BaseDirectory { get; } = "/data";
        public static PosixPath BaseDirectoryPath { get; } = new PosixPath(BaseDirectory);

        public static int GridThumbnailWidth { get; } = 400;
        public static int GridThumbnailHeight { get; } = 300;

        public static int ListThumbnailWidth { get; } = 96;
        public static int ListThumbnailHeight { get; } = 64;

        public static int ViewImageWidth { get; } = 2048;
        public static int ViewImageHeight { get; } = 2048;
    }
}
