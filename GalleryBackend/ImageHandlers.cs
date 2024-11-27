using NetVips;
using PathLib;
using Utility;

namespace GalleryBackend
{
    public static class ImageHandlers
    {
        private static Stream GetStream(string path)
        {
            var actualPath = Path.Combine(Configurations.BaseDirectory, path);
            var paths = PathUtility.SplitPathAfterArchiveFile(actualPath);

            if (paths.Length == 1)
            {
                return PhysicalFS.ReadFile(actualPath);
            }

            if (paths.Length == 2)
            {
                return ArchiveFS.ReadFile(paths[0], paths[1]);
            }

            throw new InvalidPathException(path, "Nested archive is not supported");
        }
        public static IResult CreateThumbnail(string path)
        {
            using var stream = GetStream(path);

            using var image = Image.NewFromStream(stream);
            using var thumb = image.ThumbnailImage(
                width: Configurations.ThumbnailWidth,
                height: Configurations.ThumbnailHeight,
                crop: Enums.Interesting.Entropy
            );

            var output = thumb.JpegsaveBuffer();

            return Results.Bytes(output, "image/jpeg");
        }

        public static IResult CreateViewImage(string path)
        {
            var filename = Path.GetFileName(path);

            if (MimeTypes.GetMimeType(filename) == "image/gif")
            {
                return Results.Stream(GetStream(path), contentType: "image/gif", fileDownloadName: filename);
            }

            using var stream = GetStream(path);
            using var image = Image.NewFromStream(stream);

            byte[] output;
            if (image.Width < Configurations.ViewImageWidth || image.Height < Configurations.ViewImageHeight)
            {
                output = image.WebpsaveBuffer();
            }
            else
            {
                using var thumb = image.ThumbnailImage(
                    width: Configurations.ViewImageWidth,
                    height: Configurations.ViewImageHeight,
                    crop: Enums.Interesting.None);

                output = thumb.WebpsaveBuffer();
            }

            return Results.Bytes(output, "image/webp", fileDownloadName: $"{filename}.webp");
        }
    }
}
