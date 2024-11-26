using NetVips;

namespace GalleryBackend
{
    public static class ImageHandlers
    {
        public static IResult CreateThumbnail(string path)
        {
            using var image = Image.NewFromFile(Path.Combine(Configurations.BaseDirectory, path), access: Enums.Access.Sequential);
            using var thumb = image.ThumbnailImage(width: Configurations.ThumbnailWidth, height: Configurations.ThumbnailHeight, crop: Enums.Interesting.Entropy);

            var output = thumb.JpegsaveBuffer();

            return Results.Bytes(output, "image/jpeg");
        }

        public static IResult CreateViewImage(string path)
        {
            var actualPath = Path.Combine(Configurations.BaseDirectory, path);
            var filename = Path.GetFileName(actualPath);

            if (MimeTypes.GetMimeType(filename) == "image/gif")
            {
                return Results.File(actualPath, contentType: "image/gif", fileDownloadName: filename);
            }

            using var image = Image.NewFromFile(actualPath, access: Enums.Access.Sequential);

            if (image.Width < Configurations.ViewImageWidth || image.Height < Configurations.ViewImageHeight)
            {
                return Results.File(actualPath, contentType: MimeTypes.GetMimeType(filename), fileDownloadName: filename);
            }

            using var thumb = image.ThumbnailImage(width: Configurations.ViewImageWidth, height: Configurations.ViewImageHeight, crop: Enums.Interesting.None);
            var output = thumb.WebpsaveBuffer();

            return Results.Bytes(output, "image/webp", fileDownloadName: $"{filename}.webp");
        }
    }
}
